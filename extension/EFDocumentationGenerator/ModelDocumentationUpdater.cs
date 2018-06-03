//  Entity Designer Documentation Generator
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

using System.Xml.Linq;

namespace DocumentationGenerator
{
    /// <summary>
    /// Updates XML EDMX file documentation nodes.
    /// </summary>
    internal class ModelDocumentationUpdater : IModelDocumentationUpdater
    {
        /// <summary>
        /// Initializes a new <see cref="ModelDocumentationUpdater"/>.
        /// </summary>
        /// <param name="documentationSource">The documentation source</param>
        public ModelDocumentationUpdater(IDocumentationSource documentationSource)
        {
            _documentationSource = documentationSource;
        }

        /// <summary>
        /// Iterates over the entities in the conceptual model and attempts to populate
        /// their documentation nodes with values from the database.
        /// Existing documentation will be removed and replaced by database content.
        /// </summary>
        /// <param name="modelDocument">An .edmx XML document to update</param>
        public void UpdateDocumentation(XDocument modelDocument)
        {
            var dataModel = new EntityDataModel(modelDocument.Root);
            foreach (var entity in dataModel.Entities)
            {
                var entityDocumentation = _documentationSource.GetDocumentation(entity);
                entity.UpdateDocumentation(entityDocumentation);

                foreach (var property in entity.Properties)
                {
                    var propertyDocumentation = _documentationSource.GetDocumentation(entity, property);
                    property.UpdateDocumentation(propertyDocumentation);
                }
            }
        }

        private readonly IDocumentationSource _documentationSource;
    }
}