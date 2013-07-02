using System;
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
	public class DocumentationUpdaterTests
	{
		public DocumentationUpdaterTests()
		{
			docSource.Setup(s => s.GetDocumentation(It.IsAny<string>(), It.IsAny<string>()))
			         .Returns((string entityName, string propName) => 
						 documentation[Tuple.Create(entityName, propName)]);

			updater = new DocumentationUpdater(docSource.Object);
		}

		[Fact]
		public void Test_UpdateDocumentation()
		{
			// Arrange.
			documentation = new Dictionary<Tuple<string, string>, string>
			{
				{ Tuple.Create("FirstTable",  (string)null),     "FirstTable_Summary" },
				{ Tuple.Create("FirstTable",  "FirstTableID"),   "FirstTableID_Summary" },
				{ Tuple.Create("FirstTable",  "DateTimeColumn"), "DateTimeColumn_Summary" },
				{ Tuple.Create("SecondTable", (string)null),     "SecondTable_Summary" },
				{ Tuple.Create("SecondTable", "SecondTableID"),  "SecondTableID_Summary" },
				{ Tuple.Create("SecondTable", "StringColumn"),   "StringColumn_Summary" },
				{ Tuple.Create("SecondTable", "IntegerColumn"),  "IntegerColumn_Summary" }
			};

			var edmxDocument = XDocument.Load(new StringReader(edmx));

			// Act.
			updater.UpdateDocumentation(edmxDocument);

			var entities = edmxDocument.Edm().Descendants("EntityType").ToList();

			// Assert.
			Assert.NotEmpty(entities);
			Assert.Equal(new[] { "FirstTable_Summary", "SecondTable_Summary" }, entities.Select(GetSummary));

			Assert.Equal(new[] { "FirstTableID_Summary", "DateTimeColumn_Summary" }, 
				entities.First().Edm().Elements("Property").Select(GetSummary).ToArray());

			Assert.Equal(new[] { "SecondTableID_Summary", "StringColumn_Summary", "IntegerColumn_Summary" },
				entities.Last().Edm().Elements("Property").Select(GetSummary).ToArray());
		}

		private static string GetSummary(XElement element)
		{
			return element.Edm().Element("Documentation").Edm().Element("Summary").Value;
		}

		private readonly DocumentationUpdater updater;

		private IDictionary<Tuple<string, string>, string> documentation;

		private readonly Mock<IDocumentationSource> docSource = new Mock<IDocumentationSource>();

		private const string edmx =
@"<?xml version='1.0' encoding='utf-8'?>
<edmx:Edmx Version='3.0' xmlns:edmx='http://schemas.microsoft.com/ado/2009/11/edmx'>
  <edmx:Runtime>
	<edmx:ConceptualModels>
      <Schema Namespace='TestModel' xmlns:annotation='http://schemas.microsoft.com/ado/2009/02/edm/annotation' 
									  xmlns:p1='http://schemas.microsoft.com/ado/2009/02/edm/annotation' 
									  xmlns='http://schemas.microsoft.com/ado/2009/11/edm'>
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
		</EntityType>
	  </Schema>
	</edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
	}
}