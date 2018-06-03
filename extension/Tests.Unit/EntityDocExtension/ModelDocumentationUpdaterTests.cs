using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator;
using DocumentationGenerator.Utilities;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension
{
    public class ModelDocumentationUpdaterTests : IClassFixture<EdmxFixture>
    {
        public ModelDocumentationUpdaterTests(EdmxFixture edmx)
        {
            _edmx = edmx;

            _docSource.Setup(s => s.GetDocumentation(It.IsAny<EntityType>()))
                      .Returns((EntityType entity) =>
                          _documentation.TryGetValue((entity.StorageName, null), out var doc) ? doc : null);

            _docSource.Setup(s => s.GetDocumentation(It.IsAny<EntityType>(), It.IsAny<EntityProperty>()))
                      .Returns((EntityType entity, EntityProperty property) =>
                          _documentation.TryGetValue((entity.StorageName, property.StorageName), out var doc) ? doc : null);

            _underTest = new ModelDocumentationUpdater(_docSource.Object);
        }

        [Fact]
        public void Test_UpdateDocumentation()
        {
            // Arrange.
            _documentation = new Dictionary<(string, string), string>
            {
                { ("Parent", null),                     "Parent_Summary" },
                { ("Parent", "Id"),                     "Parent_Id_Summary" },
                { ("Parent", "Name"),                   "Parent_Name_Summary" },
                { ("Children", null),                   "Children_Summary" },
                { ("Children", "ID"),                   "Children_Id_Summary" },
                { ("Children", "NAME"),                 "Children_Name_Summary" },
                { ("Children", "PARENT_ID"),             "Children_ParentId_Summary" },
                { ("Children", "FK_Children_ParentId"), "Children_Parent_ForeignKey_Summary" }
            };

            // Act.
            _underTest.UpdateDocumentation(_edmx.Document);

            var entities = _edmx.Document.Edm().Descendants("EntityType").ToList();

            // Assert.
            Assert.NotEmpty(entities);
            Assert.Equal(new[] { "Parent_Summary", "Children_Summary", null }, entities.Select(Summary));

            Assert.Equal(new[] { "Parent_Id_Summary", "Parent_Name_Summary" },
                Entity(entities, "Parent").Edm().Elements("Property").Select(Summary));

            Assert.Equal(new[] { "Children_Id_Summary", "Children_Name_Summary", "Children_ParentId_Summary" },
                Entity(entities, "Child").Edm().Elements("Property").Select(Summary));

            Assert.Equal("Children_Parent_ForeignKey_Summary", 
                Summary(Entity(entities, "Child").Edm().Elements("NavigationProperty").First()));
        }

        private static XElement Entity(IEnumerable<XElement> entities, string name) => entities.Single(e => e.Attribute("Name").Value == name);

        private static string Summary(XElement element) => element.Edm().Element("Documentation")?
                                                                  .Edm().Element("Summary")?.Value;

        private readonly ModelDocumentationUpdater _underTest;

        private IDictionary<(string, string), string> _documentation;
        private readonly Mock<IDocumentationSource> _docSource = new Mock<IDocumentationSource>();

        private readonly EdmxFixture _edmx;
    }
}