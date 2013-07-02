﻿//  Entity Designer Documentation Generator
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
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

using System.ComponentModel.Composition;
using DocumentationGenerator.Utilities;
using Microsoft.Data.Entity.Design.Extensibility;

namespace DocumentationGenerator
{
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IModelGenerationExtension))]
	public class ModelGenerationExtension : IModelGenerationExtension
	{
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
			bool isEFv2Model = context.Project.IsEntityFrameworkV2Model();
			if (isEFv2Model)
			{
				var connectionString = new ConnectionStringLocator().Locate(context.Project);
				using (var docSource = new DatabaseDocumentationSource(connectionString.ToString()))
				{
					new DocumentationUpdater(docSource)
							.UpdateDocumentation(context.CurrentDocument);
				}
			}
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
			bool isEFv2Model = context.Project.IsEntityFrameworkV2Model();
			if (isEFv2Model)
			{
				var connectionString = new ConnectionStringLocator().Locate(context.Project);
				using (var docSource = new DatabaseDocumentationSource(connectionString.ToString()))
				{
					new DocumentationUpdater(docSource)
							.UpdateDocumentation(context.CurrentDocument);
				}
			}
		}
	}
}