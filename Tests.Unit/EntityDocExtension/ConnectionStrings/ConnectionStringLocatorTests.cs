using System.Collections;
using System.Collections.Generic;
using System.IO;
using DocumentationGenerator.ConnectionStrings;
using EnvDTE;
using Moq;
using Tests.Unit.Support;
using Xunit;

namespace Tests.Unit.EntityDocExtension.ConnectionStrings
{
    public class ConnectionStringLocatorTests
    {
        public ConnectionStringLocatorTests()
        {
            // Add a default test project item.
            var defaultProjectItem = new Mock<ProjectItem> { DefaultValue = DefaultValue.Mock };
            defaultProjectItem.SetupGet(pi => pi.ProjectItems)
                              .Returns(new Mock<ProjectItems> {  DefaultValue = DefaultValue.Mock }.Object);

            _projectItems.Add(defaultProjectItem.Object);

            _mockProjectItems.Setup(pi => pi.GetEnumerator())
                             .Returns(() => _projectItems.GetEnumerator());

            _mockProjectItems.As<IEnumerable>()
                             .Setup(pi => pi.GetEnumerator())
                             .Returns(() => _projectItems.GetEnumerator());

            _mockProjectItems.SetupGet(pi => pi.Count)
                             .Returns(() => _projectItems.Count);

            _project.SetupGet(p => p.ProjectItems)
                    .Returns(_mockProjectItems.Object);
        }

        [Theory]
        [InlineData("App.config")]
        [InlineData("Web.config")]
        public void Test_Locate_FromProject(string configName)
        {
            using (var tempConfigFile = new TemporaryFile(ValidConfigXml))
            {
                // Arrange.
                _project.SetupGet(pi => pi.Saved).Returns(true);

                var configItem = CreateAppConfig(tempConfigFile.File, configName);
                _projectItems.Add(configItem.Object);

                // Act.
                var connectionString = _underTest.Locate(_project.Object);

                // Assert.
                Assert.Equal(@"LOCALHOST\SQLEXPRESS", connectionString.DataSource);
                Assert.Equal("Test", connectionString.InitialCatalog);
                Assert.True(connectionString.IntegratedSecurity);
                Assert.True(connectionString.MultipleActiveResultSets);
            }
        }

        [Theory]
        [InlineData("App.config")]
        [InlineData("Web.config")]
        public void Test_Locate_When_Multiple_ConnectionStrings(string configName)
        {
            using (var tempConfigFile = new TemporaryFile(
                @"<?xml version='1.0' encoding='utf-8'?>
                  <configuration>
                      <connectionStrings>
                      <add name='TestEntities1'
                              connectionString='metadata=res://*/TestModel.csdl|res://*/TestModel.ssdl|res://*/TestModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=LOCALHOST\SQLEXPRESS;initial catalog=Test1;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;'
                              providerName='System.Data.EntityClient' />
                      <add name='TestEntities2'
                              connectionString='metadata=res://*/TestModel.csdl|res://*/TestModel.ssdl|res://*/TestModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=LOCALHOST\SQLEXPRESS;initial catalog=Test2;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;'
                              providerName='System.Data.EntityClient' />
                      </connectionStrings>
                  </configuration>"))
            {
                // Arrange.
                _project.SetupGet(pi => pi.Saved).Returns(true);

                var configItem = CreateAppConfig(tempConfigFile.File, configName);
                _projectItems.Add(configItem.Object);

                // Act.
                var connectionString = _underTest.Locate(_project.Object);

                // Assert.
                Assert.Equal(@"LOCALHOST\SQLEXPRESS", connectionString.DataSource);
                Assert.Equal("Test2", connectionString.InitialCatalog);
                Assert.True(connectionString.IntegratedSecurity);
                Assert.True(connectionString.MultipleActiveResultSets);
            }
        }

        [Theory]
        [InlineData("App.config")]
        [InlineData("Web.config")]
        public void Test_Unsaved_Config(string configName)
        {
            using (var tempConfigFile = new TemporaryFile(ValidConfigXml))
            {
                // Arrange.
                var configItem = CreateAppConfig(tempConfigFile.File, configName, true);

                _projectItems.Add(configItem.Object);

                configItem.SetupGet(ci => ci.Document)
                          .Returns(Mock.Of<Document>(d => d.Object(It.IsAny<string>()) ==
                                       Mock.Of<TextDocument>(td => td.EndPoint == Mock.Of<TextPoint>() &&
                                                                   td.StartPoint == 
                                           Mock.Of<TextPoint>(tp => tp.CreateEditPoint() ==
                                               Mock.Of<EditPoint>(ep => ep.GetText(td.EndPoint) == ValidConfigXml)))));

                // Act.
                var connectionString = _underTest.Locate(_project.Object);

                // Assert.
                Assert.Equal(@"LOCALHOST\SQLEXPRESS", connectionString.DataSource);
                Assert.Equal("Test", connectionString.InitialCatalog);
            }
        }

        [Fact]
        public void Test_Locate_No_Config_File()
        {
            // Act/Assert.
            Assert.Throws<ConnectionStringLocationException>(() => 
                _underTest.Locate(_project.Object));
        }

        [Theory]
        [InlineData("App.config")]
        [InlineData("Web.config")]
        public void Test_Locate_No_ConnectionStrings_In_Config_File(string configName)
        {
            using (var tempConfigFile = new TemporaryFile(
                                                @"<?xml version='1.0' encoding='utf-8'?>
                                                <configuration>
                                                </configuration>"))
            {
                // Arrange.
                _projectItems.Add(CreateAppConfig(tempConfigFile.File, configName).Object);

                // Act/Assert.
                Assert.Throws<ConnectionStringLocationException>(() =>
                    _underTest.Locate(_project.Object));
            }
        }

        [Theory]
        [InlineData("App.config")]
        [InlineData("Web.config")]
        public void Test_Locate_No_EntityFramework_ConnectionString_In_Config_File(string configName)
        {
            using (var tempConfigFile = new TemporaryFile(
                                                @"<?xml version='1.0' encoding='utf-8'?>
                                                <configuration>
                                                    <connectionStrings><add name='TestEntities' connectionString='conn_string'/></connectionStrings>
                                                </configuration>"))
            {
                // Arrange.
                _projectItems.Add(CreateAppConfig(tempConfigFile.File, configName).Object);

                // Act/Assert.
                Assert.Throws<ConnectionStringLocationException>(() =>
                    _underTest.Locate(_project.Object));
            }
        }

        private static Mock<ProjectItem> CreateAppConfig(FileInfo configFile, string filename, bool hasChanges = false)
        {
            var configItem = new Mock<ProjectItem> { DefaultValue = DefaultValue.Mock };
            configItem.SetupGet(pi => pi.ProjectItems)
                         .Returns(new Mock<ProjectItems> { DefaultValue = DefaultValue.Mock }.Object);

            configItem.SetupGet(pi => pi.Name).Returns(filename);
            configItem.Setup(pi => pi.get_FileNames(0)).Returns(configFile.FullName);
            configItem.SetupGet(pi => pi.Saved).Returns(!hasChanges);
            configItem.SetupGet(pi => pi.get_IsOpen(It.IsAny<string>())).Returns(hasChanges);
            return configItem;
        }

        private readonly ConnectionStringLocator _underTest = new ConnectionStringLocator();

        private readonly Mock<Project> _project = new Mock<Project> { DefaultValue = DefaultValue.Mock };

        private readonly Mock<ProjectItems> _mockProjectItems = new Mock<ProjectItems>();
        private readonly IList<ProjectItem> _projectItems = new List<ProjectItem>();

        private const string ValidConfigXml =
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