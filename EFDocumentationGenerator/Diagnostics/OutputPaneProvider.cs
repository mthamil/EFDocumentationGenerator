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
using EnvDTE;

namespace DocumentationGenerator.Diagnostics
{
	/// <summary>
	/// Class that provides <see cref="OutputWindowPane"/>s.
	/// </summary>
	public class OutputPaneProvider : IOutputPaneProvider
	{
		/// <summary>
		/// Initializes a new <see cref="OutputPaneProvider"/>.
		/// </summary>
		/// <param name="application">The application object</param>
		public OutputPaneProvider(DTE application)
		{
			_application = application;
		}

		/// <see cref="IOutputPaneProvider.Get"/>
		public OutputWindowPane Get()
		{
			var window = _application.Windows.Item(EnvDTEConstants.vsWindowKindOutput);
			var outputWindow = (OutputWindow)window.Object;

			var outputPane = outputWindow.OutputWindowPanes
			                             .Cast<OutputWindowPane>()
			                             .FirstOrDefault(pane =>
			                                             pane.Name.Equals(PaneName, StringComparison.CurrentCultureIgnoreCase))
			                 ?? outputWindow.OutputWindowPanes.Add(PaneName);

			return outputPane;
		}

		/// <summary>
		/// The name of the output pane to provide.
		/// </summary>
		public string PaneName { get; set; }

		private readonly DTE _application;
	}
}