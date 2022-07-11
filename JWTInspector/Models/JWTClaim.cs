using System.Text.Json;

namespace JWTInspector.Models;
public record JWTClaim(string ClaimType, JsonElement ClaimValue, string? ToolTip);
