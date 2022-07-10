using System.Collections.ObjectModel;

namespace JWTInspector.Models;
public record JWTToken(JWTHeader Header, ObservableCollection<JWTClaim> Body, JWTSignature Signature) { }
