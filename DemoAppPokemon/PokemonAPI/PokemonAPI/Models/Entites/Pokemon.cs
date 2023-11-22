
namespace PokemonAPI.Models.Entites
{
    public class Pokemon : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Element { get; set; } = string.Empty;

    }
}
