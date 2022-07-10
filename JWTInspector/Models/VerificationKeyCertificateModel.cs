using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTInspector.Models;
public partial class VerificationKeyCertificateModel: VerificationKeyModel
{
    [NotifyPropertyChange("CertificateFile")]
    private string _certificateFile;
    [NotifyPropertyChange("IncludedKeyKind")]
    private string _includedKeyKind;
    [NotifyPropertyChange("HasPrivatekey")]
    private bool _hasPrivateKey;
    [NotifyPropertyChange("HasPublicKey")]
    private bool _hasPublicKey;
}
