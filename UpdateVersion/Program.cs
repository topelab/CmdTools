using CommandLine;
using System;
using UpdateVersion;

namespace SetVersion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
               .WithParsed(Proceed);
        }

        private static void Proceed(Options options)
        {
            
        }
    }
}