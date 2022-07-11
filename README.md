# JWTInspector

A simple tool and a way to brush up on my WPF skills.

Goal of this tool is to allow you to inspect JWT token in offline mode. It can be used when you do not have internet access (or its not reliable enough to use https://jwt.io/) or for security sensitive tokens you do not want to entrust to online tools (like your production access tokens or tokens acquired from customers with personally identifiable information in them).

This tool is open source and deliberately published with permissive license so you do not have issues with that.

To build it simply clone, switch into directory and run
```Shell
dotnet build
```
from command line
You will need dotnet sdk at least for .net 6

App has been tested extensively with RSxxx ESxxx and HSxxx tokens from https://jwt.io. PSxxx tokens verification should work as well but has not been tested.

Token verification supports entering plaintext or base64 encoded key for symetric key cryptography jwt signatures and certificate files and public kes in PEM format for asymetric key cryptography signatures. Encrypted PEM key files and PKCS12 file format is not supported since they are usually used to store private keys (along side public keys or certificates in case of PKCS12).
