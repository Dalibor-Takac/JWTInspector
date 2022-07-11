using System;
using System.ComponentModel;

namespace JWTInspector.Models;
public abstract class VerificationKeyModel
{
    public abstract bool IsJwtSignatureSupported(TokenSignatureAlgorithm alg);
    public abstract (SignatureState state, string? error) VerifySignature(string token, TokenSignatureAlgorithm alg);
    public abstract void SubscribeToChanges(PropertyChangedEventHandler callback);
}
