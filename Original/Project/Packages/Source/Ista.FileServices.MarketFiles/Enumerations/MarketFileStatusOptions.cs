
namespace Ista.FileServices.MarketFiles.Enumerations
{
    public enum MarketFileStatusOptions
    {
        Error = -1,
        Inserted = 0,
        Decrypted = 1,
        Encrypted = 2,
        Imported = 3,
        Transmitted = 4,
        Consumed = 5,
        Retransmitted = 6,
    }
}
