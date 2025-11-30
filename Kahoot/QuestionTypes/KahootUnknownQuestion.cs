using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootUnknownQuestion : BaseKahootQuestion
    {
        public JsonElement JSON { get; }
        public KahootUnknownQuestion(JsonElement jSON) : base()
        {
            JSON = jSON;
        }
    }
}
