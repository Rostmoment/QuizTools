using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.Exceptions
{
    class NicknameWithProfanityException : Exception
    {
        public string Nickname { get; }
        public NicknameWithProfanityException(string nickname) : base($"Nickname {nickname} contains profanity")
        {
            Nickname = nickname;
        }
    }
}
