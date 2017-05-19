using DocumentationGenerator.ConnectionStrings;
using Xunit;

namespace Tests.Unit.EntityDocExtension.ConnectionStrings
{
	public class InnerConnectionStringParserTests
	{
		[Fact]
		public void Test_SqlServer_Connection_String()
		{
			// Arrange.
			const string entityConnString = @"metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=System.Data.SqlClient;provider connection string=""data source=LOCALHOST\SQLEXPRESS;initial catalog=Test;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework""";

			// Act.
			var innerConnString = _underTest.Parse(entityConnString);

			// Assert.
			Assert.Equal(@"data source=LOCALHOST\SQLEXPRESS;initial catalog=Test;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework", innerConnString);
		}

		[Fact]
		public void Test_LocalDB_Connection_String()
		{
			// Arrange.
			const string entityConnString = @"metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=System.Data.SqlClient;provider connection string=""data source=(LocalDB)\v11.0;attachdbfilename=|DataDirectory|\TestLocalDb.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework""";

			// Act.
			var innerConnString = _underTest.Parse(entityConnString);

			// Assert.
			Assert.Equal(@"data source=(LocalDB)\v11.0;attachdbfilename=|DataDirectory|\TestLocalDb.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework", innerConnString);
		}

		private readonly InnerConnectionStringParser _underTest = new InnerConnectionStringParser();
	}
}