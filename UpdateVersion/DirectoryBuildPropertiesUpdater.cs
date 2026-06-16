namespace UpdateVersion
{
    using System.Xml.Linq;

    internal class DirectoryBuildPropertiesUpdater : IProjectUpdater
    {
        public void Update<T>(string file, T context) where T : class
        {
            if (context is not Dictionary<string, string> versionsMap)
            {
                throw new ArgumentException("Invalid context type", nameof(context));
            }

            if (versionsMap.Count != 0)
            {
                XDocument document = XDocument.Load(file);
                document
                    .Descendants()
                    .Where(d => versionsMap.ContainsKey(d.Name.LocalName))
                    .ToList()
                    .ForEach(d => d.Value = versionsMap[d.Name.LocalName]);

                File.WriteAllText(file, document.ToString());
            }
        }
    }
}
