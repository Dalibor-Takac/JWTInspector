using System.Windows;
using System.Windows.Controls;

namespace JWTInspector.CustomControls;
/// <summary>
/// Interaction logic for TextBoxWithPlaceholderAndClear.xaml
/// </summary>
public partial class TextBoxWithPlaceholderAndClear : UserControl
{
    public static DependencyProperty InputTextProperty = DependencyProperty.Register(nameof(InputText), typeof(string), typeof(TextBoxWithPlaceholderAndClear), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged});
    public static DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(TextBoxWithPlaceholderAndClear));

    public string InputText { get { return (string)GetValue(InputTextProperty); } set { SetValue(InputTextProperty, value); } }
    public string PlaceholderText { get { return (string)GetValue(PlaceholderTextProperty); } set { SetValue(PlaceholderTextProperty, value); } }
    public TextBoxWithPlaceholderAndClear()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        SetValue(InputTextProperty, null);
    }

    private void UserControl_GotFocus(object sender, RoutedEventArgs e)
    {
        txtText.Focus();
    }
}
