using System;
using System.Security.Cryptography;
using System.Text;

namespace dbj;

using static DBJLog;

public class Program
{
    public static async Task Main()
    {

        try
        {
            debug( "Starting " + app_friendly_name + ", function: " + DBJcore.Whoami() );
#if RSA_BASIC_DEMO
            RSADemo();
#else
            await ThreadChannelsSpecimen.Demo();
#endif
        }
        catch (Exception x)
        {
            //Console.Write(ConsoleColor.Red);
            error("Exception: " + x.Message );
            //Console.WriteLine("Exception: " + x.Message);
            //Console.Write(ConsoleColor.White);
        }

       debug("Done , function: " + DBJcore.Whoami());

    }
    public static void RSADemo()
    {
        using (var rsaOriginal = RSA.Create()) 		// Create original keys
        using (var private_key_imported_ = RSA.Create()) 	// For the later import of the private key
        using (var public_key_imported_ = RSA.Create())   // For the later import of the public key
        {
            // Export as DER
            var private_key_ = rsaOriginal.ExportPkcs8PrivateKey();
            var public_key_ = rsaOriginal.ExportSubjectPublicKeyInfo();

            // Convert to PEM (.NET 5+: using PemEncoding)
            var private_key_string_ = new string(PemEncoding.Write("PRIVATE KEY", private_key_)); // as of .NET 7: ExportPkcs8PrivateKeyPem
            var public_key_string_ = new string(PemEncoding.Write("PUBLIC KEY", public_key_));    // as of .NET 7: ExportSubjectPublicKeyInfoPem
            Console.WriteLine("private key: " + private_key_string_);
            Console.WriteLine("public key: " + public_key_string_);

            // Import as PEM
            private_key_imported_.ImportFromPem(private_key_string_);
            public_key_imported_.ImportFromPem(public_key_string_);

            var original_clear_text_ = "The quick brown fox jumps over the lazy dog";

            var encrypted_with_public_key_ = public_key_imported_.Encrypt(Encoding.UTF8.GetBytes(original_clear_text_), RSAEncryptionPadding.Pkcs1);

            var decrypted_with_private_key_ = private_key_imported_.Decrypt(encrypted_with_public_key_, RSAEncryptionPadding.Pkcs1);

            var decrypted_clear_text_ = Encoding.UTF8.GetString(decrypted_with_private_key_);

            // Test the keys
            Console.WriteLine("original_clear_text_: " + original_clear_text_);
            Console.WriteLine("encrypted_with_public_key_: " + Encoding.UTF8.GetString(encrypted_with_public_key_));
            Console.WriteLine("decrypted_clear_text_: " + decrypted_clear_text_);
        }
    }
}