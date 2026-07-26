using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Learnova.Domain.Specifications;

namespace Learnova.Application.Orders.Specifications
{
    public class PaidOrderItemByStudentAndCourseSpecification : BaseSpecification<OrderItem>
    {
        public PaidOrderItemByStudentAndCourseSpecification(
            string studentId,
            int courseId,
            int excludedOrderId)
            : base(x =>
                x.CourseId == courseId &&
                x.OrderId != excludedOrderId &&
                x.Order.StudentId == studentId &&
                x.Order.Status == OrderStatus.Paid)
        {
        }
    }
}
