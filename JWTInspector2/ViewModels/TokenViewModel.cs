using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JWTInspector2.ViewModels;
public partial class TokenViewModel: ObservableObject
{
    [ObservableProperty]
    private string? _signatureValue;
    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(IsSignatureInvalid))]
    [AlsoNotifyChangeFor(nameof(IsSignatureValid))]
    [AlsoNotifyChangeFor(nameof(IsSignatureUnchecked))]
    private SignatureValidity _validity;
    public bool IsSignatureValid => Validity == SignatureValidity.Valid;
    public bool IsSignatureInvalid => Validity == SignatureValidity.Invalid;
    public bool IsSignatureUnchecked => Validity == SignatureValidity.Unknown;
    [ObservableProperty]
    private string? _tokenType;
    [ObservableProperty]
    private string? _tokenSignatureAlgorithm;
    [ObservableProperty]
    private ObservableCollection<TokenClaim> _body;

    public TokenViewModel()
    {
        _body = new ObservableCollection<TokenClaim>();
    }
}

public enum SignatureValidity
{
    Unknown = default,
    Invalid,
    Valid
}

public partial class TokenClaim: ObservableObject
{
    [ObservableProperty]
    private string? _claimName;
    [ObservableProperty]
    private JsonValue? _claimValue;
}
