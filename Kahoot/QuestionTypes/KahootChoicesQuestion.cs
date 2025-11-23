using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot.QuestionTypes
{
    class KahootChoicesQuestion : BaseKahootQuestion
    {
        public override int MaxPoints
        {
            get
            {
                if (IsMultipleAnswers && !IsPoll)
                    return Choices.Count(c => c.IsCorrect) * 500 * PointsMultiplier;
                return base.MaxPoints;
            }
        }
        public bool IsMultipleAnswers => Type == KahootQuestionType.MultipleAnswersQuiz || Type == KahootQuestionType.MultipleAnswersPoll;
        public bool IsPoll => Type == KahootQuestionType.Poll || Type == KahootQuestionType.MultipleAnswersPoll;
        private List<KahootChoice> choices = new List<KahootChoice>();
        public KahootChoice[] Choices { get; private set; }

        public KahootChoicesQuestion(JsonElement jSON, KahootGame game) : base(jSON, game)
        {
            if (JSON.TryGetProperty("choices", out JsonElement element))
            {
                JsonElement.ArrayEnumerator elements = element.EnumerateArray();
                while (elements.MoveNext())
                {
                    KahootChoice choice = new KahootChoice(WebUtility.HtmlDecode(elements.Current.GetStringOrDefault("answer")), elements.Current.GetProperty("correct").GetBoolean(), ImageMetaData.FromJSON(elements.Current));
                    choices.Add(choice);
                }
            }
            Choices = choices.ToArray();
        }



        public override void WriteAnswers()
        {

            foreach (KahootChoice choice in Choices)
            {
                if (choice.IsCorrect)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" - {choice}");
            }
        }

        public override async Task<bool> AnswerAsync(KahootChallenge challenge, KahootPlayer player, HttpClient client, float accuracy = 1)
        {
            await base.AnswerCorrectAsync(challenge, player, client, accuracy);
            int pointsToGive = CalculatePoints(accuracy);
            int time = CalculateReactionTime(pointsToGive, challenge);
            string json = "";
            
            if (IsMultipleAnswers)
            {
                var payload = new
                {
                    quizId = challenge.QuizID,
                    quizType = "quiz",
                    numQuestions = challenge.NumberOfQuestions,
                    startTime = challenge.StartTime.ToUnixTimeMilliseconds(),
                    question = new
                    {
                        index = Array.IndexOf(challenge.Questions, this),
                        answers = new[]
                        {
                            new
                            {
                                reactionTime = time,
                                playerId = player.Name,
                                playerCid = player.ID,
                                selectedChoices = Choices.IndexesOf(x => x.IsCorrect),
                                isCorrect = true,
                                points = pointsToGive,
                            }
                        }
                    }
                };
                json = JsonSerializer.Serialize(payload);
            }
            else
            {
                var payload = new
                {
                    quizId = challenge.QuizID,
                    quizType = "quiz",
                    numQuestions = challenge.NumberOfQuestions,
                    startTime = challenge.StartTime.ToUnixTimeMilliseconds(),
                    question = new
                    {
                        index = Array.IndexOf(challenge.Questions, this),
                        answers = new[]
                        {
                            new 
                            {
                                reactionTime = time,
                                playerId = player.Name,
                                playerCid = player.ID,
                                choiceIndex = Choices.IndexOf(x => x.IsCorrect),
                                isCorrect = true,
                                points = pointsToGive,
                            }
                        }
                    }
                };
                json = JsonSerializer.Serialize(payload);
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, challenge.AnswersLink);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
