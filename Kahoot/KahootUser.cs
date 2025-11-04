using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootUser
    {
        public KahootUser(JsonElement json) : this(json.GetProperty("creator_username").GetString(), json.GetProperty("creator").GetString(), json.GetProperty("creator_primary_usage").GetString()) { }
        public KahootUser(string name, string id, string primaryUsage)
        {
            Name = name; 
            Id = id; 
            PrimaryUsage = primaryUsage;
        }

        public string Name { get; }
        public string Id { get; }
        public string PrimaryUsage { get; }
        public string Link => string.Format(KahootConstants.URL_USER_PROFILE, Id);

        public override string ToString() => $"{Name} [{PrimaryUsage}]";
    }
}
