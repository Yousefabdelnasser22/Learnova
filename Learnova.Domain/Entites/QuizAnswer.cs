using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entites
{
    public class QuizAnswer:BaseEntity
    {
        public int QuizQuestionId { get; set; } 
        public int ChosenAnswerIndex { get; set; } 
        public bool IsCorrect { get; set; } 
    }
}
