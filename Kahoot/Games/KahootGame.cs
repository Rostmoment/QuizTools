using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Exceptions;
using QuizTools.Kahoot.QuestionTypes;

namespace QuizTools.Kahoot.Games
{
    class KahootGame
    {
        #region read only fields

        public string Name { get; }
        public string QuizID { get; }
        public string Language { get; }

        public DateTime CreatedTime { get; }
        public DateTime ModifiedTime { get; }

        public BaseKahootQuestion[] Questions { get; }
        public KahootUser Creator { get; }

        public int NumberOfQuestions => Questions.Length;
        public int TotalMaxPoints => Questions.Sum(q => q.MaxPoints);

        public string QuizJSONLink => string.Format(KahootConstants.URL_QUIZ_ID, QuizID);
        public string DetailsLink => string.Format(KahootConstants.URL_KAHOOT_DETAILS, QuizID);

        public JsonElement JSON { get; }
        #endregion

        public KahootGame(JsonElement root)
        {
            Name = root.GetProperty("title").GetString();
            QuizID = root.GetProperty("uuid").GetString();
            Language = root.GetProperty("language").GetString();
            CreatedTime = DatetimeUtils.FromUnixTime(root.GetProperty("created").GetInt64());
            ModifiedTime = DatetimeUtils.FromUnixTime(root.GetProperty("modified").GetInt64());
            Questions = KahootQuestionsFabric.ArrayFromJSON(root.GetProperty("questions").EnumerateArray(), this);
            Creator = new KahootUser(root);

            JSON = root;
        }

        #region create quiz from input
        public static async Task<KahootGame> GetAsync(string input)
        {
            if (int.TryParse(input, out _) || input.Contains('_'))
                return await KahootChallenge.GetAsync(input);
            else if (input.Contains('.'))
                return await FromURLAsync(input);
            else
                return await FromIDAsync(input);
        }
        public static KahootGame Get(string input)
        {
            if (int.TryParse(input, out _) || input.Contains('_'))
                return KahootChallenge.Get(input);
            else if (input.Contains('.'))
                return FromURL(input);
            else
                return FromID(input);
        }

        private static async Task<KahootGame> FromURLAsync(string url)
        {
            string quizId = KahootURLExtractors.QuizIDFromURL(url);
            try
            {
                return await FromIDAsync(quizId);
            }
            catch (KahootNotFoundException)
            {
                throw new KahootNotFoundException(url);
            }
        }
        private static KahootGame FromURL(string url)
        {
            if (url.Contains(KahootConstants.CHALLENGE_ID_IN_LINK))
                return KahootChallenge.Get(url);

            string quizId = KahootURLExtractors.QuizIDFromURL(url);
            try
            {
                return FromID(quizId);
            }
            catch (KahootNotFoundException)
            {
                throw new KahootNotFoundException(url);
            }
        }

        private static async Task<KahootGame> FromIDAsync(string quizId)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_QUIZ_ID, quizId));
            HttpResponseMessage response = httpClient.Send(request);
            try
            {
                return FromJSON(await response.Content.ReadAsStringAsync());
            }
            catch (KahootNotFoundException)
            {
                throw new KahootNotFoundException(quizId);
            }
        }
        private static KahootGame FromID(string quizId)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_QUIZ_ID, quizId));
            HttpResponseMessage response = httpClient.Send(request);
            try
            {
                return FromJSON(response.Content.ReadAsStringAsync().Result);
            }
            catch (KahootNotFoundException)
            {
                throw new KahootNotFoundException(quizId);
            }
        }

        private static KahootGame FromJSON(string json)
        {
            JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("error", out JsonElement error))
            {
                if (error.GetString() == KahootConstants.NOT_FOUND_ERROR)
                    throw new KahootNotFoundException("");
                else
                    throw new Exception($"Unknown error {error.GetString()}");
                
            }
            return new KahootGame(root);
        }
        #endregion

        public KahootChallenge StartChallenge(string bearerToken, DateTime endTime, HttpClient client = null) => StartChallengeAsync(bearerToken, endTime, client).Result;
        public async Task<KahootChallenge> StartChallengeAsync(string bearerToken, DateTime endTime, HttpClient client = null)
        {
            client ??= new HttpClient();
            var payload = new
            {
                endTime = endTime.ToUnixTimeMilliseconds(),
                quizId = QuizID,
                game_options = new
                {
                    participant_id = false,
                    participant_id_placeholder = "",
                    premium_branding = false,
                    randomize_questions = false,
                    randomize_answers = true,
                    smart_practice = false,
                    question_timer = false,
                    namerator = false,
                    hide_lobby_list = false,
                    hide_scoreboard = false,
                    hide_podium = false,
                    anonymous_mode = false,
                    child_safe_open_ended_question_format = false,
                    login_required = false,
                    nano_format = false
                }
            };
            string json = JsonSerializer.Serialize(payload);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, KahootConstants.URL_CHALLENGE_NO_ARGUMENT);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            HttpResponseMessage response = await client.SendAsync(request);
            JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return await KahootChallenge.GetAsync(document.RootElement.GetProperty("challengeId").GetString());
        }
    }
}
