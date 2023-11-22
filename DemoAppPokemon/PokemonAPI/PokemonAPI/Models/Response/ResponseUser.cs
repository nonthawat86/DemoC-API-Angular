namespace PokemonAPI.Models.Response
{

    public class ResponseUser
    {
        public int? UserId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? UnitCode { get; set; }
        public string? UnitName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public int? RoleId { get; set; }
        public int? TrainerId { get; set; }
        public string? Position { get; set; }
        public string FirstTimeLogin { get; set; } = string.Empty;

    }

}
