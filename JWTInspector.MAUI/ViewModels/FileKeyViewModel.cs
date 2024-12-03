namespace JWTInspector.MAUI.ViewModels;

public class FileKeyViewModel : BaseViewModel
{
	private string _filePath = string.Empty;

	public string FilePath
	{
		get { return _filePath; }
		set { _filePath = value; RaisePropertyChanged(); }
	}

	private string _fileDecriptionPassword = string.Empty;

	public string FileDecriptionPassword
	{
		get { return _fileDecriptionPassword; }
		set { _fileDecriptionPassword = value; RaisePropertyChanged(); }
	}
}
