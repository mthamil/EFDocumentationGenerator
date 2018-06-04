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

using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using DocumentationGenerator.Utilities;

namespace DocumentationGenerator
{
    /// <summary>
    /// Represents an entity's storage model. It is based on an edmx storage entity mapping.
    /// </summary>
    public class EntityStorageModel
    {
        private readonly IReadOnlyDictionary<string, string> _propertyMappings;

        /// <summary>
        /// Initializes a new <see cref="EntityStorageModel"/>.
        /// </summary>
        /// <param name="element">An entity type mapping element.</param>
        public EntityStorageModel(XElement element)
        {
            var mappingFragment = element.Cs().Element("MappingFragment");
            EntityName = mappingFragment.Attribute("StoreEntitySet").Value;
            _propertyMappings = mappingFragment.Cs().Descendants("ScalarProperty")
                                                    .ToDictionary(p => p.Attribute("Name").Value,
                                                                  p => p.Attribute("ColumnName").Value);
        }

        /// <summary>
        /// An entity's storage name.
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Maps a property's conceptual name to its storage name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name] => _propertyMappings.TryGetValue(name, out var storageName) ? storageName : name;

    }
}