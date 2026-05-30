namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using RelationsShared.DTO;
    using System.Collections.Generic;

    public interface IElementRelationsGetter
    {
        IEnumerable<Relation> GetFromContext(RelationsContext relationsContext);
    }
}
