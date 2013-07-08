using System;
using System.Linq;
using EnvDTE;

namespace DocumentationGenerator
{
	/// <summary>
	/// A logger that writes to the Visual Studio Output Window.
	/// </summary>
	internal class OutputWindowLogger : ILogger
	{
		/// <summary>
		/// Initializes a new <see cref="OutputWindowLogger"/>.
		/// </summary>
		/// <param name="application">The application containing the output window</param>
		/// <param name="paneName">The name of the output window pane to write to</param>
		public OutputWindowLogger(DTE application, string paneName)
		{
			var window = application.Windows.Item(EnvDTEConstants.vsWindowKindOutput);
			var outputWindow = (OutputWindow)window.Object;

			_pane = outputWindow.OutputWindowPanes
			                    .Cast<OutputWindowPane>()
			                    .FirstOrDefault(pane =>
			                                    pane.Name.Equals(paneName, StringComparison.CurrentCultureIgnoreCase)) 
					?? outputWindow.OutputWindowPanes.Add(paneName);

			_pane.Clear();
		}

		/// <see cref="ILogger.Log"/>
		public void Log(string message, params object[] arguments)
		{
			_pane.OutputString(String.Format(message + Environment.NewLine, arguments));
		}

		private readonly OutputWindowPane _pane;
	}
}