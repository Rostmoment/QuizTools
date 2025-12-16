using QuizTools.GeneralUtils;
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
    abstract class BaseKahootQuestion
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
        #endregion

        #region Properties
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
        public KahootAnswer CorrectAnswer => correctAnswer != null ? new KahootAnswer(correctAnswer) : null;
        protected KahootAnswer correctAnswer;

        #endregion

        public bool Answer(KahootChallenge challenge, KahootPlayer player, KahootAnswer answer) => Answer(challenge, player, new HttpClient(), answer);
        public bool Answer(KahootChallenge challenge, KahootPlayer player, HttpClient client, KahootAnswer answer) => Task.Run(() => AnswerAsync(challenge, player, client, answer)).GetAwaiter().GetResult();

        public async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, KahootAnswer answer) => await AnswerAsync(challenge, player, new HttpClient(), answer);
        public virtual async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, KahootAnswer answer)
        {
            if (!challenge.Questions.Contains(this))
                throw new ArgumentException("The question does not belong to the specified challenge", nameof(challenge));

            ArgumentNullException.ThrowIfNull(challenge, nameof(challenge));
            ArgumentNullException.ThrowIfNull(player, nameof(player));
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(answer, nameof(answer));

            return false;
        }

        public abstract void WriteAnswers();
    }
}
