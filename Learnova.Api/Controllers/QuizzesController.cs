using Learnova.Application.Courses.Command.CreateCourse;
using Learnova.Application.Quizzes.Command.AddQuestion;
using Learnova.Application.Quizzes.Command.CreateQuiz;
using Learnova.Application.Quizzes.Command.DeleteQuestion;
using Learnova.Application.Quizzes.Command.DeleteQuiz;
using Learnova.Application.Quizzes.Command.SubmitQuizAttempt;
using Learnova.Application.Quizzes.Command.UpdateQuestion;
using Learnova.Application.Quizzes.Command.UpdateQuiz;
using Learnova.Application.Quizzes.Query.GetAllAttempts;
using Learnova.Application.Quizzes.Query.GetCourseQuizzes;
using Learnova.Application.Quizzes.Query.GetMyAttempts;
using Learnova.Application.Quizzes.Query.GetQuizById;
using Learnova.Domain.Constant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Learnova.Api.Controllers
{
    
    [Route("api/quizzes")]
    [ApiController]
    public class QuizzesController(IMediator mediator) : ControllerBase
    {
        [Route("/api/courses/{courseId}/quizzes")]
        [HttpPost]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Create a course quiz",
            Description = "Creates a quiz for the specified instructor-owned course.")]
        public async Task<IActionResult> Add(int courseId, [FromBody] CreateQuizCommand command)
        {
            command.CourseId = courseId;
            await mediator.Send(command);

            return Created();
        }
        [HttpGet]
        [Authorize]
        [Route("/api/courses/{courseId}/quizzes")]
        [SwaggerOperation(
            Summary = "Get course quizzes",
            Description = "Retrieves a paginated list of quizzes belonging to the specified course.")]
        public async Task<IActionResult> GetAll(
            int courseId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {

            var quiz = await mediator.Send(new GetCourseQuizzesQuery
            {
                CourseId = courseId,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });


            return Ok(quiz);
        }
        
        [HttpGet("{quizId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get a quiz by ID",
            Description = "Retrieves the specified quiz for an authorized course participant.")]
        public async Task<IActionResult> GetById(int quizId)
        {
            var quiz = await mediator.Send(new GetQuizByIdQuery(quizId));

            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        [HttpPut("{quizId}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Update a quiz",
            Description = "Updates the specified quiz owned by the authenticated instructor.")]
        public async Task<IActionResult> Update(int quizId, [FromBody] UpdateQuizCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid request.");
            }

            command.Id = quizId;
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{quizId}")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Delete a quiz",
            Description = "Deletes the specified quiz owned by the authenticated instructor.")]
        public async Task<IActionResult> Delete(int quizId)
        {
            await mediator.Send(new DeleteQuizCommand(quizId));
            return NoContent();
        }

        [HttpPost("{quizId}/questions")]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Add a question to a quiz",
            Description = "Adds a question to the specified instructor-owned quiz.")]
        public async Task<IActionResult> AddQuestion(int quizId, [FromBody] AddQuestionCommand command)
        {
            command.QuizId = quizId;
            await mediator.Send(command);
            return Created();
        }

        [Route("/api/questions/{questionId}")]
        [HttpPut]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Update a quiz question",
            Description = "Updates the specified question in an instructor-owned quiz.")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] UpdateQuestionCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid request.");
            }

            command.Id = questionId;
            await mediator.Send(command);
            return NoContent();
        }

        [Route("/api/questions/{questionId}")]
        [HttpDelete]
        [Authorize(Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Delete a quiz question",
            Description = "Deletes the specified question from an instructor-owned quiz.")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            await mediator.Send(new DeleteQuestionCommand(questionId));
            return NoContent();
        }


        [Route("{quizId}/attempts")]
        [HttpPost]
        [Authorize]
        [EnableRateLimiting("user-sensitive")]
        [SwaggerOperation(
            Summary = "Submit a quiz attempt",
            Description = "Submits the authenticated student's answers for the specified quiz.")]
        public async Task<IActionResult> SubmitAttempt([FromBody] SubmitQuizAttemptCommand command, int quizId)
        {
            command.QuizId = quizId;
            await mediator.Send(command);
            return Created();
        }

        [Route("{quizId}/attempts/my")]
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get my quiz attempts",
            Description = "Retrieves a paginated list of the authenticated student's attempts for the specified quiz.")]
        public async Task<IActionResult> GetmyAttempt(
            int quizId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {

            var query = await mediator.Send(new GetMyAttemptsQuery(quizId)
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            if (query == null)
            {
                return NotFound();
            }

            return Ok(query);
        }


        [Route("{quizId}/attempts")]
        [HttpGet]
        [Authorize (Roles = UserRole.Instructor)]
        [SwaggerOperation(
            Summary = "Get all quiz attempts",
            Description = "Retrieves a paginated list of student attempts for an instructor-owned quiz.")]
        public async Task<IActionResult> GetAllAttempt(
            int quizId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {

            var query = await mediator.Send(new GetAllAttemptsQuery(quizId)
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            if (query == null)
            {
                return NotFound();
            }

            return Ok(query);
        }
    }
}
