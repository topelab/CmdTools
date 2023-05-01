using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace UpdateVersion
{
    internal class ProjectUpdater : IProjectUpdater
    {
        public void Update(string file, string version)
        {
            XDocument document = XDocument.Load(file);

            var node = document.Descendants().Where(d => d.Name.LocalName == "Version").FirstOrDefault();
            if (node != null)
            {
                node.Value = version;
            }
            else
            {
                var firstPropertyGroup = document.Descendants().Where(d => d.Name.LocalName == "PropertyGroup").First();
                var versionElement = new XElement("Version", version);
                firstPropertyGroup.Add(versionElement);
            }

            File.WriteAllText(file, document.ToString());
        }
    }
}
