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
            this.isCorrect = true;
        }
        public KahootAnswer(BaseKahootQuestion question, float x, float y) : this(question, new Vector2(x, y)) { }
        public KahootAnswer(BaseKahootQuestion question, Vector2? answer) : this(question)
        {
            this.xy = answer;
        }
        public KahootAnswer(BaseKahootQuestion question, params string[] answer) : this(question)
        {
            this.inputs = answer;
        }
        public KahootAnswer(BaseKahootQuestion question, int value) : this(question)
        {
            this.value = value;
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
        /// Used for <see cref="KahootChoicesQuestion"/>
        /// </summary>
        public int[]? Answers => answers?.ToArray();
        private int[]? answers;

        /// <summary>
        /// Used for <see cref="KahootSliderQuestion"/> and <see cref="KahootScaleQuestion"/>
        /// </summary>
        public int? Value => value;
        private int? value;

        /// <summary>
        /// Used for <see cref="KahootJumbleQuestion"/> and <see cref="KahootInputTextQuestion"/>
        /// </summary>
        public string[]? Inputs => inputs?.ToArray();
        private string[]? inputs;

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
                if (value == 0)
                {
                    if (!IsCheated)
                        isCorrect = false;

                    points = 0;
                    return;
                }
                if (!Question.Type.GivesPoints())
                {
                    if (!IsCheated)
                    {
                        isCorrect = true;
                        points = 0;
                        reactionTime = 0;
                    }
                    return;
                }


                int min = Question.MinNotZeroPoints;
                if (IsCheated)
                    min = 0; // Normally you cannot have points less than min not zero but more than zero, but it's cheated mode, so sure

                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Question.MaxPoints, nameof(value)); // Even if cheated some validation should be done because kahoot doesn't allow to go above max or
                ArgumentOutOfRangeException.ThrowIfLessThan(value, min, nameof(value));

                if (!IsCheated)
                {
                    isCorrect = value != 0;
                    reactionTime = 2 * Question.Time * ((Question.MaxPoints - value) / Question.MaxPoints);
                }

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

                if (!IsCheated)
                {
                    ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
                    ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Question.Time, nameof(value));
                    points = Question.MaxPoints - ((Question.MaxPoints * value) / (2 * Question.Time));
                }

                reactionTime = value;
            }
        }

        public bool IsCorrect
        {
            get => isCorrect;
            set
            {
                if (!IsCheated)
                    throw new InvalidOperationException("You can change if answer is correct in cheated mode");
                isCorrect = value;
            }
        }
        private bool isCorrect;

        /// <summary>
        /// If true, all validations will be removed
        /// </summary>
        public bool IsCheated { get; init; } = false;

        public KahootAnswer Copy()
        {
            KahootAnswer result = new KahootAnswer(Question)
            {
                isCorrect = this.isCorrect,
                IsCheated = this.IsCheated,
                points = this.points,
                reactionTime = this.reactionTime,
                xy = this.xy,
                value = this.value,
                answers = this.answers,
                inputs = this.inputs
            };
            return result;
        }
    }
}
