using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizTools.Kahoot;

namespace QuizTools.Kahoot.Exceptions
{
    class NicknameIsTooLongException : Exception
    {
        public string Nickname { get; }
        public NicknameIsTooLongException(string nickname) : base($"Nickname '{nickname}' is too long! Nickname lenght shouldn't be longer than {KahootConstants.MAX_NICKNAME_LENGHT} symbols")
        {
            Nickname = nickname;
        }
    }
}
