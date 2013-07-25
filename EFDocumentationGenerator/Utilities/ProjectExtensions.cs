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
using System.Runtime.Versioning;
using EnvDTE;

namespace DocumentationGenerator.Utilities
{
	/// <summary>
	/// Contains extension methods for <see cref="Project"/>s.
	/// </summary>
	internal static class ProjectExtensions
	{
		/// <summary>
		/// Returns true if the specified project targets .NET Framework 4.0; otherwise returns false.
		/// .edmx files in projects that target .NET Framework 4.0 are EF v2 models. Projects that target 
		/// .NET Framework 3.5 SP1 are EF v1 models.
		/// </summary>
		public static bool IsEntityFrameworkV2Model(this Project project)
		{
			try
			{
				return project.Framework().Version.Major == 4;
			}
			catch
			{
				// Nothing to do.
				return false;
			}
		}

		/// <summary>
		/// Returns the .NET framework version a project targets.
		/// </summary>
		public static FrameworkName Framework(this Project project)
		{
			var targetFrameworkMoniker = project.Properties.Item("TargetFrameworkMoniker");
			var frameworkName = new FrameworkName(targetFrameworkMoniker.Value.ToString());
			return frameworkName;
		}

		/// <summary>
		/// Attempts to find an immediate child <see cref="ProjectItem"/> of a <see cref="Project"/>
		/// that matches the given condition.
		/// </summary>
		/// <param name="project">The parent project to search</param>
		/// <param name="predicate">The condition that must be matched</param>
		/// <returns>The matching item or null</returns>
		public static ProjectItem TryFindChild(this Project project, Predicate<ProjectItem> predicate)
		{
			return project.ProjectItems
			              .Cast<ProjectItem>()
			              .Where(item => item.ProjectItems == null || item.ProjectItems.Count == 0)
			              .SingleOrDefault(item => predicate(item));
		}
	}
}