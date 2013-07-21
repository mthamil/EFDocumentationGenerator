using System.Data;
using DocumentationGenerator;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension
{
	public class DatabaseDocumentationSourceTests
	{
		public DatabaseDocumentationSourceTests()
		{
			command.SetupGet(c => c.Parameters)
				   .Returns(new Mock<IDataParameterCollection> { DefaultValue = DefaultValue.Mock }.Object);

			connection.Setup(c => c.CreateCommand())
					  .Returns(command.Object);

			source = new DatabaseDocumentationSource("connectionString", _ => connection.Object);
		}

		[Fact]
		public void Test_Connection_Disposed()
		{
			// Act.
			source.Dispose();

			// Assert.
			connection.Verify(c => c.Dispose(), Times.Once());
		}

		private readonly DatabaseDocumentationSource source;

		private readonly Mock<IDbCommand> command = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
		private readonly Mock<IDbConnection> connection = new Mock<IDbConnection> { DefaultValue = DefaultValue.Mock };
	}
}