using JWTInspector.MAUI.ViewModels;

namespace JWTInspector.MAUI.Components;

public partial class KeyView : ContentView
{
	public KeyView()
	{
		InitializeComponent();
	}

    private async void ChooseFileButton_Clicked(object sender, EventArgs e)
    {
		var result = await FilePicker.Default.PickAsync(new PickOptions() { PickerTitle = "Choose file holding public or symmetric key" });
		if (result is not null && !string.IsNullOrEmpty(result.FullPath))
		{
			((TokenKeyViewModel)BindingContext).File.FilePath = result.FullPath;
		}
    }
}