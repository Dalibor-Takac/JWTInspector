using JWTInspector.MAUI.Services;

namespace JWTInspector.MAUI.ViewModels;

public class TokenKeyViewModel : BaseViewModel
{
	private CharacterKeyViewModel _characters = new();

	public CharacterKeyViewModel Characters
	{
		get { return _characters; }
		set { _characters = value; RaisePropertyChanged(); }
	}

	private FileKeyViewModel _file = new();

	public FileKeyViewModel File
	{
		get { return _file; }
		set { _file = value; RaisePropertyChanged(); }
	}

	private KeySource _selectedKey;

	public KeySource SelectedKey
	{
		get { return _selectedKey; }
		set { _selectedKey = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(IsCharactersSelected)); RaisePropertyChanged(nameof(IsFileSelected)); }
	}

	public bool IsCharactersSelected => _selectedKey == KeySource.Characters;
	public bool IsFileSelected => _selectedKey == KeySource.File;

	public IEnumerable<KeySource> AllSources => [KeySource.Characters, KeySource.File];
}
