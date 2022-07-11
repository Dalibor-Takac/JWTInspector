using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTInspector.Models;
public partial class TokenViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChange("Token")]
    private JWTToken? _token;

    [NotifyPropertyChange("TokenText")]
    private string? _tokenText;

    [NotifyPropertyChange("VerificationKey")]
    private VerificationKeyModel? _verificationKey;

    [NotifyPropertyChange("SelectedKeySource")]
    private int _selectedkeySource;

    [NotifyPropertyChange("ErrorMessage")]
    private string? _errorMessage;

    [NotifyPropertyChange("SignatureError")]
    private string? _signatureError;

    public TokenViewModel()
    {
        VerificationKey = new VerificationKeyBase64EncodedModel();
        VerificationKey.SubscribeToChanges(VerificationKey_PropertyChanged);
        PropertyChanged += TokenViewModel_PropertyChanged;
    }

    private void TokenViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TokenText))
        {
            if (TokenText is not null)
                (ErrorMessage, Token) = JWTToken.Parse(TokenText);
            else
            {
                Token = null;
                ErrorMessage = null;
            }
        }
        else if (e.PropertyName == nameof(SelectedKeySource))
        {
            if (SelectedKeySource == 0)
            {
                VerificationKey = new VerificationKeyBase64EncodedModel();
                VerificationKey.SubscribeToChanges(VerificationKey_PropertyChanged);
            }
            else
            {
                VerificationKey = new VerificationKeyCertificateModel();
                VerificationKey.SubscribeToChanges(VerificationKey_PropertyChanged);
            }
        }
    }

    private void VerificationKey_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Token is not null)
        {
            if (VerificationKey.IsJwtSignatureSupported(Token.Header.Algorithm))
            {
                (var checkedSignature, SignatureError) = VerificationKey.VerifySignature(TokenText, Token.Header.Algorithm);
                Token = new JWTToken(Token.Header, Token.Body, new JWTSignature(Token.Signature.Signature, checkedSignature));
            }
        }
    }
}
