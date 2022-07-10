﻿using JWTInspector.Models;
using Microsoft.Win32;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;

namespace JWTInspector.CustomControls;
/// <summary>
/// Interaction logic for CertificateSecurityKeyView.xaml
/// </summary>
public partial class CertificateSecurityKeyView : UserControl
{
    public CertificateSecurityKeyView()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var openFiledDlg = new OpenFileDialog();
        openFiledDlg.CheckFileExists = true;
        openFiledDlg.CheckPathExists = true;
        openFiledDlg.Filter = "Certificate|*.cer;*.crt;*.pfx|All files|*.*";
        if (openFiledDlg.ShowDialog().GetValueOrDefault())
        {
            var model = ((VerificationKeyCertificateModel)DataContext);
            model.CertificateFile = openFiledDlg.FileName;
            var certificate = new X509Certificate2(openFiledDlg.FileName);
            model.IncludedKeyKind = certificate.SignatureAlgorithm.FriendlyName;
            model.HasPrivatekey = certificate.HasPrivateKey;
            model.HasPublicKey = certificate.PublicKey is not null;
        }
    }
}
