using JWTInspector.Models;
using Microsoft.IdentityModel.Logging;
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
    private readonly TokenViewModel _tokenVM;
    public MainWindow()
    {
        IdentityModelEventSource.ShowPII = true;
        _tokenVM = new TokenViewModel();
        InitializeComponent();
        DataContext = _tokenVM;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _tokenVM.Clear();
        txtToken.Focus();
    }
}
