namespace PokemonAPI.Models.Response
{
    public class AuthResponse
    {
        public bool IsAuthSuccessful { get; set; }
        public ResponseUser? UserInfo { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
