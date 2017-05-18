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
using EnvDTE;

namespace DocumentationGenerator.Diagnostics
{
	/// <summary>
	/// A logger that writes to a pane of the Visual Studio Output Window.
	/// </summary>
	[Export(typeof(ILogger))]
	public class OutputWindowLogger : ILogger
	{
		/// <summary>
		/// Initializes a new <see cref="OutputWindowLogger"/>.
		/// </summary>
		/// <param name="paneProvider">Retrieves the pane to log to</param>
		[ImportingConstructor]
		public OutputWindowLogger(IOutputPaneProvider paneProvider)
		{
			_pane = paneProvider.Get();
		}

		/// <see cref="ILogger.Log"/>
		public void Log(string message, params object[] arguments)
		{
			_pane.OutputString(String.Format(message + Environment.NewLine, arguments));
		}

		private readonly OutputWindowPane _pane;
	}
}