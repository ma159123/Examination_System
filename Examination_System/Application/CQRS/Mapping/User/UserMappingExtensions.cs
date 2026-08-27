using Application.CQRS.Features.UserFeature.Commands.CreateUser;
using Domain.Entites;
using Mapster;

namespace HotelManagement.Application.Mapping.User
{
    public static class UserMappingExtensions
    {
        static UserMappingExtensions()
        {
            TypeAdapterConfig<AppUser, AddUserCommand>
                .NewConfig()
                .Map(dest => dest.FullName, src => src.FullName);
        }

        public static AppUser ToAppUser(this AddUserCommand command)
        {
            return command.Adapt<AppUser>();
        }

        public static AddUserCommand ToAddUserCommand(this AppUser user)
        {
            return user.Adapt<AddUserCommand>();
        }

    }
}
