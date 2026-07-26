using AutoMapper;
using Learnova.Application.Quizzes.DTO;
using Learnova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Mapping
{
    public class QuizProfile:Profile
    {
        public QuizProfile()
        {
            CreateMap<Quiz, GetAllQuizzesDTO>()
                .ForMember(x => x.QuizId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.CourseName, opt => opt.MapFrom(s => s.Course.Title))
                .ForMember(x => x.QuestionsCount, opt => opt.MapFrom(s => s.Questions.Count(q => !q.IsDeleted)));


            CreateMap<QuizQuestion, GetQuizByIdQuestionDTO>()
                .ForMember(x => x.QuestionId, opt => opt.MapFrom(s => s.Id));

            CreateMap<Quiz, GetQuizByIdDTO>()
                .ForMember(x => x.QuizId, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.CourseName, opt => opt.MapFrom(s => s.Course.Title))
                .ForMember(x => x.Questions, opt => opt.MapFrom(s => s.Questions.Where(q => !q.IsDeleted)));

            CreateMap<QuizAttempt, GetMyAttemptsDTO>()
               .ForMember(x => x.AttemptId, opt => opt.MapFrom(s => s.Id))
               .ForMember(x => x.QuizTitle, opt => opt.MapFrom(s => s.Quiz.Title))
               .ReverseMap();

            CreateMap<QuizAttempt, GetAllAttemptsDTO>()
              .ForMember(x => x.AttemptId, opt => opt.MapFrom(s => s.Id))
              .ForMember(x => x.QuizTitle, opt => opt.MapFrom(s => s.Quiz.Title))
              .ForMember(x => x.StudentEmail, opt => opt.MapFrom(s => s.Student.Email))
              .ReverseMap();
        }
    }
}
