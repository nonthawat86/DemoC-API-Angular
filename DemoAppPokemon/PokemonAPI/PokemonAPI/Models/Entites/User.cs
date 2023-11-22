using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models.Entites
{
    public class User : BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int EmployeeTypeId { get; set; }    //1:Employee,2:Contract,
        public int UserRoleId { get; set; } //1:Admin,2:Line Manager,3:Super Admin,4:User
        public string? Email { get; set; }
        public int? CompanyId { get; set; }
        public string? UnitCode { get; set; }
        public string? UnitName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
        public DateTime? PasswordExpireTime { get; set; }
        public bool FirstTimeLogin { get; set; }

    }
}
