using System.Security.Cryptography.X509Certificates;

namespace ClassLibrary1.Services.Certificate;

public interface ICertificateService
{
    X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName);
}