using PokemonAPI.Models.Entites;
using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models
{

    public class UserAuthen
    {
        public int EmployeeTypeId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }

    public class TokenApi
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
    public class UserAuthenMSAL
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }

    public class UserForgot
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }
    public class UserReset
    {
        [Required(ErrorMessage = "Id is required.")]
        public string? Id { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
    public class UserToken
    {
        public User UserEntity { get; set; }
        public string Token { get; set; }
    }
}
