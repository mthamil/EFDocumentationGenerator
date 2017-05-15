﻿//  Entity Designer Documentation Generator
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
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator.Utilities;
using EnvDTE;

namespace DocumentationGenerator
{
	/// <summary>
	/// Locates and extracts the connection string to use for model updates.
	/// </summary>
	[Export(typeof(IConnectionStringLocator))]
	public class ConnectionStringLocator : IConnectionStringLocator
	{
		/// <summary>
		/// Attempts to find a database connection string in the App.config file of a project.
		/// </summary>
		/// <param name="project">The project to search for a connection string</param>
		public SqlConnectionStringBuilder Locate(Project project)
		{
			// Try to find the app config file.
			var appConfigItem = project.TryFindChild(item => IgnoreCaseEquals(item.Name, ConfigName));
			if (appConfigItem == null)
				throw new ConnectionStringLocationException(String.Format("{0} file does not exist", ConfigName));

			// If the app.config file is unsaved, it probably had a connection string added to it. Save it.
			if (!appConfigItem.Saved)
				appConfigItem.Save();

			// Try to find the connection string.
			string entityConnString = TryGetConnectionString(appConfigItem);
			var connectionString = ParseInnerConnectionString(entityConnString);

			return new SqlConnectionStringBuilder(connectionString);
		}

		/// <summary>
		/// Searches a config file for an Entity Framework connection string.
		/// </summary>
		private string TryGetConnectionString(ProjectItem appConfigItem)
		{
			// Try to find the connection strings section.
			var appConfigFileName = appConfigItem.FileNames[0];
			var appConfig = XDocument.Load(appConfigFileName);
			var connectionStringsElement = appConfig
				.Elements(XName.Get("configuration")).Single()
				.Elements(XName.Get("connectionStrings")).ToList();

			if (!connectionStringsElement.Any())
				throw new ConnectionStringLocationException(String.Format("No valid connection strings found in {0} file", ConfigName));

			// Try to find the Entity Framework connection string.
			var entityConnElement = connectionStringsElement
				.Elements(XName.Get("add"))
				.FirstOrDefault(element =>
					element.Attribute("providerName") != null &&
					IgnoreCaseEquals(element.Attribute("providerName").Value, EntityProviderName));

			if (entityConnElement == null)
				throw new ConnectionStringLocationException(String.Format("Connection string for provider '{0}' not found.", EntityProviderName));

			// Parse the connection string.
			return entityConnElement.Attribute("connectionString").Value;
		}

		/// <summary>
		/// Parses an Entity Framework connection string to extract the inner database connection string.
		/// </summary>
		private string ParseInnerConnectionString(string entityConnString)
		{
			var innerConnStringStart = entityConnString.ToLower().IndexOf("data source=");
			var innerConnStringEnd = entityConnString.ToLower().LastIndexOf('"');
			var connectionString = entityConnString.Substring
				(innerConnStringStart,
				(entityConnString.Length - innerConnStringStart) - (entityConnString.Length - innerConnStringEnd)).Trim();

			return connectionString;
		}

		private static bool IgnoreCaseEquals(string first, string second)
		{
			return String.Equals(first, second, StringComparison.InvariantCultureIgnoreCase);
		}

		private const string ConfigName = "App.config";
		private const string EntityProviderName = "System.Data.EntityClient";
	}
}