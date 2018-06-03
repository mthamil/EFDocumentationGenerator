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

namespace DocumentationGenerator
{
    /// <summary>
    /// Represents a source of entity documentation.
    /// </summary>
    public interface IDocumentationSource : IDisposable
    {
        /// <summary>
        /// Retrieves documentation for an entity.
        /// </summary>
        /// <param name="entity">An entity.</param>
        /// <returns>A documentation string</returns>
        string GetDocumentation(EntityType entity);

        /// <summary>
        /// Retrieves documentation for an entity property.
        /// </summary>
        /// <param name="entity">The parent entity.</param>
        /// <param name="property">An entity property.</param>
        /// <returns>A documentation string.</returns>
        string GetDocumentation(EntityType entity, EntityProperty property);
    }
}