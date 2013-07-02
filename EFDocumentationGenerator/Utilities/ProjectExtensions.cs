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

using System.Runtime.Versioning;
using EnvDTE;

namespace EntityDocExtension.Utilities
{
	/// <summary>
	/// Contains extension methods for <see cref="Project"/>s.
	/// </summary>
	public static class ProjectExtensions
	{
		/// <summary>
		/// Returns true if the specified project targets .NET Framework 4.0; otherwise returns false.
		/// .edmx files in projects that target .NET Framework 4.0 are EF v2 models. Projects that target 
		/// .NET Framework 3.5 SP1 are EF v1 models.
		/// </summary>
		public static bool IsEntityFrameworkV2Model(this Project project)
		{
			bool isEFv2Model = false;
			try
			{
				var targetFrameworkMoniker = project.Properties.Item("TargetFrameworkMoniker");
				var frameworkName = new FrameworkName(targetFrameworkMoniker.Value.ToString());
				isEFv2Model = (frameworkName.Version.Major == 4);
			}
			catch
			{
				// Nothing to do.
			}
			return isEFv2Model;
		}
	}
}