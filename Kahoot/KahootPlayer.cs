using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Games;

namespace QuizTools.Kahoot
{
    class KahootPlayer
    {
        private string name;
        private long id;
        private KahootChallenge challenge;

        public string Name => name;
        public long ID => id;
        public KahootChallenge Challenge => challenge;


        public KahootPlayer(string name, long id, KahootChallenge challenge)
        {
            this.name = name;
            this.id = id;
            this.challenge = challenge;
        }

        public override string ToString() => $"{Name} [{ID}]";
        
    }
}
