using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentationGenerator;
using DocumentationGenerator.Utilities;
using Moq;
using Xunit;

namespace Tests.Unit.EntityDocExtension
{
    public class ModelDocumentationUpdaterTests
    {
        public ModelDocumentationUpdaterTests()
        {
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
                { ("SecondTable", null),            "SecondTable_Summary" },
                { ("SecondTable", "SecondTableID"), "SecondTableID_Summary" },
                { ("SecondTable", "StringColumn"),  "StringColumn_Summary" },
                { ("SecondTable", "IntegerColumn"), "IntegerColumn_Summary" },
                { ("SecondTable", "FK_FirstTable_Order_FirstTableID"), "ForeignKey_Summary" }
            };

            var edmxDocument = XDocument.Load(new StringReader(Edmx));

            // Act.
            _underTest.UpdateDocumentation(edmxDocument);

            var entities = edmxDocument.Edm().Descendants("EntityType").ToList();

            // Assert.
            Assert.NotEmpty(entities);
            Assert.Equal(new[] { "FirstTable_Summary", "SecondTable_Summary" }, entities.Select(GetSummary));

            Assert.Equal(new[] { "FirstTableID_Summary", "DateTimeColumn_Summary" }, 
                entities.First().Edm().Elements("Property").Select(GetSummary));

            Assert.Equal(new[] { "SecondTableID_Summary", "StringColumn_Summary", "IntegerColumn_Summary" },
                entities.Last().Edm().Elements("Property").Select(GetSummary));

            Assert.Equal("ForeignKey_Summary", GetSummary(entities.Last().Edm().Element("NavigationProperty")));
        }

        private static string GetSummary(XElement element)
        {
            return element.Edm().Element("Documentation").Edm().Element("Summary").Value;
        }

        private readonly ModelDocumentationUpdater _underTest;

        private IDictionary<(string, string), string> _documentation;
        private readonly Mock<IDocumentationSource> _docSource = new Mock<IDocumentationSource>();

        private const string Edmx =
@"<?xml version='1.0' encoding='utf-8'?>
<edmx:Edmx Version='3.0' xmlns:edmx='http://schemas.microsoft.com/ado/2009/11/edmx'>
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace='TestModel' xmlns:annotation='http://schemas.microsoft.com/ado/2009/02/edm/annotation' 
                                      xmlns:p1='http://schemas.microsoft.com/ado/2009/02/edm/annotation' 
                                      xmlns='http://schemas.microsoft.com/ado/2009/11/edm'>
        <AssociationSet Name='FK_FirstTable_Order_FirstTableID' Association='TestModel.FK_FirstTable_Order_FirstTableID'/>
        <EntityType Name='FirstTable'>
            <Key>
            <PropertyRef Name='FirstTableID' />
            </Key>
            <Property Name='FirstTableID' Type='Int32' Nullable='false' p1:StoreGeneratedPattern='Identity'/>
            <Property Name='DateTimeColumn' Type='DateTime' Nullable='false'/>
        </EntityType>
        <EntityType Name='SecondTable'>
            <Key>
            <PropertyRef Name='SecondTableID' />
            </Key>
            <Property Name='SecondTableID' Type='Int32' Nullable='false' p1:StoreGeneratedPattern='Identity'/>
            <Property Name='StringColumn' Type='String' Nullable='false'/>
            <Property Name='IntegerColumn' Type='Int32'/>
            <NavigationProperty Name='FirstTable' Relationship='TestModel.FK_FirstTable_Order_FirstTableID'/>
        </EntityType>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
    }
}