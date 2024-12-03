namespace JWTInspector.MAUI.ViewModels;

public class CharacterKeyViewModel : BaseViewModel
{
	private string _keyCharacters = string.Empty;

	public string KeyCharacters
	{
		get { return _keyCharacters; }
		set { _keyCharacters = value; RaisePropertyChanged(); }
	}

	private bool _isBase64Encoded;

	public bool IsBase64Encoded
	{
		get { return _isBase64Encoded; }
		set { _isBase64Encoded = value; RaisePropertyChanged(); }
	}

	private bool _isUrlEncoded;

	public bool IsUrlEncoded
	{
		get { return _isUrlEncoded; }
		set { _isUrlEncoded = value; RaisePropertyChanged(); }
	}
}
