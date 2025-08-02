namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using System.Collections.Generic;

    public class ReferencesBag : Dictionary<string, HashSet<string>>, IReferencesBag
    {
        public void AddReference(string element, string reference)
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

        public void AddReferences(string element, IEnumerable<string> references)
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
    }
}
