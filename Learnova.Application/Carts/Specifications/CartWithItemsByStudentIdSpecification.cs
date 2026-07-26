using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Application.Carts.Specifications
{
    public class CartWithItemsByStudentIdSpecification : BaseSpecification<Cart>
    {
        public CartWithItemsByStudentIdSpecification(string studentId)
            : base(c => c.StudentId == studentId)
        {
            AddInclude(q => q
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Course));
        }
    }
}
