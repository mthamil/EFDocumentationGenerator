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
	/// Interface for an object that supplies an EDMX document's conceptual model with documentation.
	/// </summary>
	public interface IModelDocumentationUpdater
	{
		/// <summary>
		/// Iterates over the entities in the conceptual model and attempts to populate
		/// their documentation nodes with values from the database.
		/// Existing documentation will be removed and replaced by database content.
		/// </summary>
		/// <param name="modelDocument">An .edmx XML document to update</param>
		void UpdateDocumentation(XDocument modelDocument);
	}
}