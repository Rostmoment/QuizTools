using QuizTools.Kahoot.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootQuestionsFabric
    {
        public static BaseKahootQuestion FromJSON(JsonElement json, KahootGame game)
        {
            string type = json.GetProperty("type").GetString();
            KahootQuestionType enumType = KahootQuestionTypeExtensions.ToQuestionType(type);
            switch (enumType)
            {
                case KahootQuestionType.Unknown:
                    Logger.WriteWarningLine($"Unknown type {type}");
                    return new KahootUnknownQuestion(json);
                case KahootQuestionType.Quiz or KahootQuestionType.MultipleAnswersQuiz
                or KahootQuestionType.Poll or KahootQuestionType.MultipleAnswersPoll:
                    return new KahootChoicesQuestion(json, game);
                case KahootQuestionType.OpenEnded or KahootQuestionType.Feedback or KahootQuestionType.WordCloud:
                    return new KahootInputTextQuestion(json, game);
                case KahootQuestionType.Slider:
                    return new KahootSliderQuestion(json, game);
                case KahootQuestionType.Title or KahootQuestionType.Content:
                    return new KahootUnknownQuestion(json); // It's empty, it doesn't need own type
                case KahootQuestionType.PinIt or KahootQuestionType.DropPin:
                    return new KahootPinQuestion(json, game);
                case KahootQuestionType.NPS or KahootQuestionType.Scale:
                    return new KahootScaleQuestion(json, game);
                case KahootQuestionType.Jumble:
                    return new KahootJumbleQuestion(json, game);
                default:
                    Logger.WriteWarningLine($"Type {enumType} is not implented");
                    return new KahootUnknownQuestion(json);
            }
        }
        public static BaseKahootQuestion[] ArrayFromJSON(JsonElement.ArrayEnumerator json, KahootGame game)
        {
            List<BaseKahootQuestion> result = new List<BaseKahootQuestion>();
            while (json.MoveNext())
            {
                result.Add(FromJSON(json.Current, game));
            }
            return result.ToArray();
        }

    }
}
