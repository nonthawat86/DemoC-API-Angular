using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models.Entites
{
    public class Company : BaseEntity
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyAbbr { get; set; }

    }
}
