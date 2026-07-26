using Learnova.Application.Exceptions;
using Learnova.Application.Courses.Services;
using Learnova.Application.Enrollment.Services;
using Learnova.Application.User;
using Learnova.Domain.Entities;
using Learnova.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Quizzes.Command.SubmitQuizAttempt
{
    public class SubmitQuizAttemptCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<SubmitQuizAttemptCommandHandler> logger,
        IUserContext userContext,
        ICourseAccessService courseAccessService,
        IEnrollmentProgressService enrollmentProgressService) : IRequestHandler<SubmitQuizAttemptCommand>
    {
        public async Task Handle(SubmitQuizAttemptCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling quiz attempt submission for user ID {UserId} and quiz ID {QuizId}", userContext.GetCurrentUser()?.Id, request.QuizId);

            var user = userContext.GetCurrentUser();
            if (user == null)
            {
                logger.LogWarning("Unauthorized attempt to submit quiz for QuizId {QuizId}", request.QuizId);
                throw new UnauthorizedException("User is not authenticated.");
            }

            var quiz = await unitOfWork.quiz.GetById(request.QuizId, q => q.Questions);
            if (quiz == null)
            {
                logger.LogError("Quiz not found for QuizId {QuizId}", request.QuizId);
                throw new NotFoundException("Quiz not found");
            }

            var courseId = quiz.CourseId;
            logger.LogInformation("Checking enrollment for user {UserId} in course {CourseId}", user.Id, courseId);

            await courseAccessService.EnsureStudentEnrolledInCourseAsync(
                courseId,
                user.Id,
                cancellationToken);

            var enroll = await unitOfWork.enrollment.GetByStudentAndCourseAsync(user.Id, courseId);
            if (enroll == null)
            {
                logger.LogWarning("Student {UserId} is not enrolled in course {CourseId}", user.Id, courseId);
                throw new NotFoundException("Enrollment not found.");
            }

            var quizQuestions = quiz.Questions
                .Where(question => !question.IsDeleted)
                .ToList();

            ValidateSubmittedAnswers(quizQuestions, request);

            var quizAttempt = new QuizAttempt
            {
                StudentId = user.Id,
                QuizId = request.QuizId,
                Answers = new List<QuizAnswer>()
            };

            foreach (var answer in request.Answers)
            {
                logger.LogInformation("Checking correctness of answer for QuestionId {QuizQuestionId} and ChosenAnswerIndex {ChosenAnswerIndex}",
                    answer.QuizQuestionId, answer.ChosenAnswerIndex);

                var question = quizQuestions.First(q => q.Id == answer.QuizQuestionId);

                var quizAnswer = new QuizAnswer
                {
                    QuizQuestionId = answer.QuizQuestionId,
                    ChosenAnswerIndex = answer.ChosenAnswerIndex,
                    IsCorrect = question.CorrectAnswerIndex == answer.ChosenAnswerIndex
                };

                quizAttempt.Answers.Add(quizAnswer);
            }

            quizAttempt.TotalQuestions = quizQuestions.Count;
            quizAttempt.Score = quizAttempt.Answers.Count(a => a.IsCorrect);

            var passRatio = quizAttempt.TotalQuestions == 0
                ? 0
                : (double)quizAttempt.Score / quizAttempt.TotalQuestions;

            quizAttempt.IsPass = passRatio >= 0.5d;

            logger.LogInformation(
                "Computed quiz attempt result for student {StudentId}, quiz {QuizId}: score {Score}/{Total}, pass={IsPass}",
                user.Id, request.QuizId, quizAttempt.Score, quizAttempt.TotalQuestions, quizAttempt.IsPass);

            await unitOfWork.ExecuteInTransactionAsync(async transactionCancellationToken =>
            {
                await unitOfWork.quizAttempt.Add(quizAttempt);

               
                await unitOfWork.CompleteAsync(transactionCancellationToken);

                await enrollmentProgressService.RecalculateCourseProgressAsync(
                    user.Id,
                    courseId,
                    transactionCancellationToken);

                await unitOfWork.CompleteAsync(transactionCancellationToken);
            }, cancellationToken);
        }

        private static void ValidateSubmittedAnswers(IReadOnlyCollection<QuizQuestion> quizQuestions, SubmitQuizAttemptCommand request)
        {
            var questionIds = quizQuestions.Select(q => q.Id).ToHashSet();
            var submittedQuestionIds = request.Answers.Select(a => a.QuizQuestionId).ToList();

            if (questionIds.Count == 0)
            {
                throw new BadRequestException("Quiz has no questions.");
            }

            if (submittedQuestionIds.Count != submittedQuestionIds.Distinct().Count())
            {
                throw new BadRequestException("Each quiz question can only be answered once.");
            }

            if (submittedQuestionIds.Count != questionIds.Count ||
                submittedQuestionIds.Any(id => !questionIds.Contains(id)))
            {
                throw new BadRequestException("Submitted answers must match the quiz questions.");
            }

            foreach (var answer in request.Answers)
            {
                var question = quizQuestions.First(q => q.Id == answer.QuizQuestionId);

                if (answer.ChosenAnswerIndex >= question.Options.Count)
                {
                    throw new BadRequestException(
                        $"ChosenAnswerIndex is out of range for question {answer.QuizQuestionId}.");
                }
            }
        }

    }
}

