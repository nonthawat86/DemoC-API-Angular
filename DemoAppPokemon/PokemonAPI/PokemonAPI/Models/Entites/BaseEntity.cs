namespace PokemonAPI.Models.Entites
{

    public class BaseEntity
    {
        public DateTime CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? UpdateBy { get; set; }
        public bool Active { get; set; }
        public bool IsDeleted { get; set; }

    }
}
