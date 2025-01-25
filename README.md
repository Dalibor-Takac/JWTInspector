# JWTInspector

A simple tool and a way to brush up on my WPF skills.

Goal of this tool is to allow you to inspect JWT token in offline mode. It can be used when you do not have internet access (or its not reliable enough to use https://jwt.io/) or for security sensitive tokens you do not want to entrust to online tools (like your production access tokens or tokens acquired from customers with personally identifiable information in them).

This tool is open source and deliberately published with permissive license so you do not have issues with that.

To build it simply clone, switch into directory and run
```Shell
dotnet build -f net9.0-windows10.0.22621.0 -c Release
```
from command line
You will need dotnet sdk at least version .net 9

App has been tested extensively with RSxxx ESxxx and HSxxx tokens from https://jwt.io. PSxxx tokens verification should work as well but has not been tested.

Token verification supports entering plaintext or base64 encoded key for symetric key cryptography jwt signatures and certificate files and public kes in PEM format for asymetric key cryptography signatures. Encrypted PEM key files are not supported but encrypted PKCS12 are (*.pfx)
