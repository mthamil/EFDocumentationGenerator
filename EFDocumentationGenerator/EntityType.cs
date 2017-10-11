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

using System;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator.Utilities;
using System.Collections.Generic;

namespace DocumentationGenerator
{
    /// <summary>
    /// Represents an entity type.
    /// </summary>
    public class EntityType
    {
        private readonly INamespacedOperations _conceptualModel;
        private readonly XElement _element;

        /// <summary>
        /// Initialized a new <see cref="EntityType"/>.
        /// </summary>
        /// <param name="conceptualModel">The conceptual model the entity belongs to.</param>
        /// <param name="element">The backing entity type element.</param>
        /// <param name="conceptualName">The name of an entity in the conceptual model.</param>
        /// <param name="storageName">The name of an entity in the storage model.</param>
        public EntityType(INamespacedOperations conceptualModel, XElement element, string conceptualName, string storageName)
        {
            _conceptualModel = conceptualModel ?? throw new ArgumentNullException(nameof(conceptualModel));
            _element = element ?? throw new ArgumentNullException(nameof(element));

            ConceptualName = conceptualName ?? throw new ArgumentNullException(nameof(conceptualName));
            StorageName = storageName ?? throw new ArgumentNullException(nameof(storageName));
        }

        /// <summary>
        /// The properties of an entity.
        /// </summary>
        public IEnumerable<EntityProperty> Properties => _element.Edm().Descendants("Property")
                                                                       .Select(CreateProperty)
                                                                       .Concat(
                                                         _element.Edm().Descendants("NavigationProperty")
                                                                       .Select(CreateNavProperty));
        /// <summary>
        /// The name of an entity in the conceptual model.
        /// </summary>
        public string ConceptualName { get; }

        /// <summary>
        /// The name of an entity in the storage model.
        /// </summary>
        public string StorageName { get; }

        private EntityProperty CreateProperty(XElement element)
        {
            return new EntityProperty(_conceptualModel,
                                      element,
                                      element.Attribute("Name").Value,
                                      EntityPropertyType.Property);
        }

        private EntityProperty CreateNavProperty(XElement element)
        {
            var relationship = element.Attribute("Relationship").Value;
            var association = _conceptualModel
                .Descendants("AssociationSet")
                .Single(ae => ae.Attribute("Association").Value == relationship);

            return new EntityProperty(_conceptualModel,
                                      element,
                                      association.Attribute("Name").Value,
                                      EntityPropertyType.NavigationProperty);
        }

        /// <summary>
        /// Updates the documentation for an entity.
        /// </summary>
        /// <param name="documentation"></param>
        public void UpdateDocumentation(string documentation)
        {
            if (documentation == null)
                return;

            var fixedDocumentation = documentation.Trim();

            // Remove existing documentation.
            _element.Edm().Descendants("Documentation").Remove();

            _element.AddFirst(new XElement(XName.Get("Documentation", _conceptualModel.Namespace),
                                           new XElement(XName.Get("Summary", _conceptualModel.Namespace), fixedDocumentation)));
        }
    }
}