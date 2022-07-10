using System.ComponentModel;
using System.Text.Json.Serialization;

namespace JWTInspector.Models;
public record JWTHeader([property: JsonPropertyName("alg")] TokenSignatureAlgorithm Algorithm, [property: JsonPropertyName("typ")] TokenKind Kind) { }

public enum TokenSignatureAlgorithm
{
    [Description("HMAC with SHA256")]
    HS256,
    [Description("HMAC with SHA384")]
    HS384,
    [Description("HMAC with SHA512")]
    HS512,
    [Description("RSASSA-PKCS1-v1_5 with SHA256")]
    RS256,
    [Description("RSASSA-PKCS1-v1_5 with SHA384")]
    RS384,
    [Description("RSASSA-PKCS1-v1_5 with SHA512")]
    RS512,
    [Description("ECDSA with SHA256")]
    ES256,
    [Description("ECDSA with SHA384")]
    ES384,
    [Description("ECDSA with SHA512")]
    ES512,
    [Description("RSASSA-PSS with SHA256")]
    PS256,
    [Description("RSASSA-PSS with SHA384")]
    PS384,
    [Description("RSASSA-PSS with SHA512")]
    PS512,
}

public enum TokenKind
{
    JWT
}
