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

            _docSource.Setup(s => s.GetDocumentation(It.IsAny<string>(), It.IsAny<EntityProperty>()))
                      .Returns((string entityName, EntityProperty property) =>
                          _documentation[(entityName, property?.Name)]);

            _underTest = new ModelDocumentationUpdater(_docSource.Object);
        }

        [Fact]
        public void Test_UpdateDocumentation()
        {
            // Arrange.
            _documentation = new Dictionary<(string, string), string>
            {
                { ("FirstTable", null),             "FirstTable_Summary" },
                { ("FirstTable", "FirstTableID"),   "FirstTableID_Summary" },
                { ("FirstTable", "DateTimeColumn"), "DateTimeColumn_Summary" },
                { ("SecondTables", null),            "SecondTable_Summary" },
                { ("SecondTables", "SecondTableID"), "SecondTableID_Summary" },
                { ("SecondTables", "StringColumn"),  "StringColumn_Summary" },
                { ("SecondTables", "IntegerColumn"), "IntegerColumn_Summary" },
                { ("SecondTables", "FK_FirstTable_Order_FirstTableID"), "ForeignKey_Summary" }
            };

            // Act.
            _underTest.UpdateDocumentation(_edmx.Document);

            var entities = _edmx.Document.Edm().Descendants("EntityType").ToList();

            // Assert.
            Assert.NotEmpty(entities);
            Assert.Equal(new[] { "FirstTable_Summary", "SecondTable_Summary" }, entities.Select(GetSummary));

            Assert.Equal(new[] { "FirstTableID_Summary", "DateTimeColumn_Summary" }, 
                entities.First().Edm().Elements("Property").Select(GetSummary));

            Assert.Equal(new[] { "SecondTableID_Summary", "StringColumn_Summary", "IntegerColumn_Summary" },
                entities.Last().Edm().Elements("Property").Select(GetSummary));

            Assert.Equal("ForeignKey_Summary", GetSummary(entities.Last().Edm().Element("NavigationProperty")));
        }

        private static string GetSummary(XElement element) => element.Edm().Element("Documentation")
                                                                     .Edm().Element("Summary").Value;

        private readonly ModelDocumentationUpdater _underTest;

        private IDictionary<(string, string), string> _documentation;
        private readonly Mock<IDocumentationSource> _docSource = new Mock<IDocumentationSource>();

        private readonly EdmxFixture _edmx;
    }
}