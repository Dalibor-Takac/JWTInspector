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
    private readonly TokenViewModel _tokenVM;
    public MainWindow()
    {
        _tokenVM = new TokenViewModel();
        InitializeComponent();
        DataContext = _tokenVM;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _tokenVM.TokenString = null;
        _tokenVM.ErrorMessage = null;
        _tokenVM.Token = null;
        txtToken.Focus();
    }
}
