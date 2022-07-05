using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RolePlayingGame.Data;
using RolePlayingGame.Dtos.Character;
using RolePlayingGame.Models;
using System.Security.Claims;

namespace RolePlayingGame.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private async Task<List<GetCharacterDto>> GetCharactersForCurrentUser()
        {
            return await _dbContext.Characters
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c))
                    .ToListAsync();
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddNewCharacter(AddCharacterDto newCharacter)
        {
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == GetUserId());
            await _dbContext.Characters.AddAsync(character);
            await _dbContext.SaveChangesAsync();
            var response = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = await GetCharactersForCurrentUser()
            };
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = await _dbContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                if (character == null)
                {
                    throw new KeyNotFoundException();
                }
                _dbContext.Characters.Remove(character);
                await _dbContext.SaveChangesAsync();

                response.Data = await GetCharactersForCurrentUser();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = await GetCharactersForCurrentUser()
            };
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _dbContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                if (character == null)
                {
                    throw new KeyNotFoundException();
                }
                response.Data = _mapper.Map<GetCharacterDto>(character);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if (character == null)
                {
                    throw new KeyNotFoundException();
                }
                character.Name = updatedCharacter.Name;
                character.Strength = updatedCharacter.Strength;
                character.Defence = updatedCharacter.Defence;
                character.Class = updatedCharacter.Class;
                character.Intelligence = updatedCharacter.Intelligence;
                character.HitPoints = updatedCharacter.HitPoints;
                _dbContext.Characters.Update(character);
                await _dbContext.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}
