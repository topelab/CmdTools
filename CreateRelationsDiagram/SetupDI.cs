namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using Topelab.Core.Resolver.Entities;

    public class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(RelationsShared.SetupDI.Register())
                ;
        }
    }
}