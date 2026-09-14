using Domain.Entites;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataSeeding
{

    public static class DbInitializer
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // 1. Diplomas
            var webDiplomaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var mobileDiplomaId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Diploma>().HasData(
                new Diploma
                {
                    Id = webDiplomaId,
                    Title = "Full-Stack Web Development Diploma",
                    Description = "Master ASP.NET Core and Modern Frontend Frameworks",
                    Status = ContentStatus.Published,
                    CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new Diploma
                {
                    Id = mobileDiplomaId,
                    Title = "Mobile App Development Diploma",
                    Description = "Build Cross-Platform Apps using Flutter",
                    Status = ContentStatus.Draft,
                    CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 2. Quizzes
            var csharpQuizId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var efCoreQuizId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            modelBuilder.Entity<Quiz>().HasData(
                new Quiz
                {
                    Id = csharpQuizId,
                    DiplomaId = webDiplomaId,
                    Title = "C# Fundamentals Quiz",
                    Instructions = "Answer all questions within the time limit.",
                    DurationMinutes = 30,
                    PassScore = 70,
                    MaxAttempts = 3,
                    Status = ContentStatus.Published,
                    CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new Quiz
                {
                    Id = efCoreQuizId,
                    DiplomaId = webDiplomaId,
                    Title = "Entity Framework Core Advanced Quiz",
                    Instructions = "Focus on performance and entity configurations.",
                    DurationMinutes = 45,
                    PassScore = 60,
                    MaxAttempts = null,
                    Status = ContentStatus.Draft,
                    CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 3. Questions
            var q1Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var q2Id = Guid.Parse("66666666-6666-6666-6666-666666666666");

            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = q1Id,
                    QuizId = csharpQuizId,
                    Text = "Which keyword is used to handle exceptions in C#?",
                    Explanation = "The 'try-catch' block is used to catch and handle runtime exceptions.",
                    OrderIndex = 1,
                    CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new Question
                {
                    Id = q2Id,
                    QuizId = csharpQuizId,
                    Text = "What is the default access modifier for members of a class in C#?",
                    Explanation = "In C#, class members are private by default if no modifier is specified.",
                    OrderIndex = 2,
                    CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 4. Question Options
            modelBuilder.Entity<QuestionOption>().HasData(
                // Options for Q1
                new QuestionOption { Id = Guid.Parse("77777777-1111-1111-1111-111111111111"), QuestionId = q1Id, Text = "try / catch", IsCorrect = true },
                new QuestionOption { Id = Guid.Parse("77777777-2222-2222-2222-222222222222"), QuestionId = q1Id, Text = "do / while", IsCorrect = false },
                new QuestionOption { Id = Guid.Parse("77777777-3333-3333-3333-333333333333"), QuestionId = q1Id, Text = "if / else", IsCorrect = false },

                // Options for Q2
                new QuestionOption { Id = Guid.Parse("88888888-1111-1111-1111-111111111111"), QuestionId = q2Id, Text = "public", IsCorrect = false },
                new QuestionOption { Id = Guid.Parse("88888888-2222-2222-2222-222222222222"), QuestionId = q2Id, Text = "private", IsCorrect = true },
                new QuestionOption { Id = Guid.Parse("88888888-3333-3333-3333-333333333333"), QuestionId = q2Id, Text = "protected", IsCorrect = false },
                new QuestionOption { Id = Guid.Parse("88888888-4444-4444-4444-444444444444"), QuestionId = q2Id, Text = "internal", IsCorrect = false }
            );
        }
    }
}
