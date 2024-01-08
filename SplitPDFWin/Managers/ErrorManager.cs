using System;
using System.Collections.Generic;
using System.Linq;
using SplitPDFWin.ViewModels;

namespace SplitPDFWin.Managers
{
    internal class ErrorManager<T> : IErrorManager<T> where T : BaseVM
    {
        private readonly Dictionary<string, List<string>> errors;
        private readonly HashSet<string> properties;

        public ErrorManager()
        {
            errors = [];
            properties = typeof(T).GetProperties().Select(p => p.Name).ToHashSet();
        }

        public bool HasErrors => errors.Any(e => e.Value.Count != 0);

        public void ClearErrors(string propertyName = null)
        {
            if (propertyName != null)
            {
                CheckPropertyName(propertyName);
                if (errors.TryGetValue(propertyName, out var value))
                {
                    value.Clear();
                }
            }
            else
            {
                errors.Clear();
            }
        }

        public void SetError(string propertyName, string error)
        {
            CheckPropertyName(propertyName);
            if (errors.TryGetValue(propertyName, out var errorByProperty))
            {
                errorByProperty.Add(error);
            }
            else
            {
                errors[propertyName] = [error];
            }
        }

        public void SetError(string propertyName, IEnumerable<string> errorList)
        {
            CheckPropertyName(propertyName);
            if (errors.TryGetValue(propertyName, out var errorByProperty))
            {
                errorByProperty.AddRange(errorList);
            }
            else
            {
                errors[propertyName] = [.. errorList];
            }
        }

        public IEnumerable<string> GetErrors(string propertyName = null)
        {
            var result = new List<string>();
            if (propertyName == null)
            {
                result.AddRange(errors.SelectMany(e => e.Value));
            }
            else
            {
                CheckPropertyName(propertyName);
                if (errors.TryGetValue(propertyName, out var value))
                {
                    result.AddRange(value);
                }
            }
            return result;
        }

        private void CheckPropertyName(string propertyName)
        {
            if (!properties.Contains(propertyName))
            {
                throw new Exception($"{typeof(T).Name} no contiene {propertyName}");
            }
        }
    }
}
