namespace JWTInspector.MAUI.ViewModels;

public class MainPageViewModel : BaseViewModel
{
	private string _token = string.Empty;

	public string Token
	{
		get { return _token; }
		set { _token = value; RaisePropertyChanged(); }
	}

	private TokenViewModel _tokenView = new();

	public TokenViewModel TokenView
	{
		get { return _tokenView; }
		set { _tokenView = value; RaisePropertyChanged(); }
	}

	private TokenKeyViewModel _keyViewModel = new();

	public TokenKeyViewModel KeyViewModel
	{
		get { return _keyViewModel; }
		set { _keyViewModel = value; RaisePropertyChanged(); }
	}
}
