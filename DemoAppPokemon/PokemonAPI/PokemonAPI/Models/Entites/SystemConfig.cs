using System.ComponentModel.DataAnnotations;

namespace PokemonAPI.Models.Entites
{

    public class SystemConfig
    {
        [Key]
        public int ConfigId { get; set; }
        public string ConfigName { get; set; }
        public string? ConfigDescription { get; set; }
        public string? ConfigParameter1 { get; set; }
        public string? ConfigParameter2 { get; set; }
        public string? ConfigParameter3 { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Active { get; set; }

    }
}
