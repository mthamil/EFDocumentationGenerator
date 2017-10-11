using System.IO;
using System.Xml.Linq;

namespace Tests.Unit.EntityDocExtension
{
    public class EdmxFixture
    {
        public EdmxFixture()
        {
            using (var edmxStream = GetType().Assembly.GetManifestResourceStream("Tests.Unit.EntityDocExtension.TestModel.xml"))
            using (var reader = new StreamReader(edmxStream))
            {
                Document = XDocument.Load(reader);
            }
        }

        public XDocument Document { get; }
    }
}