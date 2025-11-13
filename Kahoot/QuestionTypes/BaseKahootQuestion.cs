using QuizTools.GeneralUtils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot.QuestionTypes
{
    class BaseKahootQuestion
    {
        #region initialization
        public BaseKahootQuestion() { }
        public BaseKahootQuestion(JsonElement jSON, KahootGame game)
        {
            Game = game;
            Type = KahootQuestionTypeExtensions.ToQuestionType(jSON.GetProperty("type").GetString());
            Title = WebUtility.HtmlDecode(jSON.GetProperty("question").GetString());

            Image = ImageMetaData.FromJSON(jSON);

            Time = jSON.GetProperty("time").GetInt32();

            GivesPoints = Type.GivesPoints() && jSON.GetBooleanOrDefault("points", true);

            PointsMultiplier = jSON.GetInt32OrDefault("pointsMultiplier", 1);
            JSON = jSON;
        }
        public int Index => Array.IndexOf(Game.Questions, this);
        public KahootGame Game { get; }
        public KahootQuestionType Type { get; }
        public string Title { get; }
        public ImageMetaData Image { get; }

        public int Time { get; }
        public int SecondsTime => Time / 1000;


        public bool GivesPoints { get; }
        public int PointsMultiplier { get; }
        public virtual int MaxPoints => GivesPoints ? 1000 * PointsMultiplier : 0;
        public virtual int MinNotZeroPoints => MaxPoints / 2;

        public JsonElement JSON { get; }

        #endregion
        public bool AnswerCorrect(KahootChallenge challenge, KahootPlayer player, float accuracy = 1) => AnswerCorrect(challenge, player, new HttpClient(), accuracy);
        public bool AnswerCorrect(KahootChallenge challenge, KahootPlayer player, HttpClient client, float accuracy = 1) => Task.Run(() => AnswerCorrectAsync(challenge, player, client, accuracy)).GetAwaiter().GetResult();

        public async Task<bool> AnswerCorrAnswerCorrectAsyncect(KahootChallenge challenge, KahootPlayer player, float accuracy = 1) => await AnswerCorrectAsync(challenge, player, new HttpClient(), accuracy);
        public virtual async Task<bool> AnswerCorrectAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, float accuracy = 1)
        {
            if (!challenge.Questions.Contains(this))
                throw new ArgumentException("The question does not belong to the specified challenge", nameof(challenge));

            ArgumentNullException.ThrowIfNull(challenge, nameof(challenge));
            ArgumentNullException.ThrowIfNull(player, nameof(player));
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            if (accuracy < 0 || accuracy > 1 || float.IsNaN(accuracy))
                throw new ArgumentOutOfRangeException(nameof(accuracy), "Accuracy must be between 0 and 1");

            return false;
        }

        public virtual void WriteAnswers()
        {

        }


        public int CalculatePoints(float accuracy)
        {
            int result = (int)(MaxPoints * accuracy) + Program.RNG.Next(0, 10) * Program.RNG.RandomSign();
            result = Math.Clamp(result, 0, MaxPoints);
            return result;
        }
        public int CalculateReactionTime(int points, KahootChallenge challenge)
        {
            if (!challenge.GameOptions.QuestionTimer)
                return 0;

            if (MaxPoints == 0)
                return Math.Max(Time - Program.RNG.Next(100, 200), 0);

            int result = 2 * Time - (2 * points * Time / MaxPoints);
            if (result > Time)
                return Math.Max(Time - Program.RNG.Next(100, 200), 0);
            return Math.Max(result, 0);
        }
    }
}
