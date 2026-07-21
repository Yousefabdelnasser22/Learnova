using Learnova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Favorites.DTO
{
    public class FavoriteCourseDto
    {
        public int FavoriteId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public CourseLevel Level { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
