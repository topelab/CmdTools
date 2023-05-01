using System;
using System.Linq;
using System.Xml.Linq;

namespace UpdateVersion
{
    internal class ProjectUpdater : IProjectUpdater
    {
        public void Update(string file, string version, bool hasToIncrease)
        {
            XDocument document = XDocument.Load(file);

            var node = document.Descendants().Where(d => d.Name.LocalName == "Version").FirstOrDefault();
            if (node != null)
            {
                Console.WriteLine($"Actual version: {node.Value}");
            }
        }
    }
}
