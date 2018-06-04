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

using DocumentationGenerator.Utilities;
using System;
using System.Xml.Linq;

namespace DocumentationGenerator
{
    /// <summary>
    /// Represents an entity property.
    /// </summary>
    public class EntityProperty
    {
        private readonly INamespacedOperations _conceptualModel;
        private readonly XElement _element;

        /// <summary>
        /// Initializes a new <see cref="EntityProperty"/>.
        /// </summary>
        /// <param name="conceptualModel">The model a property belongs to.</param>
        /// <param name="element">The backing property element.</param>
        /// <param name="conceptualName">The property name in the conceptual model.</param>
        /// <param name="storageName">The property name in the storage model.</param>
        /// <param name="type">The property type.</param>
        public EntityProperty(INamespacedOperations conceptualModel, XElement element, string conceptualName, string storageName, EntityPropertyType type)
        {
            _conceptualModel = conceptualModel ?? throw new ArgumentNullException(nameof(conceptualModel));
            _element = element;
            ConceptualName = conceptualName;
            StorageName = storageName;
            Type = type;
        }

        /// <summary>
        /// The property name in the conceptual model.
        /// </summary>
        public string ConceptualName { get; }

        /// <summary>
        /// The property name in the storage model.
        /// </summary>
        public string StorageName { get; }

        /// <summary>
        /// The property type.
        /// </summary>
        public EntityPropertyType Type { get; }

        /// <summary>
        /// Updates the documentation for a property.
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