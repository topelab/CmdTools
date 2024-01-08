using System.Collections.Generic;
using SplitPDFWin.ViewModels;

namespace SplitPDFWin.Managers
{
    internal interface IErrorManager<T> where T : BaseVM
    {
        bool HasErrors { get; }

        void ClearErrors(string propertyName = null);
        IEnumerable<string> GetErrors(string propertyName = null);
        void SetError(string propertyName, IEnumerable<string> errorList);
        void SetError(string propertyName, string error);
    }
}