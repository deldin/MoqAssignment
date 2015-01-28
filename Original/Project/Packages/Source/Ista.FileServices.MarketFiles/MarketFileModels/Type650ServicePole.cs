using Ista.FileServices.MarketFiles.Enumerations;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650ServicePole : IType650Model
    {
        public Type650Types ModelType
        {
            get { return Type650Types.ServicePole; }
        }

        public int ServiceKey { get; set; }
        public int? ServicePoleKey { get; set; }
        public string PoleNbr { get; set; }
    }
}