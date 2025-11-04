using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.Exceptions
{
    class KahootIsFullException : Exception
    {
        public string Game { get; }

        public KahootIsFullException(string game) : base($"Kahoot {game} is full!")
        {
            Game = game;
        }
    }
}
