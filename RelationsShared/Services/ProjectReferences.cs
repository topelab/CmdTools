namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Collections.Generic;

    internal class ProjectReferences : IProjectReferences
    {
        private bool withPackages;
        private readonly Dictionary<string, string> packageVersions = [];
        private readonly List<ProjectRelation> projectRelations = [];

        public void Initialize(ProjectRelationsContext context)
        {
            InitializePackages(context);
            InitializeProjectsRelations(context);
        }

        public ReferencesBag GetReferences(HashSet<string> projectFiles)
        {
            ReferencesBag references = [];
            foreach (var file in projectFiles)
            {
                references[Path.GetFileNameWithoutExtension(file)] = [.. Get(file)];
            }

            return references;
        }

        public ReferencesBag GetInverseReferences(HashSet<string> projectFiles)
        {
            ReferencesBag references = [];
            foreach (var file in projectFiles)
            {
                Get(file).ToList().ForEach(r => references.AddReference(r, Path.GetFileNameWithoutExtension(file)));
            }

            return references;
        }

        private void InitializePackages(ProjectRelationsContext context)
        {
            var options = context.Options as ProjectOptions;
            withPackages = options.WithPackages;
            packageVersions.Clear();
            if (withPackages)
            {
                context.PackageVersions.Keys.ToList().ForEach(k => packageVersions[k] = context.PackageVersions[k]);
            }
        }

        private void InitializeProjectsRelations(ProjectRelationsContext context)
        {
            projectRelations.Clear();
            context.ElementsRelations.Keys
                .ToList()
                .ForEach(r =>
                {
                    var relations = context.ElementsRelations[r]
                        .Where(pr => withPackages || pr.RelationType != ProjectRelationType.PackageReference)
                        .Select(pr => new ProjectRelation(r, pr));

                    projectRelations.AddRange(relations);
                });
        }

        private IEnumerable<string> Get(string projectPath)
        {
            return projectRelations
                .Where(r => r.Parent == projectPath)
                .Select(r => $"{r.Child}")
                .Distinct();
        }
    }
}
