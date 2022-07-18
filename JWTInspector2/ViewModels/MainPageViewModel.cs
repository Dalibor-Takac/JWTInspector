using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTInspector2.ViewModels;
public partial class MainPageViewModel: ObservableObject
{
    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(IsClearTokenTextVisible))]
    private string? _tokenText;
    public bool IsClearTokenTextVisible => !string.IsNullOrWhiteSpace(TokenText);
    [ObservableProperty]
    private TokenViewModel? _token;

    [ICommand]
    public void ClearToken()
    {
        TokenText = null;
        Token = null;
    }
}
