using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokemonAPI.Models;
using PokemonAPI.Repository;
using System.Collections.Generic;

namespace PokemonAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository  _pokemonRepository;

        public PokemonController(IPokemonRepository pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;
        }

        [HttpGet(Name = "GetAllPokemon")]
        public async Task<ActionResult> GetAllPokemon()
        {
            try
            {
                return Ok(await _pokemonRepository.GetPokemonAll());
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}
