using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public interface ITypeCustomerInfoModel
    {
        TypeCustomerInfoTypes ModelType { get; }
    }
}
