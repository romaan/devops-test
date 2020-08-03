using System.Collections.Generic;
using System.Linq;
using DevOpsDeploy.Models;
using DevOpsDeploy.Service.Dto;

namespace DevOpsDeploy.Service.Mapper {
    public class UserMapper {
        public IEnumerable<UserDto> UsersToUserDtos(IEnumerable<User> users)
        {
            return users.Filter(it => it != null).Map(UserToUserDto);
        }

        public UserDto UserToUserDto(User user)
        {
            return new UserDto(user);
        }

        public IEnumerable<User> UserDtosToUsers(IEnumerable<UserDto> userDtos)
        {
            return userDtos.Filter(it => it != null).Map(UserDtoToUser);
        }

        public User UserDtoToUser(UserDto userDto)
        {
            if (userDto == null) return null;

            var userRoles = new HashSet<UserRole>();
            if (userDto.Roles != null)
                userRoles = userDto.Roles.Select(role => new UserRole {
                    Role = new Role {Name = role},
                    UserId = userDto.Id
                }).ToHashSet();

            return new User {
                Id = userDto.Id,
                Login = userDto.Login,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                ImageUrl = userDto.ImageUrl,
                Activated = userDto.Activated,
                LangKey = userDto.LangKey,
                UserRoles = userRoles
            };
        }

        public User UserFromId(string id)
        {
            if (id == null) return null;

            return new User {
                Id = id
            };
        }
    }
}
