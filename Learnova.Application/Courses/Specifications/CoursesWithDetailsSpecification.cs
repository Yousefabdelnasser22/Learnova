using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Courses.Specifications
{
    public class CoursesWithDetailsSpecification : BaseSpecification<Course>
    {
        public CoursesWithDetailsSpecification()
        {
            AddCourseIncludes();
        }

        public CoursesWithDetailsSpecification(
            int pageNumber,
            int pageSize,
            string? search,
            int? categoryId,
            int? subCategoryId,
            decimal? minPrice,
            decimal? maxPrice,
            CourseLevel? level,
            string? sort)
            : base(c =>
                c.Status == CourseStatus.Published &&
                (string.IsNullOrWhiteSpace(search) ||
                    c.Title.Contains(search) ||
                    (c.Description != null && c.Description.Contains(search)) ||
                    c.SubCategory.Name.Contains(search) ||
                    c.SubCategory.Category.Name.Contains(search)) &&
                (!categoryId.HasValue || c.SubCategory.CategoryId == categoryId.Value) &&
                (!subCategoryId.HasValue || c.SubCategoryId == subCategoryId.Value) &&
                (!minPrice.HasValue || c.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || c.Price <= maxPrice.Value) &&
                (!level.HasValue || c.Level == level.Value))
        {
            AddCourseIncludes();
            ApplySort(sort);

            var skip = (pageNumber - 1) * pageSize;
            ApplyPagination(skip, pageSize);
        }

        public CoursesWithDetailsSpecification(int id)
            : base(c => c.Id == id && c.Status == CourseStatus.Published)
        {
            AddCourseIncludes();
        }

        public CoursesWithDetailsSpecification(int id, bool includeUnpublished)
            : base(c => c.Id == id && (includeUnpublished || c.Status == CourseStatus.Published))
        {
            AddCourseIncludes();
        }

        private void AddCourseIncludes()
        {
            AddInclude(c => c.Instructor);
            AddInclude(c => c.SubCategory);
            AddInclude(c => c.SubCategory.Category);
        }

        private void ApplySort(string? sort)
        {
            switch (sort?.Trim().ToLowerInvariant())
            {
                case "title_desc":
                    AddOrderByDescending(c => c.Title);
                    break;
                case "price":
                    AddOrderBy(c => c.Price);
                    break;
                case "price_desc":
                    AddOrderByDescending(c => c.Price);
                    break;
                case "newest":
                    AddOrderByDescending(c => c.CreatedAt);
                    break;
                case "oldest":
                    AddOrderBy(c => c.CreatedAt);
                    break;
                default:
                    AddOrderBy(c => c.Title);
                    break;
            }
        }
    }
}
