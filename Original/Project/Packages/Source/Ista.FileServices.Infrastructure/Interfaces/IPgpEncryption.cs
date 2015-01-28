
namespace Ista.FileServices.Infrastructure.Interfaces
{
    /// <summary>
    /// Responsble for encrypting and decrypting files using PGP algorithms.
    /// </summary>
    public interface IPgpEncryption
    {
        /// <summary>
        /// Decrypts contents of source file writing contents
        /// to destination target file.
        /// </summary>
        /// <param name="sourceName">Encrypted file</param>
        /// <param name="targetName">Decrypted file</param>
        void DecryptFile(string sourceName, string targetName);

        /// <summary>
        /// Encrypts contents of source file writing contents
        /// to destination target file.
        /// </summary>
        /// <param name="sourceName">Non Encrypted file</param>
        /// <param name="targetName">Encrypted file</param>
        void EncryptFile(string sourceName, string targetName);
    }
}
