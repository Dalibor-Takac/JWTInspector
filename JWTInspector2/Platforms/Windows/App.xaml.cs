using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JWTInspector2.WinUI;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        var sizeTemplate = new AppShell();
        WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, window) =>
        {
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;
            handler.PlatformView.Activate();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(sizeTemplate.WidthRequest), Convert.ToInt32(sizeTemplate.HeightRequest)));
        });
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

