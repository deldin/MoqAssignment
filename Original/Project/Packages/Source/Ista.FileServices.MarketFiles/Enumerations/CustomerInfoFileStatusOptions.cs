
namespace Ista.FileServices.MarketFiles.Enumerations
{
    public enum CustomerInfoFileStatusOptions
    {
        Error = -1,
        New = 1,
        ReadyForDownload = 2,
        ReadyForTransmission = 3,
        Sent = 4,
        Imported = 5,
    }
}
