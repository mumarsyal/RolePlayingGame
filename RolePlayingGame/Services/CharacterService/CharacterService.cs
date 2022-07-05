using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RolePlayingGame.Data;
using RolePlayingGame.Dtos.Character;
using RolePlayingGame.Models;

namespace RolePlayingGame.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> Characters = new List<Character>
        {
            new Character(),
            new Character{ Id = 1, Name = "Samuel", Class = RpgClass.Cleric}
        };
        private readonly IMapper _mapper;
        private readonly DataContext _dbContext;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _dbContext = context;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddNewCharacter(AddCharacterDto newCharacter)
        {
            var character = _mapper.Map<Character>(newCharacter);
            await _dbContext.Characters.AddAsync(character);
            await _dbContext.SaveChangesAsync();
            var response = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = (_dbContext.Characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList()
            };
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = await _dbContext.Characters.FirstAsync(c => c.Id == id);
                _dbContext.Characters.Remove(character);
                await _dbContext.SaveChangesAsync();

                response.Data = (_dbContext.Characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
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
            var dbCharacters = await _dbContext.Characters.ToListAsync();
            var response = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = (dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList()
            };
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == id);
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
