using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocumentationGenerator;
using DocumentationGenerator.Diagnostics;
using EnvDTE;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension.Diagnostics
{
	public class OutputPaneProviderTests
	{
		public OutputPaneProviderTests()
		{
			outputPanes.As<IEnumerable>().Setup(op => op.GetEnumerator())
					   .Returns(() => panes.GetEnumerator());

			outputPanes.Setup(op => op.Add(It.IsAny<string>()))
			           .Returns((string name) =>
			           {
						   var newPane = CreatePane(name);
						   panes.Add(newPane);
				           return newPane;
			           });

			var outputWindow = new Mock<OutputWindow>();
			outputWindow.SetupGet(w => w.OutputWindowPanes).Returns(outputPanes.Object);

			var window = new Mock<Window>();
			window.SetupGet(w => w.Object).Returns(outputWindow.Object);

			var windows = new Mock<Windows>();
			windows.Setup(ws => ws.Item(EnvDTEConstants.vsWindowKindOutput)).Returns(window.Object);

			var dte = new Mock<DTE>();
			dte.SetupGet(d => d.Windows).Returns(windows.Object);

			provider = new OutputPaneProvider(dte.Object);
		}

		[Fact]
		public void Test_Pane_Does_Not_Exist()
		{
			// Arrange.
			provider.PaneName = "TestPane";

			panes.Add(CreatePane("NotTheRightPane"));

			// Act.
			var pane = provider.Get();

			// Assert.
			Assert.NotNull(pane);
			Assert.Equal("TestPane", pane.Name);
			Assert.Equal(2, panes.Count);
		}

		[Fact]
		public void Test_Pane_Already_Exists()
		{
			// Arrange.
			provider.PaneName = "TestPane";

			var existingPanes = new[] { "NotTheRightPane", "TestPane" }
				.Select(CreatePane);

			foreach (var existingPane in existingPanes)
				panes.Add(existingPane);	

			// Act.
			var pane = provider.Get();

			// Assert.
			Assert.NotNull(pane);
			Assert.Equal("TestPane", pane.Name);
			Assert.Equal(2, panes.Count);
		}

		private OutputWindowPane CreatePane(string name)
		{
			var newPane = new Mock<OutputWindowPane>();
			newPane.SetupGet(wp => wp.Name).Returns(name);
			return newPane.Object;
		}

		private readonly OutputPaneProvider provider;

		private readonly ICollection<OutputWindowPane> panes = new List<OutputWindowPane>();
		private readonly Mock<OutputWindowPanes> outputPanes = new Mock<OutputWindowPanes>();
	}
}