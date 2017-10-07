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
    /// Represents the contents of an EDMX file. It is essentially a wrapper
    /// around an EDMX file.
    /// </summary>
    public class EntityDataModel
    {
        private readonly INamespacedOperations _conceptualModel;
        private readonly string _conceptualTypeNamespace;
        private readonly IDictionary<string, string> _mappings;

        /// <summary>
        /// Initializes a new <see cref="EntityDataModel"/>.
        /// </summary>
        /// <param name="edmxDocument">The edmx file containing the model.</param>
        public EntityDataModel(XDocument edmxDocument)
        {
            _conceptualModel = edmxDocument?.Edm() ?? throw new ArgumentNullException(nameof(edmxDocument));

            _conceptualTypeNamespace = _conceptualModel.Descendants("Schema")
                                                       .Single()
                                                       .Attribute("Namespace").Value;

            // The conceptual model entity type that is being mapped is specified by the
            // TypeName attribute of the EntityTypeMapping element. The table or view 
            // that is being mapped is specified by the StoreEntitySet attribute of the 
            // child MappingFragment element.
            _mappings = edmxDocument.Cs().Descendants("EntitySetMapping")
                                         .Select(es => es.Cs().Element("EntityTypeMapping"))
                                         .ToDictionary(et => et.Attribute("TypeName").Value,
                                                       et => et.Cs().Element("MappingFragment")
                                                                    .Attribute("StoreEntitySet").Value);
        }

        /// <summary>
        /// The entities in the conceptual data model.
        /// </summary>
        public IEnumerable<EntityType> Entities => _conceptualModel.Descendants("EntityType")
                                                                   .Select(CreateEntity)
                                                                   .Where(e => e != null);

        private EntityType CreateEntity(XElement element)
        {
            var conceptualName = $"{_conceptualTypeNamespace}.{element.Attribute("Name").Value}";
            if (!_mappings.TryGetValue(conceptualName, out var storageName))
                return null;

            return new EntityType(_conceptualModel,
                                  element,
                                  conceptualName,
                                  storageName);
        }
    }
}