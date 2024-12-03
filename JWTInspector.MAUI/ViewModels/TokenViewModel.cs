using System.Collections.ObjectModel;

namespace JWTInspector.MAUI.ViewModels;

public class TokenViewModel : BaseViewModel
{
	private ObservableCollection<KeyValueWithTooltipViewModel> _tokenHeader = new();

	public ObservableCollection<KeyValueWithTooltipViewModel> TokenHeader
	{
		get { return _tokenHeader; }
		set { _tokenHeader = value; RaisePropertyChanged(); }
	}

	private ObservableCollection<KeyValueWithTooltipViewModel> _tokenContent = new();

	public ObservableCollection<KeyValueWithTooltipViewModel> TokenContent
	{
		get { return _tokenContent; }
		set { _tokenContent = value; RaisePropertyChanged(); }
	}

	private TokenVerificationStatus _verificationStatus;

	public TokenVerificationStatus VerificationStatus
	{
		get { return _verificationStatus; }
		set { _verificationStatus = value; RaisePropertyChanged(); }
	}

	private string? _errorMessage;

	public string? ErrorMessage
	{
		get { return _errorMessage; }
		set { _errorMessage = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(HasError)); }
	}

	public bool HasError
	{
		get { return !string.IsNullOrEmpty(_errorMessage); }
	}

	public void Reset()
	{
		ErrorMessage = null;
		TokenHeader.Clear();
		TokenContent.Clear();
        VerificationStatus = TokenVerificationStatus.Unverified;
	}

	public void AddHeaderEntry(string key, string value, string? tooltip)
	{
		if (!string.IsNullOrEmpty(value))
			TokenHeader.Add(new KeyValueWithTooltipViewModel() { Key = key, Value = value, Tooltip = tooltip });
	}

	public void AddContentEntry(string key, string value, string? tooltip)
	{
		AddContentEntry(key, val => !string.IsNullOrEmpty(val), _ => value, _ => tooltip, value);
    }

	public void AddContentEntry<T>(string key, Predicate<T> isDefined, Func<T, string> valueFormatter, Func<T, string?> tooltipFormatter, T value)	{
		if (isDefined(value))
			TokenContent.Add(new KeyValueWithTooltipViewModel() { Key = key, Value = valueFormatter(value), Tooltip = tooltipFormatter(value) });
	}
}

public enum TokenVerificationStatus
{
	Unverified = default,
	Valid,
	Invalid
}
