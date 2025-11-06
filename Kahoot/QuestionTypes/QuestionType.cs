using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.QuestionTypes
{
    public enum QuestionType
    {

        Unknown,
        Quiz, // choices.correct
        MultipleAnswersQuiz, // choices.correct
        PinIt, // choiceShapes.x and choiceShapes.y
        Scale, // no solution, doesn't give points
        NPS, // no solution, doesn't give points
        Slider, // choiceRange.correct
        DropPin, // PinIt, but doesn't give points
        OpenEnded, // choices.answer
        Poll, // doesn't give points
        MultipleAnswersPoll, // doesn't give points
        Brainstorming, // no solution, doesn't give points
        Content, // just a powerpoint slide, no solution, doesn't give points
        WordCloud, // no solution, doesn't give points
        Jumble, // choices is correct order
        Feedback, // no solution, doesn't give points
        Title // Same as content
    }
    public static class QuestionTypeExtensions
    {
        private static QuestionType[] givesPoints = [QuestionType.Quiz, QuestionType.MultipleAnswersQuiz, QuestionType.PinIt, 
            QuestionType.Slider, QuestionType.OpenEnded, QuestionType.Jumble];

        private static Dictionary<string, QuestionType> stringToType = new()
        {
            { "quiz", QuestionType.Quiz },
            { "multiple_select_quiz", QuestionType.MultipleAnswersQuiz },
            { "pin_it", QuestionType.PinIt },
            { "scale", QuestionType.Scale },
            { "nps", QuestionType.NPS },
            { "slider", QuestionType.Slider },
            { "drop_pin", QuestionType.DropPin },
            { "survey", QuestionType.Poll },
            { "multiple_select_poll", QuestionType.MultipleAnswersPoll },
            { "open_ended", QuestionType.OpenEnded },
            { "brainstorming", QuestionType.Brainstorming },
            { "content", QuestionType.Content },
            { "word_cloud", QuestionType.WordCloud },
            { "jumble", QuestionType.Jumble },
            { "feedback", QuestionType.Feedback },
            { "title", QuestionType.Title }
        };

        private static Dictionary<QuestionType, string> typeToString = stringToType.ToDictionary(x => x.Value, x => x.Key);

        public static bool GivesPoints(this QuestionType type) => givesPoints.Contains(type);
        public static QuestionType ToQuestionType(this string str)
        {
            if (stringToType.TryGetValue(str, out QuestionType type))
                return type;
            Logger.WriteWarningLine($"Unknown type of question: {str}");
            return QuestionType.Unknown;
        }

        public static string ToJsonString(this QuestionType type)
        {
            if (typeToString.TryGetValue(type, out string str)) 
                return str;
            return "Unknown";
        }
    }
}
