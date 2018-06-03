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
using System.Data;
using System.Data.SqlClient;

namespace DocumentationGenerator
{
    /// <summary>
    /// An entity documentation source that pulls documentation from a SQL Server database.
    /// </summary>
    internal class DatabaseDocumentationSource : IDocumentationSource
    {
        /// <summary>
        /// Initializes a new <see cref="DatabaseDocumentationSource"/>.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        public DatabaseDocumentationSource(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _connection.Open();
        }

        /// <see cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _connection.Dispose();
        }

        /// <summary>
        /// <see cref="IDocumentationSource.GetDocumentation(EntityType)"/>
        /// </summary>
        public string GetDocumentation(EntityType entity) => GetDocumentation(entity.StorageName, null);

        /// <summary>
        /// <see cref="IDocumentationSource.GetDocumentation(EntityType, EntityProperty)"/>
        /// </summary>
        public string GetDocumentation(EntityType entity, EntityProperty property) => GetDocumentation(entity.StorageName, property);

        private string GetDocumentation(string entityName, EntityProperty property)
        {
            bool useSecondLevel = property != null;

            var query = String.Format(@"
                        SELECT [MSDescription].[value] FROM [sys].[schemas]
                        CROSS APPLY fn_listextendedproperty (
                            'MS_Description', 
                            'schema', [sys].[schemas].[name], 
                            'table', @tableName,
                             {0}, {1}) as MSDescription
                        WHERE [sys].[schemas].[name] <> 'sys' AND 
                              [sys].[schemas].[name] NOT LIKE 'db\_%' ESCAPE '\'",
                    useSecondLevel ? GetSecondLevelType(property.Type) : "null",
                    useSecondLevel ? "@secondLevelName" : "null");

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("tableName", entityName));

                if (useSecondLevel)
                    command.Parameters.Add(new SqlParameter("secondLevelName", property.StorageName));

                return command.ExecuteScalar() as string;
            }
        }

        private static string GetSecondLevelType(EntityPropertyType propertyType) =>
            propertyType == EntityPropertyType.Property ? "'column'" : "'constraint'";

        private readonly IDbConnection _connection;
    }
}