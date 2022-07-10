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
    private JWTToken _token;

    [NotifyPropertyChange("TokenText")]
    private string _tokenText;

    [NotifyPropertyChange("VerificationKey")]
    private VerificationKeyModel _verificationKey;

    [NotifyPropertyChange("SelectedKeySource")]
    private int _selectedkeySource;

    public TokenViewModel()
    {
        PropertyChanged += TokenViewModel_PropertyChanged;
    }

    private void TokenViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Token))
        {

        }
        else if (e.PropertyName == nameof(SelectedKeySource))
        {
            if (SelectedKeySource == 0)
                VerificationKey = new VerificationKeyBase64EncodedModel();
            else
                VerificationKey = new VerificationKeyCertificateModel();
        }
    }
}
