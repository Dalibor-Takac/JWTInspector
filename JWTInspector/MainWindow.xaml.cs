using JWTInspector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JWTInspector;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var token = new TokenViewModel();
        try
        {
            token.ParseToken((sender as TextBox ?? throw new ArgumentException("Sender of event was not a TextBox")).Text);
        }
        catch (Exception ex)
        {
            token.Token = new Token(null, Enumerable.Empty<BodyElement>(), $"Failed to parse the token, {ex.Message}");
        }
        DataContext = token;
    }
}
