using Application.CQRS.Features.UserFeature.Commands.CreateUser;
using Application.DTOs;
using Domain.Entites;
using Mapster;

namespace Application.CQRS.Mapping.Diplomas
{
    public static class DiplomaMappingExtension
    {
        static DiplomaMappingExtension()
        {
            TypeAdapterConfig<Diploma, GetDiplomaResponse>
                .NewConfig();
        }

        public static GetDiplomaResponse ToGetDiplomaResponse(this Diploma diploma)
        {
            return diploma.Adapt<GetDiplomaResponse>();
        }

        public static AddUserCommand ToDiplomaCommand(this Diploma diploma)
        {
            return diploma.Adapt<AddUserCommand>();
        }

    }
}
