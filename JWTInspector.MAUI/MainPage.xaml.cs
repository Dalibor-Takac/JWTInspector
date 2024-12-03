using JWTInspector.MAUI.Services;
using JWTInspector.MAUI.ViewModels;

namespace JWTInspector.MAUI;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _vm;
    private readonly ITokenProvider _tokenProvider;
    private readonly ITokenVerificationKeyProvider _keyProvider;

    public MainPage(ITokenProvider tokentProvider, ITokenVerificationKeyProvider keyProvider)
    {
        _tokenProvider = tokentProvider;
        _keyProvider = keyProvider;
        _vm = new MainPageViewModel();

        _vm.PropertyChanged += (sender, evt) =>
        {
            if (evt.PropertyName == nameof(_vm.Token))
            {
                _tokenProvider.ParseToken(_vm.Token, _vm.TokenView);
            }
        };
        _vm.KeyViewModel.Characters.PropertyChanged += (sender, evt) =>
        {
            var key = _keyProvider.GetSecurityKey(_vm.KeyViewModel, KeySource.Characters);
            if (key is not null && !string.IsNullOrEmpty(_vm.Token))
                _tokenProvider.ValidateToken(_vm.Token, key, _vm.TokenView);
        };
        _vm.KeyViewModel.File.PropertyChanged += (sender, evt) =>
        {
            var key = _keyProvider.GetSecurityKey(_vm.KeyViewModel, KeySource.File);
            if (key is not null && !string.IsNullOrEmpty(_vm.Token))
                _tokenProvider.ValidateToken(_vm.Token, key, _vm.TokenView);
        };

        InitializeComponent();
        BindingContext = _vm;
    }
}

