using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootURLExtractors
    {
        public static async Task<string> QuizIDFromURLAsync(string url)
        {
            if (url.Contains($"{KahootConstants.QUIZ_ID_IN_LINK}="))
                return url.Split($"{KahootConstants.QUIZ_ID_IN_LINK}=")[1].Split('&')[0];
            else if (url.Contains(KahootConstants.CHALLENGE_ID_IN_LINK))
                return await QuizIDFromChallengeLinkAsync(url.Split($"{KahootConstants.CHALLENGE_ID_IN_LINK}=")[1].Split('&')[0]);
            else if (url.Contains(KahootConstants.DETAILS_IN_LINK))
                return url.Split($"{KahootConstants.DETAILS_IN_LINK}/")[1].Split('&')[0];
            else
                throw new Exception("Invalid URL format!");
        }

        public static string QuizIDFromURL(string url)
        {
            if (url.Contains($"{KahootConstants.QUIZ_ID_IN_LINK}="))
                return url.Split($"{KahootConstants.QUIZ_ID_IN_LINK}=")[1].Split('&')[0];
            else if (url.Contains(KahootConstants.CHALLENGE_ID_IN_LINK))
                return QuizIDFromChallengeLink(url.Split($"{KahootConstants.CHALLENGE_ID_IN_LINK}=")[1].Split('&')[0]);
            else if (url.Contains(KahootConstants.DETAILS_IN_LINK))
                return url.Split($"{KahootConstants.DETAILS_IN_LINK}/")[1].Split('&')[0];
            else
                throw new Exception("Invalid URL format!");
        }

        private static string QuizIDFromChallengeLink(string challengeId)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_CHALLENGE, challengeId));
            HttpResponseMessage response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();
            using JsonDocument document = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
            JsonElement root = document.RootElement;
            return root.GetProperty("quizId").GetString();
        }

        private static async Task<string> QuizIDFromChallengeLinkAsync(string challengeId)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_CHALLENGE, challengeId));
            HttpResponseMessage response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();
            using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement root = document.RootElement;
            return root.GetProperty("quizId").GetString();
        }
    }
}
