using OpenQA.Selenium.DevTools;
using QuizTools.GeneralUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita
{
    class VseosvitaUser
    {
        private readonly HttpClient Client;
        private readonly CookieContainer Cookies;
        public string Name { get; }
        public string CurrentTestID { get; private set; }

        public VseosvitaUser(string name)
        {
            Cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = Cookies,
                AllowAutoRedirect = true,
                UseDefaultCredentials = false
            };

            Client = new HttpClient(handler); 

            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            Client.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");

            Client.DefaultRequestHeaders.Add("Accept-Language", "uk-UA,uk;q=0.9,ru;q=0.8,en;q=0.7");

            Client.DefaultRequestHeaders.Add("sec-ch-ua",
                "\"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\", \"Not=A?Brand\";v=\"99\"");

            Client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            Client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            Client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
            Client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
            Client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            Client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");

            Name = name;
            CurrentTestID = "";
        }
        public void JoinTest(string testID) => JoinTestAsync(testID).GetAwaiter().GetResult();
        public async Task<bool> JoinTestAsync(string testID)
        {
            try
            {
                // 1. Creating session with cookies for the test
                Uri startUri = new Uri(string.Format(VseosvitaConstants.CREATE_SESSION_FOR_TEST_URL, testID));
                HttpResponseMessage startResponse = await Client.GetAsync(startUri);
                startResponse.EnsureSuccessStatusCode();

                // 2. Getting test settings page
                Uri settingsUri = new Uri(string.Format(VseosvitaConstants.START_TEST_URL, testID));
                HttpResponseMessage settingsResponse = await Client.GetAsync(settingsUri);;
                settingsResponse.EnsureSuccessStatusCode();

                // 3. Sending user name to test settings
                FormUrlEncodedContent joinContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("TestDesignerSettings[full_name]", Name)
                });

                HttpRequestMessage joinRequest = new HttpRequestMessage(HttpMethod.Post, settingsUri);
                joinRequest.Content = joinContent;
                joinRequest.Headers.Referrer = settingsUri;
                joinRequest.Headers.Add("Upgrade-Insecure-Requests", "1");

                HttpResponseMessage joinResponse = await Client.SendAsync(joinRequest);
                joinResponse.EnsureSuccessStatusCode();

                // 4. Go top OPL page
                Uri oplUri = new Uri(VseosvitaConstants.GO_OPL_URL);
                HttpRequestMessage oplRequest = new HttpRequestMessage(HttpMethod.Get, oplUri);
                oplRequest.Headers.Referrer = settingsUri;

                HttpResponseMessage oplResponse = await Client.SendAsync(oplRequest);
                oplResponse.EnsureSuccessStatusCode();
                string oplContent = await oplResponse.Content.ReadAsStringAsync();

                // 5. # Extract user_key from OPL page
                string userKey = ExtractUserKey(oplContent);
                if (string.IsNullOrEmpty(userKey))
                {
                    Logger.WriteErrorLine("Failed to extract user_key");
                    return false;
                }

                Logger.WriteInfoLine($"User Key: {userKey}");

                // 6. Joining to test (for some reason it's named active screen)

                Uri activeScreenUri = new Uri(string.Format(VseosvitaConstants.ACTIVE_SCREEN_DATA_URL, userKey));
                HttpRequestMessage activeScreenRequest = new HttpRequestMessage(HttpMethod.Post, activeScreenUri);
                activeScreenRequest.Headers.Referrer = oplUri;
                activeScreenRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

                HttpResponseMessage activeScreenResponse = await Client.SendAsync(activeScreenRequest);
                activeScreenResponse.EnsureSuccessStatusCode();

                // 7. Starting test
                /*
                Uri startExecutionUri = new Uri($"https://vseosvita.ua/ext/test-designer/testing-pupil/start-execution?isAjax=1&isAjaxUrl={WebUtility.UrlEncode("https://vseosvita.ua/test/go-olp")}&user_key={userKey}");
                HttpRequestMessage startExecutionRequest = new HttpRequestMessage(HttpMethod.Post, startExecutionUri);
                startExecutionRequest.Headers.Referrer = oplUri;
                startExecutionRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

                var startExecutionResponse = await Client.SendAsync(startExecutionRequest);
                Logger.WriteInfoLine($"Step 6 - Start execution: {startExecutionResponse.StatusCode}");

                Logger.WriteInfoLine(startExecutionResponse.Content.ReadAsStringAsync().Result);
                */
                CurrentTestID = testID;
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLine($"Error joining to test: {ex.Message}");
                return false;
            }
        }

        private string ExtractUserKey(string content)
        {
            string marker = "user_key";
            int index = content.IndexOf(marker);
            if (index == -1)
                return "";

            string subString = content.Substring(index + marker.Length + 3);
            string userKey = string.Concat(subString.TakeWhile(x => x != '"'));
            return WebUtility.UrlEncode(userKey);
        }
    }
}