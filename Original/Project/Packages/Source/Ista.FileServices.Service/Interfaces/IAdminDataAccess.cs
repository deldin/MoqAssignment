using Ista.Miramar.Interfaces;

namespace Ista.FileServices.Service.Interfaces
{
    /// <summary>
    /// Data Access methods for the Billing Admin database.
    /// </summary>
    public interface IAdminDataAccess
    {
        IMiramarClientInfo LoadClientInfo(int clientId);
    }
}
