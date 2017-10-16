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
		    _outputPanes.As<IEnumerable>()
		                .Setup(op => op.GetEnumerator())
		                .Returns(() => _panes.GetEnumerator());

		    _outputPanes.Setup(op => op.Add(It.IsAny<string>()))
		                .Returns((string name) =>
		                {
		                    var newPane = CreatePane(name);
		                    _panes.Add(newPane);
		                    return newPane;
		                });

		    var outputWindow = Mock.Of<OutputWindow>(w => w.OutputWindowPanes == _outputPanes.Object);
		    var window = Mock.Of<Window>(w => w.Object == outputWindow);
		    var windows = Mock.Of<Windows>(ws => ws.Item(EnvDTEConstants.vsWindowKindOutput) == window);
		    var dte = Mock.Of<DTE>(d => d.Windows == windows);

			_underTest = new OutputPaneProvider(dte);
		}

		[Fact]
		public void Test_Pane_Does_Not_Exist()
		{
			// Arrange.
			_underTest.PaneName = "TestPane";

			_panes.Add(CreatePane("NotTheRightPane"));

			// Act.
			var pane = _underTest.Get();

			// Assert.
			Assert.NotNull(pane);
			Assert.Equal("TestPane", pane.Name);
			Assert.Equal(2, _panes.Count);
		}

		[Fact]
		public void Test_Pane_Already_Exists()
		{
			// Arrange.
			_underTest.PaneName = "TestPane";

			var existingPanes = new[] { "NotTheRightPane", "TestPane" }.Select(CreatePane);

			foreach (var existingPane in existingPanes)
				_panes.Add(existingPane);	

			// Act.
			var pane = _underTest.Get();

			// Assert.
			Assert.NotNull(pane);
			Assert.Equal("TestPane", pane.Name);
			Assert.Equal(2, _panes.Count);
		}

		private static OutputWindowPane CreatePane(string name) => Mock.Of<OutputWindowPane>(wp => wp.Name == name);

	    private readonly OutputPaneProvider _underTest;

		private readonly ICollection<OutputWindowPane> _panes = new List<OutputWindowPane>();
		private readonly Mock<OutputWindowPanes> _outputPanes = new Mock<OutputWindowPanes>();
	}
}