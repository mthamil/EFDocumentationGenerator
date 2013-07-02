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

using System;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator.Utilities;

namespace DocumentationGenerator
{
	/// <summary>
	/// Updates XML EDMX file documentation nodes.
	/// </summary>
	public class DocumentationUpdater
	{
		/// <summary>
		/// Initializes a new <see cref="DocumentationUpdater"/>.
		/// </summary>
		/// <param name="documentationSource">The documentation source</param>
		public DocumentationUpdater(IDocumentationSource documentationSource)
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
			var entityTypeElements = modelDocument.Edm().Descendants("EntityType").ToList();
			foreach (var entityType in entityTypeElements)
			{
				string tableName = entityType.Attribute("Name").Value;
				var propertyElements = entityType.Edm().Descendants("Property").ToList();

				UpdateNodeDocumentation(entityType, _documentationSource.GetDocumentation(tableName));

				foreach (var propertyElement in propertyElements)
				{
					string columnName = propertyElement.Attribute("Name").Value;
					UpdateNodeDocumentation(propertyElement, _documentationSource.GetDocumentation(tableName, columnName));
				}
			}
		}

		private void UpdateNodeDocumentation(XElement element, string documentation)
		{
			if (String.IsNullOrEmpty(documentation))
				return;

			// Remove existing documentation.
			element.Edm().Descendants("Documentation").Remove();

			element.AddFirst(new XElement(XName.Get("Documentation", Namespace),
			                              new XElement(XName.Get("Summary", Namespace), documentation)));
		}

		private readonly IDocumentationSource _documentationSource;

		private const string Namespace = "http://schemas.microsoft.com/ado/2009/11/edm";
	}
}