//  Entity Designer Documentation Generator
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

namespace EntityDocExtension
{
	/// <summary>
	/// Represents a source of entity documentation.
	/// </summary>
	public interface IDocumentationSource
	{
		/// <summary>
		/// Retrieves documentation for an entity or entity property.
		/// </summary>
		/// <param name="entityName">An entity name</param>
		/// <param name="propertyName">An optional entity property name</param>
		/// <returns>A documentation string</returns>
		string GetDocumentation(string entityName, string propertyName = null);
	}
}