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
}
