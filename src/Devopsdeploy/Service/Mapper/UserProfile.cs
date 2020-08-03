using AutoMapper;
using DevOpsDeploy.Models;
using DevOpsDeploy.Service.Dto;

namespace DevOpsDeploy.Service.Mapper {
    public class UserProfile : Profile {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
