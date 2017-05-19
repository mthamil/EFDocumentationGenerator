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

        [Fact]
        public void Test_Locate_FromProject()
        {
            using (var tempConfigFile = new TemporaryFile(ValidConfigXml))
            {
                // Arrange.
                _project.SetupGet(pi => pi.Saved).Returns(true);

                var appConfigItem = CreateAppConfig(tempConfigFile.File);
                _projectItems.Add(appConfigItem.Object);

                // Act.
                var connectionString = _underTest.Locate(_project.Object);

                // Assert.
                Assert.Equal(@"LOCALHOST\SQLEXPRESS", connectionString.DataSource);
                Assert.Equal("Test", connectionString.InitialCatalog);
                Assert.True(connectionString.IntegratedSecurity);
                Assert.True(connectionString.MultipleActiveResultSets);
                appConfigItem.Verify(pi => pi.Save(It.IsAny<string>()), Times.Never());
                _project.Verify(p => p.Save(It.IsAny<string>()), Times.Never());
            }
        }

        [Fact]
        public void Test_Unsaved_AppConfig()
        {
            using (var tempConfigFile = new TemporaryFile(ValidConfigXml))
            {
                // Arrange.
                var appConfigItem = CreateAppConfig(tempConfigFile.File, true);

                _projectItems.Add(appConfigItem.Object);

                // Act.
                _underTest.Locate(_project.Object);

                // Assert.
                appConfigItem.Verify(pi => pi.Save(""), Times.Once());
            }
        }

        [Fact]
        public void Test_Locate_No_Config_File()
        {
            // Act/Assert.
            Assert.Throws<ConnectionStringLocationException>(() => 
                _underTest.Locate(_project.Object));
        }

        [Fact]
        public void Test_Locate_No_ConnectionStrings_In_Config_File()
        {
            using (var tempConfigFile = new TemporaryFile(
                                                @"<?xml version='1.0' encoding='utf-8'?>
                                                <configuration>
                                                </configuration>"))
            {
                // Arrange.
                _projectItems.Add(CreateAppConfig(tempConfigFile.File).Object);

                // Act/Assert.
                Assert.Throws<ConnectionStringLocationException>(() =>
                    _underTest.Locate(_project.Object));
            }
        }

        [Fact]
        public void Test_Locate_No_EntityFramework_ConnectionString_In_Config_File()
        {
            using (var tempConfigFile = new TemporaryFile(
                                                @"<?xml version='1.0' encoding='utf-8'?>
                                                <configuration>
                                                    <connectionStrings><add name='TestEntities' connectionString='conn_string'/></connectionStrings>
                                                </configuration>"))
            {
                // Arrange.
                _projectItems.Add(CreateAppConfig(tempConfigFile.File).Object);

                // Act/Assert.
                Assert.Throws<ConnectionStringLocationException>(() =>
                    _underTest.Locate(_project.Object));
            }
        }

        private static Mock<ProjectItem> CreateAppConfig(FileInfo configFile, bool hasChanges = false)
        {
            var appConfigItem = new Mock<ProjectItem> { DefaultValue = DefaultValue.Mock };
            appConfigItem.SetupGet(pi => pi.ProjectItems)
                         .Returns(new Mock<ProjectItems> { DefaultValue = DefaultValue.Mock }.Object);

            appConfigItem.SetupGet(pi => pi.Name).Returns("App.config");
            appConfigItem.Setup(pi => pi.get_FileNames(0)).Returns(configFile.FullName);
            appConfigItem.SetupGet(pi => pi.Saved).Returns(!hasChanges);
            return appConfigItem;
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