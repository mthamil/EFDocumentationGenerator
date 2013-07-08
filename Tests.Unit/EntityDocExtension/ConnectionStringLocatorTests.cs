using System.Collections;
using System.Collections.Generic;
using DocumentationGenerator;
using EnvDTE;
using Moq;
using Tests.Unit.Support;
using Xunit;

namespace Tests.Unit.EntityDocExtension
{
	public class ConnectionStringLocatorTests
	{
		public ConnectionStringLocatorTests()
		{
			// Add a default test project item.
			var defaultProjectItem = new Mock<ProjectItem> { DefaultValue = DefaultValue.Mock };
			defaultProjectItem.SetupGet(pi => pi.ProjectItems)
			                  .Returns(new Mock<ProjectItems> {  DefaultValue = DefaultValue.Mock }.Object);

			projectItems.Add(defaultProjectItem.Object);

			mockProjectItems.Setup(pi => pi.GetEnumerator())
							.Returns(() => projectItems.GetEnumerator());

			mockProjectItems.As<IEnumerable>().Setup(pi => pi.GetEnumerator())
							.Returns(() => projectItems.GetEnumerator());

			mockProjectItems.SetupGet(pi => pi.Count)
			                .Returns(() => projectItems.Count);

			project.SetupGet(p => p.ProjectItems)
			       .Returns(mockProjectItems.Object);
		}

		[Fact]
		public void Test_Locate_FromProject()
		{
			using (var tempConfigFile = new TemporaryFile(validAppConfigXml))
			{
				// Arrange.
				var appConfigItem = new Mock<ProjectItem> { DefaultValue = DefaultValue.Mock };
				appConfigItem.SetupGet(pi => pi.ProjectItems)
							 .Returns(new Mock<ProjectItems> { DefaultValue = DefaultValue.Mock }.Object);

				appConfigItem.SetupGet(pi => pi.Name).Returns("App.config");
				appConfigItem.Setup(pi => pi.get_FileNames(0)).Returns(tempConfigFile.File.FullName);

				projectItems.Add(appConfigItem.Object);

				// Act.
				var connectionString = locator.Locate(project.Object);

				// Assert.
				Assert.Equal(@"LOCALHOST\SQLEXPRESS", connectionString.DataSource);
				Assert.Equal("Test", connectionString.InitialCatalog);
				Assert.True(connectionString.IntegratedSecurity);
				Assert.True(connectionString.MultipleActiveResultSets);
			}
		}

		[Fact]
		public void Test_Locate_No_Config_File()
		{
			// Act/Assert.
			Assert.Throws<ConnectionStringLocationException>(() => 
				locator.Locate(project.Object));
		}

		[Fact]
		public void Test_Locate_No_ConnectionString_In_Config_File()
		{
			using (var tempConfigFile = new TemporaryFile(
												@"<?xml version='1.0' encoding='utf-8'?>
												<configuration>
												</configuration>"))
			{
				// Arrange.
				var appConfigItem = new Mock<ProjectItem> { DefaultValue = DefaultValue.Mock };
				appConfigItem.SetupGet(pi => pi.ProjectItems)
				             .Returns(new Mock<ProjectItems>  {  DefaultValue = DefaultValue.Mock }.Object);

				appConfigItem.SetupGet(pi => pi.Name).Returns("App.config");
				appConfigItem.Setup(pi => pi.get_FileNames(0)).Returns(tempConfigFile.File.FullName);

				projectItems.Add(appConfigItem.Object);

				// Act/Assert.
				Assert.Throws<ConnectionStringLocationException>(() =>
					locator.Locate(project.Object));
			}
		}

		private readonly ConnectionStringLocator locator = new ConnectionStringLocator();

		private readonly Mock<Project> project = new Mock<Project> { DefaultValue = DefaultValue.Mock };

		private readonly Mock<ProjectItems> mockProjectItems = new Mock<ProjectItems>();
		private readonly IList<ProjectItem> projectItems = new List<ProjectItem>();

		private const string validAppConfigXml =
@"<?xml version='1.0' encoding='utf-8'?>
<configuration>
  <connectionStrings>
    <add name='TestEntities'
         connectionString='metadata=res://*/TestModel.csdl|res://*/TestModel.ssdl|res://*/TestModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=LOCALHOST\SQLEXPRESS;initial catalog=Test;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;'
         providerName='System.Data.EntityClient' />
  </connectionStrings>
</configuration>";
	}
}