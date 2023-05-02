using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace UpdateVersion
{
    internal class ProjectUpdater : IProjectUpdater
    {
        private const string VersionNodeName = "Version";
        private const string PropertyGroupNodeName = "PropertyGroup";

        public void Update(string file, string version)
        {
            XDocument document = XDocument.Load(file);

            var node = document.Descendants().Where(d => d.Name.LocalName == VersionNodeName).FirstOrDefault();
            if (node != null)
            {
                node.Value = version;
            }
            else
            {
                var firstPropertyGroup = document.Descendants().Where(d => d.Name.LocalName == PropertyGroupNodeName).First();
                var versionElement = new XElement(VersionNodeName, version);
                firstPropertyGroup.Add(versionElement);
            }

            File.WriteAllText(file, document.ToString());
        }
    }
}
