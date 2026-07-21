using Learnova.Application.Courses.Services;
using Learnova.Domain.Entites;
using Microsoft.Extensions.Logging;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Learnova.Infrastructure.Search
{
    public class CourseSearchService(
     QdrantClient qdrant,
     EmbeddingService embeddingService,
     ILogger<CourseSearchService> logger) : ICourseSearchService
    {
        private const string CollectionName = "courses";

        public async Task<List<int>> SearchAsync(
            string searchTerm,
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || limit <= 0)
                return [];

            var embedding = await embeddingService.GenerateEmbeddingAsync(
                searchTerm.Trim(),
                cancellationToken);

            var results = await qdrant.SearchAsync(
                CollectionName,
                embedding,
                limit: (ulong)limit,
                cancellationToken: cancellationToken);

            return results
                .Where(point => point.Id.HasNum)
                .Select(point => (int)point.Id.Num)
                .ToList();
        }

        public async Task IndexCourseAsync(
            Course course,
            CancellationToken cancellationToken = default)
        {
            if (course is null)
                throw new ArgumentNullException(nameof(course));

            var text = string.Join(" ",
                course.Title,
                course.Description,
                course.Level.ToString(),
                course.Language);

            try
            {
                var embedding = await embeddingService.GenerateEmbeddingAsync(
                    text,
                    cancellationToken);

                var point = new PointStruct
                {
                    Id = (ulong)course.Id,
                    Vectors = embedding
                };

                point.Payload.Add("courseId", course.Id);
                point.Payload.Add("title", course.Title ?? string.Empty);
                point.Payload.Add("level", course.Level.ToString());
                point.Payload.Add("language", course.Language ?? string.Empty);

                await qdrant.UpsertAsync(
                    CollectionName,
                    new[] { point },
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to index course {CourseId}", course.Id);
                throw;
            }
        }

        public async Task DeleteCourseAsync(int courseId, CancellationToken cancellationToken = default)
        {
            try
            {
                await qdrant.DeleteAsync(
                    CollectionName,
                    (ulong)courseId,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Failed to remove course {CourseId} from search index.", courseId);
            }
        }
    }
}
