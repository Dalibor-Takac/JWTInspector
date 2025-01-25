using JWTInspector.MAUI.ViewModels;

namespace JWTInspector.MAUI.Components;

public partial class KeyView : ContentView
{
	public KeyView()
	{
		InitializeComponent();
	}

	private readonly string[] _convinienceExtensions = [".pem", ".key", ".cer", ".crt", ".pfx", string.Empty];

    private async void ChooseFileButton_Clicked(object sender, EventArgs e)
    {
		var result = await FilePicker.Default.PickAsync(new PickOptions()
		{
			PickerTitle = "Choose file holding public or symmetric key",
			FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
			{
				{ DevicePlatform.WinUI, _convinienceExtensions }
			})
		});
		if (result is not null && !string.IsNullOrEmpty(result.FullPath))
		{
			((TokenKeyViewModel)BindingContext).File.FilePath = result.FullPath;
		}
    }
}