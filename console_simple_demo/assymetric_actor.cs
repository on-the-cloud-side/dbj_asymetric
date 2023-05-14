using System.Security.Cryptography;

/*
   sends and recives encrypted messages
   interface parameters data type is string
   private key is never revealed or sent anywhere

   we use tuple literal we do not throw exceptions

   public (int value, string error) GetValueOrError()
{
    if (someCondition)
    {
        // Return value
        return (value: 42, error: null);
    }
    else
    {
        // Return error
        return (value: -1, error: "An error occurred.");
    }
}

    exceptions are not logical on the micro level and are detrimental to performance

    BEHAVIOUR: existence of interlocutor is implied. instances of this type are used as part of implementation only. otherwise there is no secrecy.
*/

namespace dbj;
interface IAssymtericActor
{
    // return the public key make here
    (string? value, string? error) GetPublicKey();
    //    assymetric public key is required to
    //    encrypt messages to be received by this actor
    //    assymetric private key is used to decrypt such messages
    (string? value, string? error) DecryptFromInterlocutor(string msg_);

    (string? value, string? error) EncryptForInterlocutor(string msg_);
}

sealed class AssymtericActor : IAssymtericActor
{
    RSA rsa_;
    public AssymtericActor()
    {
        rsa_ = RSA.Create();
        // implementers philosophy: all is in the rsa object created
        // no need and not safe too keep things outside
        // this is how you get keys pair
        // var private_key_ = rsa_.ExportPkcs8PrivateKey();
        // var public_key_ = rsa_.ExportSubjectPublicKeyInfo();
        // var private_key_string_ = new string(PemEncoding.Write("PRIVATE KEY", private_key_)); // as of .NET 7: ExportPkcs8PrivateKeyPem
        // var public_key_string_ = new string(PemEncoding.Write("PUBLIC KEY", public_key_));    // as of .NET 7: ExportSubjectPublicKeyInfoPem
    }

    // return the public key make here
    public (string? value, string? error) GetPublicKey()
    {
        // yes compute it on each call, do not keep keys arround
        // var public_key_ = rsa_.ExportSubjectPublicKeyInfo();
        // var public_key_string_ = 
        return (
            new string(PemEncoding.Write("PUBLIC KEY", rsa_.ExportSubjectPublicKeyInfo())),    // as of .NET 7: ExportSubjectPublicKeyInfoPem
            null // no error
        );
    }

    //    assymetric public key is required to
    //    encrypt messages to be received by this actor
    //    assymetric private key is used to decrypt such messages

    // interlocutor uses this method to get decrypted messages that where encrypted previously by this actor
    public (string? value, string? error) DecryptFromInterlocutor(string msg_)
    {
        return (msg_, null);
    }

    // interlocutor uses this method to get encrypted messages that only actor interlocutor can decrypt
    public (string? value, string? error) EncryptForInterlocutor(string msg_)
    {
        return (msg_, null);
    }

    // Object overrides are barred ------------------------------------------------------------------

    public override bool Equals(object? obj)
    {
        throw new NotImplementedException();
        // return false; // base.Equals(obj);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
        // return 0; // base.GetHashCode();
    }

    public override string? ToString()
    {
        throw new NotImplementedException();
        // return ""; // base.ToString();
    }
}