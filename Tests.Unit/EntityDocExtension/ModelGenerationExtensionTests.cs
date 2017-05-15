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
			project.SetupGet(p => p.Properties).Returns(projectProperties.Object);
			project.SetupGet(p => p.UniqueName).Returns("DataProject.csproj");

			var frameworkProperty = new Mock<Property>();
			frameworkProperty.SetupGet(p => p.Value).Returns(".NETFramework,Version=v4.5");

			projectProperties.Setup(p => p.Item("TargetFrameworkMoniker"))
							 .Returns(frameworkProperty.Object);

			connectionStringLocator.Setup(c => c.Locate(project.Object))
			                       .Returns(new SqlConnectionStringBuilder(@"data source=LOCALHOST\SQLEXPRESS;initial catalog=Test"));

			extension = new ModelGenerationExtension(
				Mock.Of<ILogger>(), 
				connectionStringLocator.Object, 
				_ => documentationSource.Object, 
				_ => modelUpdater.Object,
				errorList);
		}

		private readonly ModelGenerationExtension extension;

		[Fact]
		public void Test_Update_Only_Occurs_Once()
		{
			// Arrange.
			var generationContext = Mock.Of<ModelGenerationExtensionContext>(
				c => c.Project == project.Object &&
				     c.WizardKind == WizardKind.UpdateModel &&
				     c.CurrentDocument == document);

			var updateContext = Mock.Of<UpdateModelExtensionContext>(
				c => c.Project == project.Object &&
				     c.WizardKind == WizardKind.UpdateModel &&
				     c.CurrentDocument == document);

			// Act.
			extension.OnAfterModelGenerated(generationContext);
			extension.OnAfterModelUpdated(updateContext);

			// Assert.
			modelUpdater.Verify(u => u.UpdateDocumentation(document), Times.Once());
		}

		[Fact(Skip="This behavior may not be desirable.")]
		public void Test_Update_Prevented_By_Model_Errors()
		{
			// Arrange.
			var error = new Mock<ErrorItem> { DefaultValue = DefaultValue.Mock };
			error.SetupGet(e => e.Project).Returns(project.Object.UniqueName);
			error.SetupGet(e => e.Description).Returns("Error 1234: Testing");
			error.SetupGet(e => e.FileName).Returns("test.edmx");

			errorList.Add(error.Object);

			var generationContext = Mock.Of<ModelGenerationExtensionContext>(
				c => c.Project == project.Object &&
					 c.WizardKind == WizardKind.UpdateModel &&
					 c.CurrentDocument == document);

			var updateContext = Mock.Of<UpdateModelExtensionContext>(
				c => c.Project == project.Object &&
					 c.WizardKind == WizardKind.UpdateModel &&
					 c.CurrentDocument == document);

			// Act.
			extension.OnAfterModelGenerated(generationContext);
			extension.OnAfterModelUpdated(updateContext);

			// Assert.
			modelUpdater.Verify(u => u.UpdateDocumentation(document), Times.Never());
		}

		private readonly Mock<Project> project = new Mock<Project> { DefaultValue = DefaultValue.Mock };
		private readonly Mock<Properties> projectProperties = new Mock<Properties>();
		private readonly XDocument document = new XDocument();

		private readonly List<ErrorItem> errorList = new List<ErrorItem>();
		private readonly Mock<IModelDocumentationUpdater> modelUpdater = new Mock<IModelDocumentationUpdater>();
		private readonly Mock<IDocumentationSource> documentationSource = new Mock<IDocumentationSource>();
		private readonly Mock<IConnectionStringLocator> connectionStringLocator = new Mock<IConnectionStringLocator>();
	}
}