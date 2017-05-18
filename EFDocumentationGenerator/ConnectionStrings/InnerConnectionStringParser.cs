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

namespace DocumentationGenerator.ConnectionStrings
{
	/// <summary>
	/// Responsible for parsing out a database connection string from an Entity Framework connection string.
	/// </summary>
	public class InnerConnectionStringParser
	{
		/// <summary>
		/// Parses an inner database connection string from an Entity Framework connection string.
		/// </summary>
		/// <param name="entityConnectionString">The connection string to parse</param>
		/// <returns>An inner database connection string</returns>
		public string Parse(string entityConnectionString)
		{
			var innerConnStringStart = entityConnectionString.IndexOf("data source=", StringComparison.OrdinalIgnoreCase);
			var innerConnStringEnd = entityConnectionString.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase);
		    var connectionString = entityConnectionString.Substring(innerConnStringStart,
		                                                            (entityConnectionString.Length - innerConnStringStart) -
		                                                            (entityConnectionString.Length - innerConnStringEnd))
		                                                 .Trim();

			return connectionString;
		}
	}
}