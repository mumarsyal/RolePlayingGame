using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RolePlayingGame.Dtos.Character;
using RolePlayingGame.Services.CharacterService;
using System.Security.Claims;

namespace RolePlayingGame.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCharacter(AddCharacterDto character)
        {
            return Ok(await _characterService.AddNewCharacter(character));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var response = await _characterService.DeleteCharacter(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _characterService.GetCharacterById(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto character)
        {
            var response = await _characterService.UpdateCharacter(character);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
