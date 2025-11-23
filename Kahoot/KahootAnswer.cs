using QuizTools.Kahoot.QuestionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootAnswer(BaseKahootQuestion question)
    {
        public BaseKahootQuestion Question { get; } = question;

        /// <summary>
        /// Used for <see cref="KahootPinQuestion"/>
        /// </summary>
        public Vector2? xy;

        /// <summary>
        /// Used for <see cref="KahootJumbleQuestion"/> and <see cref=".ahootChoicesQuestion"/>
        /// </summary>
        public int[]? answers;

        /// <summary>
        /// Used for <see cref="KahootSliderQuestion"/> and <see cref="KahootScaleQuestion"/>
        /// </summary>
        public int? value; // Used for scale

        /// <summary>
        /// Used for <see cref="KahootInputTextQuestion"/>
        /// </summary>
        public string? input;

        private int points;
        public int Points
        {
            get => points;
            set
            {
                if (!Question.Type.GivesPoints())
                {
                    points = 0;
                    reactionTime = 0;
                    return;
                }

                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Question.MaxPoints, nameof(value));
                if (Question.MinNotZeroPoints > value && value != 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                reactionTime = 2 * Question.Time * ((Question.MaxPoints - value) /  Question.MaxPoints);
                points = value;
            }
        }

        private int reactionTime;
        public int ReactionTime 
        {

            get => reactionTime;
            set
            {
                if (!Question.Type.GivesPoints())
                {
                    points = 0;
                    reactionTime = 0;
                    return;
                }

                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Question.Time, nameof(value));

                points = Question.MaxPoints - ((Question.MaxPoints * value) / (2 * Question.Time));
                reactionTime = value;
            }
        }
    }
}
