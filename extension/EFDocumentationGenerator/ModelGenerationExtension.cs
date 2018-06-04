﻿//  Entity Designer Documentation Generator
//  Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator.ConnectionStrings;
using DocumentationGenerator.Diagnostics;
using DocumentationGenerator.Utilities;
using EnvDTE;
using EnvDTE80;
using Microsoft.Data.Entity.Design.Extensibility;

namespace DocumentationGenerator
{
    /// <summary>
    /// The entry point to the extension.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelGenerationExtension))]
    public class ModelGenerationExtension : IModelGenerationExtension
    {
        /// <summary>
        /// Initializes a new <see cref="ModelGenerationExtension"/>.
        /// </summary>
        /// <param name="logger">Used for logging informational messages</param>
        /// <param name="connectionStringLocator">Used for retrieving a connection string</param>
        /// <param name="errorList">A read-only view of the Error List</param>
        [ImportingConstructor]
        public ModelGenerationExtension(ILogger logger, IConnectionStringLocator connectionStringLocator, IReadOnlyList<ErrorItem> errorList)
            : this(logger,
                   connectionStringLocator,
                   connectionString => new DatabaseDocumentationSource(new SqlConnection(connectionString)),
                   source => new ModelDocumentationUpdater(source),
                   errorList)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ModelGenerationExtension"/>.
        /// </summary>
        /// <param name="logger">Used for logging informational messages</param>
        /// <param name="connectionStringLocator">Used for retrieving a connection string</param>
        /// <param name="documentationSourceFactory">Creates <see cref="IDocumentationSource"/> objects</param>
        /// <param name="modelUpdaterFactory">Creates objects that populate an EDMX model's documentation nodes</param>
        /// <param name="errorList">A read-only view of the Error List</param>
        public ModelGenerationExtension(
            ILogger logger, 
            IConnectionStringLocator connectionStringLocator, 
            Func<string, IDocumentationSource> documentationSourceFactory,
            Func<IDocumentationSource, IModelDocumentationUpdater> modelUpdaterFactory,
            IReadOnlyList<ErrorItem> errorList)
        {
            _logger = logger;
            _connectionStringLocator = connectionStringLocator;
            _documentationSourceFactory = documentationSourceFactory;
            _modelUpdaterFactory = modelUpdaterFactory;
            _errorList = errorList;
        }

        /// <summary>
        /// Called after an .edmx document is generated by the Entity Data Model Wizard or the Update Model Wizard.
        /// </summary>
        /// <param name="context">
        /// context.CurrentDocument = The XDocument that will be saved.
        ///                           An extension can modify this document. Note that the document may have been modified by another extension's implementation of OnAfterModelGenerated().
        /// 
        /// context.GeneratedDocument = The original XDocument that was generated Entity Data Model Wizard or the Update Model Wizard.
        ///                             An extension cannot modify this document.
        /// 
        /// context.Project = The EnvDTE.Project that contains the .edmx file
        /// 
        /// context.WizardKind = The wizard that initiated the .edmx file generation or update process. Possible values are WizardKind.Generate or WizardKind.UpdateModel.
        /// </param>
        public void OnAfterModelGenerated(ModelGenerationExtensionContext context)
        {
            // Capture any model errors that were NOT created by this extension.
            _edmxErrors = _errorList.Where(error =>
                                    error.FileName.EndsWith(".edmx") &&
                                    error.Description.StartsWith("Error") &&
                                    error.Project == context.Project.UniqueName).ToList();

            // When in Update mode, both methods will be called and only one of them needs to execute.
            if (context.WizardKind == WizardKind.Generate)
                UpdateModel(context.Project, context.CurrentDocument, context.WizardKind);
        }

        /// <summary>
        /// Called after a model is updated by the Update Model Wizard.
        /// Note: the Update Model Wizard generates a temporary .edmx document which is then merged with the existing document 
        /// to produce the updated document. The OnAfterModelGenerated() method will be called on the temporary document before 
        /// the merge process begins. This OnAfterModelUpdated() method allows you to make further changes to the document 
        /// after it has been merged with the existing document.
        /// </summary>
        /// <param name="context">
        /// context.OriginalDocument = The original XDocument before the Update Model Wizard started.
        ///                            An extension cannot modify this document.
        /// 
        /// context.GeneratedDocument = The temporary XDocument that was generated by the Update Model wizard from the database.
        ///                             An extension cannot modify this document.
        /// 
        /// context.UpdateModelDocument = The contents of context.OriginalDocument merged with the contents of context.GeneratedDocument.
        ///                               An extension cannot modify this document.
        /// 
        /// context.CurrentDocument = The XDocument that will be saved.
        ///                           An extension can modify this document. Note that the document may have been modified by another extension's implementation of OnAfterModelUpdated().
        /// 
        /// context.ProjectItem = The EnvDTE.ProjectItem of current .edmx file.
        /// 
        /// context.Project = The EnvDTE.Project that contains the .edmx file.
        /// 
        /// context.WizardKind = The wizard that initiated the .edmx file generation or update process (WizardKind.UpdateModel).
        /// </param>
        public void OnAfterModelUpdated(UpdateModelExtensionContext context)
        {
            // Emit a warning if model errors already existed, particularly to avoid this extension getting blamed for them.
            if (_edmxErrors.Count > 0)
                _logger.LogTimestamped("Warning - Model contains errors prior to update. This plugin may erroneously be blamed for them.");

            UpdateModel(context.Project, context.CurrentDocument, context.WizardKind);
        }

        private void UpdateModel(Project project, XDocument currentDocument, WizardKind mode)
        {
            bool isEFv2Model = project.IsEntityFrameworkV2Model();
            if (!isEFv2Model)
            {
                _logger.Log("Could not generate documentation because the entity model targets an older framework.");
                return;
            }

            _logger.LogTimestamped("------ Starting documentation generation for project: {0} ------", project.UniqueName);

            // Attempt to find the database connection string.
            SqlConnectionStringBuilder connectionString;
            try
            {
                connectionString = _connectionStringLocator.Locate(project);
            }
            catch (ConnectionStringLocationException exception)
            {
                _logger.Log("Connection string could not be located for project '{0}': {1}", project.Name, exception.Message);
                if (mode == WizardKind.Generate)
                    _logger.Log("Try updating the model after initial generation and the connection string has been saved to a config file.");

                _logger.LogTimestamped("Documentation generation failed");
                return;
            }

            using (var docSource = _documentationSourceFactory(connectionString.ToString()))
            {
                _modelUpdaterFactory(docSource).UpdateDocumentation(currentDocument);
            }

            _logger.LogTimestamped("Documentation generation succeeded");
        }

        private ICollection<ErrorItem> _edmxErrors; 

        private readonly IConnectionStringLocator _connectionStringLocator;
        private readonly Func<string, IDocumentationSource> _documentationSourceFactory;
        private readonly Func<IDocumentationSource, IModelDocumentationUpdater> _modelUpdaterFactory;
        private readonly IReadOnlyList<ErrorItem> _errorList;
        private readonly ILogger _logger;
    }
}