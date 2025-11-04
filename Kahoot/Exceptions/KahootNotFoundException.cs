using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot.Exceptions
{
    [Serializable]
    class KahootNotFoundException : Exception
    {
        public string Game { get; }
        public KahootNotFoundException(string game) : base($"Kahoot {game} was not found!")
        {
            Game = game;
        }
    }
}
