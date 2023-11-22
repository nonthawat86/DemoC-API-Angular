using Microsoft.EntityFrameworkCore;
using PokemonAPI.Data;
using PokemonAPI.Models.Entites;

namespace PokemonAPI.Repository
{
    public interface IPokemonRepository
    {
        Task<List<Pokemon>> GetPokemonAll();  
    }
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _db;
        public PokemonRepository(DataContext context)
        {
            _db = context;
        }

        public async Task<List<Pokemon>> GetPokemonAll()
        {
            return await _db.pokemons.Where(s=>s.IsDeleted == false).ToListAsync();
        }
    }
}
