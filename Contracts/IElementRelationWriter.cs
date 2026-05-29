namespace CmdTools.Contracts
{
    public interface IElementRelationWriter
    {
        void Write(string content, string outputFile, bool openOutput);
    }
}
