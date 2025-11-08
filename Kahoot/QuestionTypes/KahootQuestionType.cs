using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.QuestionTypes
{
    public enum KahootQuestionType
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
    public static class KahootQuestionTypeExtensions
    {
        private static KahootQuestionType[] givesPoints = [KahootQuestionType.Quiz, KahootQuestionType.MultipleAnswersQuiz, KahootQuestionType.PinIt, 
            KahootQuestionType.Slider, KahootQuestionType.OpenEnded, KahootQuestionType.Jumble];

        private static Dictionary<string, KahootQuestionType> stringToType = new()
        {
            { "quiz", KahootQuestionType.Quiz },
            { "multiple_select_quiz", KahootQuestionType.MultipleAnswersQuiz },
            { "pin_it", KahootQuestionType.PinIt },
            { "scale", KahootQuestionType.Scale },
            { "nps", KahootQuestionType.NPS },
            { "slider", KahootQuestionType.Slider },
            { "drop_pin", KahootQuestionType.DropPin },
            { "survey", KahootQuestionType.Poll },
            { "multiple_select_poll", KahootQuestionType.MultipleAnswersPoll },
            { "open_ended", KahootQuestionType.OpenEnded },
            { "brainstorming", KahootQuestionType.Brainstorming },
            { "content", KahootQuestionType.Content },
            { "word_cloud", KahootQuestionType.WordCloud },
            { "jumble", KahootQuestionType.Jumble },
            { "feedback", KahootQuestionType.Feedback },
            { "title", KahootQuestionType.Title }
        };

        private static Dictionary<KahootQuestionType, string> typeToString = stringToType.ToDictionary(x => x.Value, x => x.Key);

        public static bool GivesPoints(this KahootQuestionType type) => givesPoints.Contains(type);
        public static KahootQuestionType ToQuestionType(this string str)
        {
            if (stringToType.TryGetValue(str, out KahootQuestionType type))
                return type;
            Logger.WriteWarningLine($"Unknown type of question: {str}");
            return KahootQuestionType.Unknown;
        }

        public static string ToJsonString(this KahootQuestionType type)
        {
            if (typeToString.TryGetValue(type, out string str)) 
                return str;
            return "Unknown";
        }
    }
}
