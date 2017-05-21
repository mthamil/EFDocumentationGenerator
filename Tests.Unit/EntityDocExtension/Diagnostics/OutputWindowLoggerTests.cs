using DocumentationGenerator.Diagnostics;
using EnvDTE;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension.Diagnostics
{
	public class OutputWindowLoggerTests
	{
		public OutputWindowLoggerTests()
		{
			_underTest = new OutputWindowLogger(Mock.Of<IOutputPaneProvider>(p => p.Get() == _pane.Object));
		}

		[Fact]
		public void Test_Log()
		{
			// Act.
			_underTest.Log("Test message {0}", 1);

			_pane.Verify(p => p.OutputString("Test message 1\r\n"), Times.Once());
		}

		private readonly OutputWindowLogger _underTest;

		private readonly Mock<OutputWindowPane> _pane = new Mock<OutputWindowPane>(); 
	}
}