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

using System.Data.SqlClient;
using EnvDTE;

namespace DocumentationGenerator.ConnectionStrings
{
	/// <summary>
	/// Locates and extracts the connection string to use for model updates.
	/// </summary>
	public interface IConnectionStringLocator
	{
		/// <summary>
		/// Attempts to find a database connection string in the App.config file of a project.
		/// </summary>
		/// <param name="project">The project to search for a connection string</param>
		SqlConnectionStringBuilder Locate(Project project);
	}
}