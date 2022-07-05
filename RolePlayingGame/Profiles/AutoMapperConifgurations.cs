using AutoMapper;
using RolePlayingGame.Dtos.Character;
using RolePlayingGame.Models;

namespace RolePlayingGame.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddCharacterDto, Character>().ReverseMap(); //reverse so the both direction
            CreateMap<GetCharacterDto, Character>().ReverseMap(); //reverse so the both direction
        }
    }
}
