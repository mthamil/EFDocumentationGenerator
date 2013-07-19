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
using System.ComponentModel.Composition;
using System.Linq;
using DocumentationGenerator.Utilities;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace DocumentationGenerator.Diagnostics
{
	/// <summary>
	/// Provides the <see cref="OutputWindowPane"/> used for documentation generation.
	/// </summary>
	[Export(typeof(IOutputPaneProvider))]
	public class DocGeneratorOutputPaneProvider : IOutputPaneProvider
	{
		/// <summary>
		/// Initializes a new <see cref="DocGeneratorOutputPaneProvider"/>.
		/// </summary>
		[ImportingConstructor]
		public DocGeneratorOutputPaneProvider()
			: this(ServiceProvider.GlobalProvider.GetService<DTE>())	// This is done instead of using: [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider
																		// because of the following bug: http://entityframework.codeplex.com/workitem/672
		{
		}

		/// <summary>
		/// Initializes a new <see cref="DocGeneratorOutputPaneProvider"/>.
		/// </summary>
		/// <param name="application">The applicaiton object</param>
		public DocGeneratorOutputPaneProvider(DTE application)
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
			                                             pane.Name.Equals(EntityDesignerPaneName, StringComparison.CurrentCultureIgnoreCase))
			                 ?? outputWindow.OutputWindowPanes.Add(EntityDesignerPaneName);

			return outputPane;
		}

		private readonly DTE _application;

		private const string EntityDesignerPaneName = "Entity Documentation Generator";
	}
}