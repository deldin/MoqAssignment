
namespace Ista.FileServices.Infrastructure.Interfaces
{
    /// <summary>
    /// Responsible for FTP'ing files.
    /// </summary>
    public interface IFtpHandler
    {
        /// <summary>
        /// Transmits local file via FTP.
        /// </summary>
        /// <remarks>
        /// Performs a "ChangeDir" if a remote directory is given.
        /// </remarks>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <param name="localFileName">Full path to local file</param>
        /// <param name="remoteFileName">Remote file name</param>
        /// <exception cref="System.ArgumentNullException">Local File is empty or null</exception>
        /// <exception cref="System.ArgumentNullException">Remote file name is empty or null</exception>
        /// <exception cref="System.Exception">Unable to FTP file</exception>
        void SendFile(string remoteDirectory, string localFileName, string remoteFileName);
    }
}
