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
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using EnvDTE;

namespace DocumentationGenerator
{
	/// <summary>
	/// Locates and extracts the connection string to use for model updates.
	/// </summary>
	public class ConnectionStringLocator
	{
		/// <summary>
		/// Attempts to find a database connection string in the App.config file of a project.
		/// </summary>
		/// <param name="project">The project to search for a connection string</param>
		public SqlConnectionStringBuilder Locate(Project project)
		{
			var appConfigItem = project.ProjectItems
				.Cast<ProjectItem>()
				.Where(item => item.ProjectItems == null || item.ProjectItems.Count == 0)
				.SingleOrDefault(item => string.Equals(item.Name, ConfigName, StringComparison.InvariantCultureIgnoreCase));

			if (appConfigItem == null)
				throw new ConnectionStringLocationException(String.Format("'{0}' file does not exist.", ConfigName));

			var appConfigFileName = appConfigItem.FileNames[0];
			var appConfig = XDocument.Load(appConfigFileName);
			var connectionStringsElement = appConfig
				.Elements(XName.Get("configuration")).Single()
				.Elements(XName.Get("connectionStrings"));

			if (!connectionStringsElement.Any())
				throw new ConnectionStringLocationException(String.Format("No connection strings found in '{0}' file.", ConfigName));

			var entityConnString = connectionStringsElement
				.Elements(XName.Get("add"))
				.First(element => element.Attribute("providerName").Value == "System.Data.EntityClient")
				.Attribute("connectionString").Value;

			var innerConnStringStart = entityConnString.ToLower().IndexOf("data source=");
			var innerConnStringEnd = entityConnString.ToLower().LastIndexOf('"');
			var connectionString = entityConnString.Substring
				(innerConnStringStart, 
				(entityConnString.Length - innerConnStringStart) - (entityConnString.Length - innerConnStringEnd)).Trim();

			return new SqlConnectionStringBuilder(connectionString);
		}

		private const string ConfigName = "App.config";
	}
}