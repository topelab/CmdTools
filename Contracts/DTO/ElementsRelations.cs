namespace CmdTools.Contracts.DTO
{
    using System.Collections.Generic;

    public class ElementsRelations : Dictionary<string, List<ElementRelation>>
    {
        public void AddReference(string element, ElementRelation reference)
        {
            if (TryGetValue(element, out var existingReferences))
            {
                existingReferences.Add(reference);
            }
            else
            {
                this[element] = [reference];
            }
        }

        public void AddReferences(string element, IEnumerable<ElementRelation> references)
        {
            if (TryGetValue(element, out var existingReferences))
            {
                foreach (var reference in references)
                {
                    existingReferences.Add(reference);
                }
            }
            else
            {
                this[element] = [.. references];
            }
        }

        public void AddRange(ElementsRelations newRelations)
        {
            foreach (var kvp in newRelations)
            {
                AddReferences(kvp.Key, kvp.Value);
            }
        }
    }
}
