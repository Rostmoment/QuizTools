using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Exceptions;

namespace QuizTools.Kahoot.Games
{
    class KahootChallenge : KahootGame
    {
        private static readonly Dictionary<string, Func<string, string, Exception>> errorMap = new()
        {
            [KahootConstants.NICKNAME_EXISTS_ERROR] = (nick, id) => new NicknameExistsException(nick),
            [KahootConstants.MAX_PLAYERS_ERROR] = (nick, id) => new KahootIsFullException(id),
            [KahootConstants.NOT_FOUND_ERROR] = (nick, id) => new KahootNotFoundException(id),
            [KahootConstants.NICKNAME_WITH_PROFANITY_ERROR] = (nick, id) => new NicknameWithProfanityException(nick)
        };


        public KahootChallenge(JsonElement root) : base(KahootGame.Get(root.GetProperty("quizId").GetString()).JSON)
        {
            Title = root.GetProperty("title").GetString();
            ChallengeID = root.GetProperty("challengeId").GetString();
            Pin = root.GetProperty("pin").GetString();
            StartTime = DatetimeUtils.FromUnixTime(root.GetProperty("startTime").GetInt64());
            EndTime = DatetimeUtils.FromUnixTime(root.GetProperty("endTime").GetInt64());
            MaxPlayers = root.GetProperty("maxPlayers").GetInt32();

            JsonElement host = root.GetProperty("quizMaster");
            Host = new KahootUser(host.GetProperty("username").GetString(), host.GetProperty("uuid").GetString(), host.GetProperty("primary_usage").GetString());

            GameOptions = KahootGameOptions.FromJSON(root);
        }
        public string Title { get; }
        public string ChallengeID { get; }
        public string Pin { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public int MaxPlayers { get; }
        public KahootGameOptions GameOptions { get; }
        public KahootUser Host { get; }
      
        public string ChallengeIDJSONLink => string.Format(KahootConstants.URL_CHALLENGE, ChallengeID);
        public string PinJSONLink => string.Format(KahootConstants.URL_CHALLENGE_PIN, Pin);
        public string JoinLink => string.Format(KahootConstants.URL_JOIN_CHALLENGE, Pin, ChallengeID);
        public string AnswersLink => string.Format(KahootConstants.URL_ANSWERS, ChallengeID);

        #region create methods
        public static new async Task<KahootChallenge> GetAsync(string data)
        {
            if (int.TryParse(data, out _))
                return await FromPinAsync(data);
            else if (data.Contains('.'))
                return await FromUrlAsync(data);
            else
                return await FromIDAsync(data);
        }
        public static new KahootChallenge Get(string data)
        {
            if (int.TryParse(data, out _))
                return FromPin(data);
            else if (data.Contains('.'))
                return FromUrl(data);
            else
                return FromID(data);
        }

        private static async Task<KahootChallenge> FromPinAsync(string pin)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_CHALLENGE_PIN, pin));
            HttpResponseMessage response = httpClient.Send(request);
            using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("error", out JsonElement error))
            {
                if (error.GetString() == KahootConstants.NOT_FOUND_ERROR)
                    throw new KahootNotFoundException(pin);
                else
                    throw new Exception($"Unknown error {error.GetString()}");

            }
            return await FromIDAsync(root.GetProperty("challenge").GetProperty("challengeId").GetString());
        }
        private static KahootChallenge FromPin(string pin)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_CHALLENGE_PIN, pin));
            HttpResponseMessage response = httpClient.Send(request);
            using JsonDocument document = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("error", out JsonElement error))
            {
                if (error.GetString() == KahootConstants.NOT_FOUND_ERROR)
                    throw new KahootNotFoundException(pin);
                else
                    throw new Exception($"Unknown error {error.GetString()}");
                
            }
            return FromID(root.GetProperty("challenge").GetProperty("challengeId").GetString());
        }

        private static async Task<KahootChallenge> FromIDAsync(string id)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_CHALLENGE, id));
            HttpResponseMessage response = httpClient.Send(request);
            try
            {
                return FromJSON(await response.Content.ReadAsStringAsync());
            }
            catch (KahootNotFoundException)
            {
                throw new KahootNotFoundException(id);
            }
        }
        private static KahootChallenge FromID(string id)
        {
            using HttpClient httpClient = new();
            using HttpRequestMessage request = new(HttpMethod.Get, string.Format(KahootConstants.URL_CHALLENGE, id));
            HttpResponseMessage response = httpClient.Send(request);
            try
            {
                return FromJSON(response.Content.ReadAsStringAsync().Result);
            }
            catch (KahootNotFoundException)
            {
                throw new KahootNotFoundException(id);
            }
        }

        private static async Task<KahootChallenge> FromUrlAsync(string url) => await FromIDAsync(url.Split($"{KahootConstants.CHALLENGE_ID_IN_LINK}=")[1]);
        private static KahootChallenge FromUrl(string url) => FromID(url.Split($"{KahootConstants.CHALLENGE_ID_IN_LINK}=")[1]);

        private static KahootChallenge FromJSON(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("error", out JsonElement error))
            {
                if (error.GetString() == KahootConstants.NOT_FOUND_ERROR)
                    throw new KahootNotFoundException("");
                else
                    throw new Exception($"Unknown error {error.GetString()}");
            }
            KahootGame quiz = KahootGame.Get(root.GetProperty("quizId").GetString());
            List<string> players = new();
            JsonElement.ArrayEnumerator array = root.GetProperty("challengeUsersList").EnumerateArray();
            while (array.MoveNext())
                players.Add(array.Current.GetProperty("nickname").GetString());

            return new KahootChallenge(root);
        }

        #endregion

        #region join
        public KahootPlayer Join(string nickname) => Join(nickname, new HttpClient());
        public KahootPlayer Join(string nickname, HttpClient client)
        {
            if (nickname.Length > KahootConstants.MAX_NICKNAME_LENGHT)
                throw new NicknameIsTooLongException(nickname);

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format(KahootConstants.URL_JOIN_KAHOOT_BY_API, ChallengeID, nickname));
            HttpResponseMessage response = client.Send(request);
            JsonElement json = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result).RootElement;
            if (json.TryGetProperty("error", out JsonElement element))
            {
                string error = element.GetString();

                if (errorMap.TryGetValue(error, out var func))
                    throw func(nickname, ChallengeID);
                else
                    throw new Exception($"Unknown error {error}");
            }
            return new KahootPlayer(nickname, json.GetProperty("playerCid").GetInt64(), this);
        }


        public async Task<KahootPlayer> JoinAsync(string nickname) => await JoinAsync(nickname, new HttpClient());
        public async Task<KahootPlayer> JoinAsync(string nickname, HttpClient client)
        {
            if (nickname.Length > KahootConstants.MAX_NICKNAME_LENGHT)
                throw new NicknameIsTooLongException(nickname);

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format(KahootConstants.URL_JOIN_KAHOOT_BY_API, ChallengeID, nickname));
            HttpResponseMessage response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            JsonElement json = JsonDocument.Parse(result).RootElement;
            if (json.TryGetProperty("error", out JsonElement element))
            {
                string error = element.GetString();

                if (errorMap.TryGetValue(error, out var func))
                    throw func(nickname, ChallengeID);
                else
                    throw new Exception($"Unknown error {error}");
            }
            return new KahootPlayer(nickname, json.GetProperty("playerCid").GetInt64(), this);
        }

        public KahootPlayer[] GetPlayers(HttpClient client = null)
        {
            List<KahootPlayer> players = new(MaxPlayers);
            client ??= new HttpClient();

            using HttpRequestMessage request = new(HttpMethod.Get, AnswersLink);
            HttpResponseMessage response = client.Send(request);
            using JsonDocument document = JsonDocument.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("challenge", out JsonElement challenge) && challenge.TryGetProperty("challengeUsersList", out JsonElement list))
            {
                foreach (JsonElement player in list.EnumerateArray())
                {
                    players.Add(new KahootPlayer(
                        player.GetProperty("nickname").GetString(),
                        player.GetProperty("playerCId").GetInt64(),
                        this));
                }
            }
            return players.ToArray();
        }

        public async Task<KahootPlayer[]> GetPlayersAsync(HttpClient client = null)
        {
            List<KahootPlayer> players = new(MaxPlayers);
            client ??= new HttpClient();

            using HttpRequestMessage request = new(HttpMethod.Get, AnswersLink);
            HttpResponseMessage response = await client.SendAsync(request);
            using JsonDocument document = JsonDocument.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("challenge", out JsonElement challenge) && challenge.TryGetProperty("challengeUsersList", out JsonElement list))
            {
                foreach (JsonElement player in list.EnumerateArray())
                {
                    players.Add(new KahootPlayer(
                        player.GetProperty("nickname").GetString(),
                        player.GetProperty("playerCId").GetInt64(),
                        this));
                }
            }
            return players.ToArray();
        }

        #endregion
    }
}
