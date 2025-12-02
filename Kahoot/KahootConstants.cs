using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootConstants
    {
        public const string URL_QUIZ_ID = "https://create.kahoot.it/rest/kahoots/{0}";
        public const string URL_CHALLENGE = "https://kahoot.it/rest/challenges/{0}";
        public const string URL_CHALLENGE_NO_ARGUMENT = "https://kahoot.it/rest/challenges";
        public const string URL_CHALLENGE_PIN = "https://kahoot.it/rest/challenges/pin/{0}";
        public const string URL_JOIN_KAHOOT_BY_API = "https://kahoot.it/rest/challenges/{0}/join/?nickname={1}";
        public const string URL_PRIVATE_KAHOOT = "https://play.kahoot.it/reserve/session/{0}";
        public const string URL_ANSWERS = "https://kahoot.it/rest/challenges/{0}/answers";

        public const string URL_KAHOOT_DETAILS = "https://create.kahoot.it/details/{0}?drawer=";
        public const string URL_USER_PROFILE = "https://create.kahoot.it/profiles/{0}";
        public const string URL_JOIN_CHALLENGE = "https://kahoot.it/challenge/{0}?challenge-id={1}";

        public const string CHALLENGE_ID_IN_LINK = "challenge-id";
        public const string QUIZ_ID_IN_LINK = "quizId";
        public const string DETAILS_IN_LINK = "details";

        public const string NICKNAME_EXISTS_ERROR = "NICKNAME_EXISTS";
        public const string MAX_PLAYERS_ERROR = "MAX_PLAYERS_REACHED";
        public const string NOT_FOUND_ERROR = "NOT_FOUND";
        public const string NICKNAME_WITH_PROFANITY_ERROR = "NICKNAME_WITH_PROFANITY";

        public const string SESSION_STORAGE = "kahoot-challenge_session";

        public const int MAX_PLAYERS = 10000;
        public const int MIN_NICKNAME_LENGHT = 1;
        public const int MAX_NICKNAME_LENGHT = 50;
        public const int MAX_KAHOOT_PIN = 99_999_999;
    }
}
