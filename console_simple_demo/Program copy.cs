#if NEVER
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static void Main()
    {
        using (var rsaOriginal = RSA.Create()) 		// Create original keys
        using (var rsaImportedPriv = RSA.Create()) 	// For the later import of the private key
        using (var rsaImportedPub = RSA.Create())   // For the later import of the public key
        {
            // Export as DER
            var pkcs8Der = rsaOriginal.ExportPkcs8PrivateKey();
            var spkiDer = rsaOriginal.ExportSubjectPublicKeyInfo();

            // Convert to PEM (.NET 5+: using PemEncoding)
            var pkcs8Pem = new string(PemEncoding.Write("PRIVATE KEY", pkcs8Der)); // as of .NET 7: ExportPkcs8PrivateKeyPem
            var spkiPem = new string(PemEncoding.Write("PUBLIC KEY", spkiDer));    // as of .NET 7: ExportSubjectPublicKeyInfoPem
            Console.WriteLine(pkcs8Pem);
            Console.WriteLine(spkiPem);

            // Import as PEM
            rsaImportedPriv.ImportFromPem(pkcs8Pem);
            rsaImportedPub.ImportFromPem(spkiPem);

            // Test the keys
            Console.WriteLine(
                Encoding.UTF8.GetString(
                    rsaImportedPriv.Decrypt(
                        rsaImportedPub.Encrypt(
                            Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog"),
                            RSAEncryptionPadding.Pkcs1),
                        RSAEncryptionPadding.Pkcs1)));
        }
    }
}
#endif