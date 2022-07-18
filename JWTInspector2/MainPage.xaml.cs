using JWTInspector2.ViewModels;

namespace JWTInspector2;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel() { Token = new TokenViewModel() { TokenType = "JWT", TokenSignatureAlgorithm = "HS256", Validity = SignatureValidity.Unknown, Body = new System.Collections.ObjectModel.ObservableCollection<TokenClaim>() { new TokenClaim() { ClaimName = "bla", ClaimValue = System.Text.Json.Nodes.JsonValue.Create("some value") }, new TokenClaim() { ClaimName = "exp", ClaimValue = System.Text.Json.Nodes.JsonValue.Create(DateTime.Now) } } } };
    }
}

