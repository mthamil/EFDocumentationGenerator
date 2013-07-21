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
using System.Data;
using System.Data.SqlClient;

namespace DocumentationGenerator
{
	/// <summary>
	/// An entity documentation source that pulls documentation from a SQL Server database.
	/// </summary>
	public class DatabaseDocumentationSource : IDocumentationSource, IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="DatabaseDocumentationSource"/>.
		/// </summary>
		/// <param name="connectionString">The database connection string</param>
		public DatabaseDocumentationSource(string connectionString)
			: this(connectionString, cs => new SqlConnection(cs))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="DatabaseDocumentationSource"/>.
		/// </summary>
		/// <param name="connectionString">The database connection string</param>
		/// <param name="connectionFactory">Creates database connections from a connection string</param>
		public DatabaseDocumentationSource(string connectionString, Func<string, IDbConnection> connectionFactory)
		{
			_connection = connectionFactory(connectionString);
			_connection.Open();
		}

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			_connection.Dispose();
		}

		/// <see cref="IDocumentationSource.GetDocumentation"/>
		public string GetDocumentation(string entityName, string propertyName = null)
		{
			bool useColumn = propertyName != null;

			var query = String.Format(@"
						SELECT [MSDescription].[value] FROM [sys].[schemas]
						CROSS APPLY fn_listextendedproperty (
							'MS_Description', 
							'schema', [sys].[schemas].[name], 
							'table', @tableName,
							 {0}, {1}) as MSDescription
						WHERE [sys].[schemas].[name] <> 'sys' AND 
							  [sys].[schemas].[name] NOT LIKE 'db\_%' ESCAPE '\'",
					useColumn ? "'column'" : "null",
					useColumn ? "@columnName" : "null");

			using (var command = _connection.CreateCommand())
			{
				command.CommandText = query;
				command.Parameters.Add(new SqlParameter("tableName", entityName));

				if (useColumn)
					command.Parameters.Add(new SqlParameter("columnName", propertyName));

				return command.ExecuteScalar() as string;
			}
		}

		private readonly IDbConnection _connection;
	}
}