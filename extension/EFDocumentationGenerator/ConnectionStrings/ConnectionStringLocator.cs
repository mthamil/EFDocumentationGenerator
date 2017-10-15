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
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentationGenerator.Utilities;
using EnvDTE;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace DocumentationGenerator.ConnectionStrings
{
    /// <summary>
    /// Locates and extracts the connection string to use for model updates.
    /// </summary>
    [Export(typeof(IConnectionStringLocator))]
    public class ConnectionStringLocator : IConnectionStringLocator
    {
        /// <summary>
        /// Attempts to find a database connection string for a project.
        /// </summary>
        /// <param name="project">The project to search for a connection string</param>
        public SqlConnectionStringBuilder Locate(Project project)
        {
            // Try to find the app config file.
            var configItem = project.TryFindChild(item => !String.IsNullOrEmpty(item.Name) && ConfigNamePattern.IsMatch(item.Name));
            if (configItem == null)
                throw new ConnectionStringLocationException("Valid config file could not be found.");

            var configFile = new ExeConfigurationFileMap { ExeConfigFilename = configItem.FileNames[0] };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
            var connectionStringSettings = config.ConnectionStrings.ConnectionStrings
                                                 .Cast<ConnectionStringSettings>()
                                                 .Reverse()
                                                 .FirstOrDefault(cs => cs.ProviderName == EntityProviderName);

            if (connectionStringSettings == null)
                throw new ConnectionStringLocationException($"Connection string for provider '{EntityProviderName}' not found.");

            var entityConnString = new DbConnectionStringBuilder
            {
                ConnectionString = connectionStringSettings.ConnectionString
            };
            var connectionString = (string)entityConnString[ProviderConnectionStringKey];
            return new SqlConnectionStringBuilder(connectionString);
        }

        private static readonly Regex ConfigNamePattern = new Regex(@"(app|web)\.config", RegexOptions.IgnoreCase);
        private const string EntityProviderName = "System.Data.EntityClient";
        private const string ProviderConnectionStringKey = "provider connection string";
    }
}