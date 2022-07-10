using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTInspector.Models;
public partial class VerificationKeyBase64EncodedModel: VerificationKeyModel
{
    [NotifyPropertyChange("Base64EncodedKey")]
    private string _base64EncodedKey;
    [NotifyPropertyChange("IsBase64Encoded")]
    private bool _isBase64Encoded;
}
