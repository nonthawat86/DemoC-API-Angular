using Microsoft.EntityFrameworkCore;
using PokemonAPI.Models.Entites;

namespace PokemonAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        //Dbset
        public DbSet<Pokemon> pokemons { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
        public DbSet<Company> Company { get; set; } = null!;
        public DbSet<SystemConfig> SystemConfig { get; set; } = null!;
    }
}
