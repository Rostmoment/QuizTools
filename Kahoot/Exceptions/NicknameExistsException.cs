using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.Exceptions
{
    class NicknameExistsException : Exception
    {
        public string Nickname { get; }

        public NicknameExistsException(string nickname) : base($"Nickname {nickname} is already exists in kahoot!")
        {
            Nickname = nickname;
        }
    }
}
