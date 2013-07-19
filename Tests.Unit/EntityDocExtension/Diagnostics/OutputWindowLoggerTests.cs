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
			logger = new OutputWindowLogger(Mock.Of<IOutputPaneProvider>(p => p.Get() == pane.Object));
		}

		[Fact]
		public void Test_Log()
		{
			// Act.
			logger.Log("Test message {0}", 1);

			pane.Verify(p => p.OutputString("Test message 1\r\n"), Times.Once());
		}

		private readonly OutputWindowLogger logger;

		private readonly Mock<OutputWindowPane> pane = new Mock<OutputWindowPane>(); 
	}
}