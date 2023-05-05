namespace UpdateVersion
{
    internal interface IVersionSplitter
    {
        (string name, string value) Split(string version, string defaultName);
    }
}