using System.Xml.Linq;

namespace DocumentationGenerator
{
	/// <summary>
	/// Interface for an object that supplies an EDMX document's conceptual model with documentation.
	/// </summary>
	public interface IModelDocumentationUpdater
	{
		/// <summary>
		/// Iterates over the entities in the conceptual model and attempts to populate
		/// their documentation nodes with values from the database.
		/// Existing documentation will be removed and replaced by database content.
		/// </summary>
		/// <param name="modelDocument">An .edmx XML document to update</param>
		void UpdateDocumentation(XDocument modelDocument);
	}
}