using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWTInspector.Helpers;
public class KeyFileImportResult
{
    public RSA? RsaKey { get; set; }
    public ECDsa? EcKey { get; set; }
    public string? ErrorMessage { get; set; }
}
