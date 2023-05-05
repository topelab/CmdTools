namespace UpdateVersion
{
    internal class VersionSplitter : IVersionSplitter
    {
        public (string name, string value) Split(string version, string defaultName)
        {
            string name;
            string value;

            if (version.Contains(Constants.ProjectVersionSeparator))
            {
                var parts = version.Split(Constants.ProjectVersionSeparator);
                name = parts[0].Trim();
                value = parts[1].Trim();
            }
            else
            {
                name = defaultName;
                value = version.Trim();
            }

            return (name, value);
        }
    }
}
