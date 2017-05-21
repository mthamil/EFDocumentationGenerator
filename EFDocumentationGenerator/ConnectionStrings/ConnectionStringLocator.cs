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
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator.Utilities;
using EnvDTE;

namespace DocumentationGenerator.ConnectionStrings
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
            var configItem = project.TryFindChild(item => IgnoreCaseEquals(item.Name, ConfigName));
            if (configItem == null)
                throw new ConnectionStringLocationException($"{ConfigName} file does not exist.");

            // If the app.config file is unsaved, it probably had a connection string added to it. Save it.
            if (!configItem.Saved)
                configItem.Save();

            // Try to find the connection string.
            var entityConnString = TryGetConnectionString(configItem);
            var connectionString = _innerConnStringParser.Parse(entityConnString);

            return new SqlConnectionStringBuilder(connectionString);
        }

        /// <summary>
        /// Searches a config file for an Entity Framework connection string.
        /// </summary>
        private string TryGetConnectionString(ProjectItem configItem)
        {
            // Try to find the connection strings section.
            var configFileName = configItem.FileNames[0];
            var config = XDocument.Load(configFileName);
            var connectionStringsElement = config
                .Elements(XName.Get("configuration")).Single()
                .Elements(XName.Get("connectionStrings")).ToList();

            if (!connectionStringsElement.Any())
                throw new ConnectionStringLocationException($"No valid connection strings found in {ConfigName} file.");

            // Try to find the Entity Framework connection string.
            var entityConnElement = connectionStringsElement
                .Elements(XName.Get("add"))
                .FirstOrDefault(element =>
                    IgnoreCaseEquals(element.Attribute("providerName")?.Value, EntityProviderName));

            if (entityConnElement == null)
                throw new ConnectionStringLocationException($"Connection string for provider '{EntityProviderName}' not found.");

            // Parse the connection string.
            return entityConnElement.Attribute("connectionString").Value;
        }

        private static bool IgnoreCaseEquals(string first, string second) => 
            String.Equals(first, second, StringComparison.OrdinalIgnoreCase);

        private readonly InnerConnectionStringParser _innerConnStringParser = new InnerConnectionStringParser();

        private const string ConfigName = "App.config";
        private const string EntityProviderName = "System.Data.EntityClient";
    }
}