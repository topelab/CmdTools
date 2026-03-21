namespace RelationsShared.DTO
{
    public record SolutionExplorerItem(string Name, string Path, string SolutionPath, Dictionary<string, string> Projects);
}
