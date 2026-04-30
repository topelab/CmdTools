namespace CmdTools.Contracts
{
    using CmdTools.Contracts.DTO;
    using System.Collections.Generic;

    public interface IElementRelationsGetter
    {
        IEnumerable<Relation> Get<T>(T args) where T : class;
    }
}
