using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace UpdateVersion
{
    internal class ProjectUpdater : IProjectUpdater
    {
        private const string VersionNodeName = "Version";
        private const string PropertyGroupNodeName = "PropertyGroup";
        private const string UseMaui = "UseMaui";
        private const string MauiVersionNodeName = "ApplicationDisplayVersion";

        public void Update(string file, string version)
        {
            XDocument document = XDocument.Load(file);

            var versionNodeName = GetVersionNodeName(document);
            var node = document.Descendants().Where(d => d.Name.LocalName == versionNodeName).FirstOrDefault();
            if (node != null)
            {
                node.Value = version;
            }
            else
            {
                var firstPropertyGroup = document.Descendants().Where(d => d.Name.LocalName == PropertyGroupNodeName).First();
                var versionElement = new XElement(versionNodeName, version);
                firstPropertyGroup.Add(versionElement);
            }

            File.WriteAllText(file, document.ToString());
        }

        private string GetVersionNodeName(XDocument document)
        {
            var result = VersionNodeName;

            if (document.Descendants().Any(d => d.Name.LocalName == UseMaui && d != null && d.Value.Equals("true", System.StringComparison.CurrentCultureIgnoreCase)))
            {
                result = MauiVersionNodeName;
            }

            return result;
        }
    }
}
