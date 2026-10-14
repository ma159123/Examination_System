using Application.DTOs;
using Domain.Entites;
using Mapster;

namespace Application.CQRS.Mapping.QuizMapping
{
    public static class QuizMappingExtension
    {
        static QuizMappingExtension()
        {
            TypeAdapterConfig<Quiz, QuizResponseDTO>
                .NewConfig();
        }

        public static QuizResponseDTO ToQuizResponseDTO(this Quiz quiz)
        {
            return quiz.Adapt<QuizResponseDTO>();
        }


    }
}
