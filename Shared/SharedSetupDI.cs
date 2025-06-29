using CmdTools.Contracts;
using Topelab.Core.Resolver.Entities;

namespace CmdTools.Shared
{
    public class SharedSetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddTransient<IFileExecutor, FileExecutor>()
                ;
        }
    }
}