using System.ComponentModel;

namespace JWTInspector.Models;
public partial class VerificationKeyModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChange("KeyKind")]
    private string _keyKind;
}
