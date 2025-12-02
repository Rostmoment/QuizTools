using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;

namespace QuizTools.Kahoot
{
    class KahootGameOptions
    {
        public bool RandomizeAnswers { get; }
        public bool HideLobbyList { get; }
        public bool HidePlayerRank { get; }
        public bool HidePodium { get; }
        public bool HideScoreboard { get; }
        public bool QuestionTimer { get; }
        public bool ParticipantId { get; }
        public bool SmartPractice { get; }
        public bool PremiumBranding { get; }
        public bool SendWrongAnswers { get; }
        public bool LoginRequired { get; }
        public bool GhostMode { get; }
        public bool Namerator { get; }
        public int ScoringVersion { get; }
        public bool GroupChallenge { get; }  
        public bool RequiresPinCode { get; }
        public bool OnboardingChallenge { get; }
        public bool ChildSafe { get; }
        public bool AnonymousMode { get; }

        public JsonElement JSON { get; }


        private KahootGameOptions(bool randomizeAnswers, bool hideLobbyList, bool hidePlayerRank, bool hidePodium, bool hideScoreboard,
                bool questionTimer, bool participantId, bool smartPractice, bool premiumBranding, bool sendWrongAnswers,
                bool loginRequired, bool ghostMode, bool namerator, int scoringVersion, bool groupChallenge,
                bool requiresPinCode, bool onboardingChallenge, bool childSafe, JsonElement json)
        {
            RandomizeAnswers = randomizeAnswers;
            HideLobbyList = hideLobbyList;
            HidePlayerRank = hidePlayerRank;
            HidePodium = hidePodium;
            HideScoreboard = hideScoreboard;
            QuestionTimer = questionTimer;
            ParticipantId = participantId;
            SmartPractice = smartPractice;
            PremiumBranding = premiumBranding;
            SendWrongAnswers = sendWrongAnswers;
            LoginRequired = loginRequired;
            GhostMode = ghostMode;
            Namerator = namerator;
            ScoringVersion = scoringVersion;
            GroupChallenge = groupChallenge;
            RequiresPinCode = requiresPinCode;
            OnboardingChallenge = onboardingChallenge;
            ChildSafe = childSafe;

            AnonymousMode = true;
            if (json.TryGetProperty("anonymous_mode", out JsonElement anon))
            {
                if (anon.ValueKind == JsonValueKind.False)
                    AnonymousMode = false;
            }

            JSON = json;
        }


        public static KahootGameOptions FromJSON(JsonElement root)
        {
            if (!root.TryGetProperty("game_options", out JsonElement element))
                return null;

            try
            {
                return new(
                    element.GetProperty("randomize_answers").GetBoolean(),
                    element.GetProperty("hide_lobby_list").GetBoolean(),
                    element.GetProperty("hide_player_rank").GetBoolean(),
                    element.GetProperty("hide_podium").GetBoolean(),
                    element.GetProperty("hide_scoreboard").GetBoolean(),
                    element.GetProperty("question_timer").GetBoolean(),
                    element.GetProperty("participant_id").GetBoolean(),
                    element.GetProperty("smart_practice").GetBoolean(),
                    element.GetProperty("premium_branding").GetBoolean(),
                    element.GetProperty("send_players_incorrect_text_answers").GetBoolean(),
                    element.GetProperty("login_required").GetBoolean(),
                    element.GetProperty("ghost_mode").GetBoolean(),
                    element.GetProperty("namerator").GetBoolean(),
                    element.GetProperty("scoring_version").GetInt32(),
                    element.GetProperty("group_challenge").GetBoolean(),
                    element.GetProperty("require_pin_code").GetBoolean(),
                    element.GetProperty("onboarding_challenge").GetBoolean(),
                    element.GetProperty("child_safe_open_ended_question_format").GetBoolean(),
                    element
                );
            }
            catch (Exception e)
            {
                Logger.WriteErrorLine(e);
                return null;
            }
        }

    }
}
