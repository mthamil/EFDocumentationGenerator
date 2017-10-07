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
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator.Utilities;

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
            var conceptualModel = modelDocument.Edm();
            var conceptualTypeNamespace = conceptualModel.Descendants("Schema")
                                                         .Single()
                                                         .Attribute("Namespace").Value;

            // The conceptual model entity type that is being mapped is specified by the
            // TypeName attribute of the EntityTypeMapping element. The table or view 
            // that is being mapped is specified by the StoreEntitySet attribute of the 
            // child MappingFragment element.
            var mappings = modelDocument.Cs().Descendants("EntitySetMapping")
                                             .Select(es => es.Cs().Element("EntityTypeMapping"))
                                             .ToDictionary(et => et.Attribute("TypeName").Value, 
                                                           et => et.Cs().Element("MappingFragment")
                                                                        .Attribute("StoreEntitySet").Value);

            foreach (var entityType in conceptualModel.Descendants("EntityType").ToList())
            {
                var conceptualName = entityType.Attribute("Name").Value;

                if (!mappings.TryGetValue($"{conceptualTypeNamespace}.{conceptualName}", out var storageName))
                    continue;

                UpdateNodeDocumentation(entityType, conceptualModel.Namespace, 
                    _documentationSource.GetDocumentation(storageName));

                var properties =
                        entityType.Edm().Descendants("Property")
                                  .Select(e => new
                                  {
                                      Element = e,
                                      Property = new EntityProperty(
                                                   e.Attribute("Name").Value,
                                                   EntityPropertyType.Property)
                                  })
                    .Concat(
                        entityType.Edm().Descendants("NavigationProperty")
                                  .Select(e => new
                                  {
                                      Element = e,
                                      Property = CreateNavProperty(e, modelDocument)
                                  }));

                foreach (var property in properties)
                {
                    UpdateNodeDocumentation(property.Element, conceptualModel.Namespace,
                        _documentationSource.GetDocumentation(storageName, property.Property));
                }
            }
        }

        private static void UpdateNodeDocumentation(XContainer element, string edmNamespace, string documentation)
        {
            if (String.IsNullOrWhiteSpace(documentation))
                return;

            var fixedDocumentation = documentation.Trim();

            // Remove existing documentation.
            element.Edm().Descendants("Documentation").Remove();

            element.AddFirst(new XElement(XName.Get("Documentation", edmNamespace),
                                          new XElement(XName.Get("Summary", edmNamespace), fixedDocumentation)));
        }

        private static EntityProperty CreateNavProperty(XElement element, XContainer document)
        {
            var relationship = element.Attribute("Relationship").Value;
            var association = document.Edm()
                .Descendants("AssociationSet")
                .Single(ae => ae.Attribute("Association").Value == relationship);

            return new EntityProperty(
                association.Attribute("Name").Value,
                EntityPropertyType.NavigationProperty);
        }

        private readonly IDocumentationSource _documentationSource;
    }
}