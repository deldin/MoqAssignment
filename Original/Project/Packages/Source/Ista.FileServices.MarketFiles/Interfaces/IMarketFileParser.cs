using System.IO;

namespace Ista.FileServices.MarketFiles.Interfaces
{
    public interface IMarketFileParser
    {
        IMarketFileParseResult Parse(string fileName);
        IMarketFileParseResult Parse(Stream stream);
    }
}