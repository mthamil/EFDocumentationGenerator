using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;
using DocumentationGenerator;
using DocumentationGenerator.ConnectionStrings;
using DocumentationGenerator.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Microsoft.Data.Entity.Design.Extensibility;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension
{
    public class ModelGenerationExtensionTests
    {
        public ModelGenerationExtensionTests()
        {
            _project.SetupGet(p => p.Properties).Returns(_projectProperties.Object);
            _project.SetupGet(p => p.UniqueName).Returns("DataProject.csproj");

            var frameworkProperty = Mock.Of<Property>(p => p.Value == ".NETFramework,Version=v4.5");
            _projectProperties.Setup(p => p.Item("TargetFrameworkMoniker"))
                              .Returns(frameworkProperty);

            _connectionStringLocator.Setup(c => c.Locate(_project.Object))
                                    .Returns(new SqlConnectionStringBuilder(@"data source=LOCALHOST\SQLEXPRESS;initial catalog=Test"));

            _underTest = new ModelGenerationExtension(
                Mock.Of<ILogger>(), 
                _connectionStringLocator.Object, 
                _ => _documentationSource.Object, 
                _ => _modelUpdater.Object,
                _errorList);
        }

        [Fact]
        public void Test_Update_Only_Occurs_Once()
        {
            // Arrange.
            var generationContext = Mock.Of<ModelGenerationExtensionContext>(
                c => c.Project == _project.Object &&
                     c.WizardKind == WizardKind.UpdateModel &&
                     c.CurrentDocument == _document);

            var updateContext = Mock.Of<UpdateModelExtensionContext>(
                c => c.Project == _project.Object &&
                     c.WizardKind == WizardKind.UpdateModel &&
                     c.CurrentDocument == _document);

            // Act.
            _underTest.OnAfterModelGenerated(generationContext);
            _underTest.OnAfterModelUpdated(updateContext);

            // Assert.
            _modelUpdater.Verify(u => u.UpdateDocumentation(_document), Times.Once());
        }

        [Fact(Skip="This behavior may not be desirable.")]
        public void Test_Update_Prevented_By_Model_Errors()
        {
            // Arrange.
            var error = new Mock<ErrorItem> { DefaultValue = DefaultValue.Mock };
            error.SetupGet(e => e.Project).Returns(_project.Object.UniqueName);
            error.SetupGet(e => e.Description).Returns("Error 1234: Testing");
            error.SetupGet(e => e.FileName).Returns("test.edmx");

            _errorList.Add(error.Object);

            var generationContext = Mock.Of<ModelGenerationExtensionContext>(
                c => c.Project == _project.Object &&
                     c.WizardKind == WizardKind.UpdateModel &&
                     c.CurrentDocument == _document);

            var updateContext = Mock.Of<UpdateModelExtensionContext>(
                c => c.Project == _project.Object &&
                     c.WizardKind == WizardKind.UpdateModel &&
                     c.CurrentDocument == _document);

            // Act.
            _underTest.OnAfterModelGenerated(generationContext);
            _underTest.OnAfterModelUpdated(updateContext);

            // Assert.
            _modelUpdater.Verify(u => u.UpdateDocumentation(_document), Times.Never());
        }

        private readonly ModelGenerationExtension _underTest;

        private readonly Mock<Project> _project = new Mock<Project> { DefaultValue = DefaultValue.Mock };
        private readonly Mock<Properties> _projectProperties = new Mock<Properties>();
        private readonly XDocument _document = new XDocument();

        private readonly List<ErrorItem> _errorList = new List<ErrorItem>();
        private readonly Mock<IModelDocumentationUpdater> _modelUpdater = new Mock<IModelDocumentationUpdater>();
        private readonly Mock<IDocumentationSource> _documentationSource = new Mock<IDocumentationSource>();
        private readonly Mock<IConnectionStringLocator> _connectionStringLocator = new Mock<IConnectionStringLocator>();
    }
}