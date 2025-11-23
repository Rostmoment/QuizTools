using QuizTools.Kahoot.QuestionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootAnswer
    {
        private KahootAnswer(BaseKahootQuestion question)
        {
            this.Question = question;
            this.Points = question.MaxPoints;
        }
        public KahootAnswer(BaseKahootQuestion question, float x, float y) : this(question, new Vector2(x, y)) { }
        public KahootAnswer(BaseKahootQuestion question, Vector2? answer) : this(question)
        {
            this.xy = answer;
        }
        public KahootAnswer(BaseKahootQuestion question, string answer) : this(question)
        {
            this.Input = answer;
        }
        public KahootAnswer(BaseKahootQuestion question, int value) : this(question)
        {
            this.Value = value;
        }
        public KahootAnswer(BaseKahootQuestion question, params int[] answers) : this(question)
        {
            this.answers = answers;
        }

        public BaseKahootQuestion Question { get; }

        /// <summary>
        /// Used for <see cref="KahootPinQuestion"/>
        /// </summary>
        public Vector2? XY => xy;
        private Vector2? xy;

        /// <summary>
        /// Used for <see cref="KahootJumbleQuestion"/> and <see cref=".ahootChoicesQuestion"/>
        /// </summary>
        public int[]? Answers => answers?.ToArray();
        private int[]? answers;

        /// <summary>
        /// Used for <see cref="KahootSliderQuestion"/> and <see cref="KahootScaleQuestion"/>
        /// </summary>
        public int? Value { get; }

        /// <summary>
        /// Used for <see cref="KahootInputTextQuestion"/>
        /// </summary>
        public string? Input { get; }


        private int points;
        /// <summary>
        /// Points that you will get for answering question correctly
        /// Value should be 0 or not less than min not zero and not bigger than max points to question
        /// Setting new value automatically recalculates <see cref="KahootAnswer.ReactionTime"/>
        /// </summary>
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
        /// <summary>
        /// How much time you spend on question in milliseconds
        /// Should not be negative or more than max time
        /// Setting new value automatically recalculates <see cref="KahootAnswer.Points"/>
        /// </summary>
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
