using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public interface IType650Model
    {
        Type650Types ModelType { get; }
    }
}