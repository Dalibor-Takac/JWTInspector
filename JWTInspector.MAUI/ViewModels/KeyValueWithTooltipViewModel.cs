namespace JWTInspector.MAUI.ViewModels;

public class KeyValueWithTooltipViewModel : BaseViewModel
{
	private string _key = default!;

	public required string Key
	{
		get { return _key; }
		set { _key = value; RaisePropertyChanged(); }
	}

	private string? _value;

	public string? Value
	{
		get { return _value; }
		set { _value = value; RaisePropertyChanged(); }
	}

	private string? _tooltip;

	public string? Tooltip
	{
		get { return _tooltip; }
		set { _tooltip = value; RaisePropertyChanged(); }
	}
}
