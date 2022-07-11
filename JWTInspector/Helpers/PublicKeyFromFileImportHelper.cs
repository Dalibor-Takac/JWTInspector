using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace JWTInspector.Helpers;
public static class PublicKeyFromFileImportHelper
{
    public static KeyFileImportResult ImportKeysFromCertificateFile(string file)
    {
        try
        {
            var cert = new X509Certificate2(file);
            return new KeyFileImportResult()
            {
                RsaKey = cert.GetRSAPublicKey(),
                EcKey = cert.GetECDsaPublicKey()
            };
        }
        catch (CryptographicException ex)
        {
            return new KeyFileImportResult() { ErrorMessage = ex.Message };
        }
    }

    public static KeyFileImportResult ImportKeysFromPemFile(string file)
    {
        try
        {
            var fileLines = File.ReadAllLines(file);
            var base64EncodedContent = string.Join("", fileLines.Skip(1).Take(fileLines.Length - 2));
            var rsa = RSA.Create();
            var ecdsa = ECDsa.Create();
            if (fileLines[0].Contains("BEGIN PUBLIC KEY"))
            {
                try
                {
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(base64EncodedContent), out var _);
                }
                catch (CryptographicException)
                {
                    ecdsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(base64EncodedContent), out var _);
                }
            }
            else if (fileLines[0].Contains("BEGIN RSA PUBLIC KEY"))
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(base64EncodedContent), out var _);
            }
            return new KeyFileImportResult() { RsaKey = rsa, EcKey = ecdsa };
        }
        catch (CryptographicException ex)
        {
            return new KeyFileImportResult() { ErrorMessage = ex.Message };
        }
    }

    public static KeyFileImportResult ImportKeysFromFile(string file)
    {
        var certImportResult = ImportKeysFromCertificateFile(file);
        var pemImportResult = ImportKeysFromPemFile(file);
        return certImportResult.ErrorMessage is null ? certImportResult : pemImportResult;
    }
}
