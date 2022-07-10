using System.ComponentModel;

namespace JWTInspector.Models;

public record JWTSignature(string Signature, SignatureState SignatureState);

public enum SignatureState
{
    Unknown = default,
    Valid,
    Invalid
}
