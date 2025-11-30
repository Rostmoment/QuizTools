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
        /// <summary>
        /// Creates copy of instance
        /// </summary>
        /// <param name="original"></param>
        public KahootAnswer(KahootAnswer original)
        {
            this.IsCheated = original.IsCheated;
            this.almostCorrect = original.almostCorrect;
            this.Question = original.Question;  
            this.Points = original.Points;
            this.ReactionTime = original.ReactionTime;
            this.xy = original.xy;
            this.value = original.value;
            this.answers = original.answers;
            this.inputs = original.inputs;
        }
        private KahootAnswer(BaseKahootQuestion question)
        {
            this.Question = question;
            this.Points = question.MaxPoints;
            this.almostCorrect = false;
        }
        #region pin question
        /// <summary>
        /// Used for <see cref="KahootPinQuestion"/>
        /// </summary>
        /// <param name="question">Instance of question</param>
        /// <param name="x">X where pin should be dropped</param>
        /// <param name="y">Y where pin should be dropped</param>
        public KahootAnswer(BaseKahootQuestion question, float x, float y) : this(question, new Vector2(x, y)) { }
        /// <summary>
        /// Used for <see cref="KahootPinQuestion"/>
        /// </summary>
        /// <param name="question">Instance of question</param>
        /// <param name="answer">Coordinates of where pin should be dropped</param>
        public KahootAnswer(BaseKahootQuestion question, Vector2? answer) : this(question)
        {
            this.xy = answer;
        }
        #endregion
        #region questions with text input
        /// <summary>
        /// Used for <see cref="KahootInputTextQuestion"/>
        /// </summary>
        /// <param name="question">Instance of question</param>
        /// <param name="answer">Array of answers. Will use first element from array if question needs only 1 answer</param>
        public KahootAnswer(BaseKahootQuestion question, params string[] inputs) : this(question)
        {
            this.inputs = inputs;
        }
        #endregion
        #region questions where answer is number
        /// <summary>
        /// Used for <see cref="KahootSliderQuestion"/> and <see cref="KahootScaleQuestion"/>
        /// </summary>
        /// <param name="question">Instance of question</param>
        /// <param name="value">Number that needs to be sent as answer</param>
        public KahootAnswer(BaseKahootQuestion question, int value) : this(question)
        {
            this.value = value;
        }
        #endregion
        #region questions where answer is order or choices
        /// <summary>
        /// Used for <see cref="KahootChoicesQuestion"/> and <see cref="KahootJumbleQuestion"/>
        /// If question is <see cref="KahootChoicesQuestion"/>, answers at indexes from <see cref="KahootAnswer.Answers"/> will be used
        /// If question is <see cref="KahootJumbleQuestion"/>, order of answers from <see cref="KahootAnswer.Answers"/> will be used (order starts from 0)
        /// </summary>
        /// <param name="question">Instance of question</param>
        /// <param name="answers">Indexes of answers</param>
        public KahootAnswer(BaseKahootQuestion question, params int[] answers) : this(question)
        {
            this.answers = answers;
        }
        #endregion

        public BaseKahootQuestion Question { get; }

        /// <summary>
        /// Used for <see cref="KahootPinQuestion"/>
        /// </summary>
        public Vector2? XY => xy;
        private Vector2? xy;

        /// <summary>
        /// Used for <see cref="KahootChoicesQuestion"/> and <see cref="KahootJumbleQuestion"/>
        /// </summary>
        public int[]? Answers => answers?.ToArray();
        private int[]? answers;

        /// <summary>
        /// Used for <see cref="KahootSliderQuestion"/> and <see cref="KahootScaleQuestion"/>
        /// </summary>
        public int? Value => value;
        private int? value;

        /// <summary>
        /// Used for <see cref="KahootInputTextQuestion"/>
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

        public bool AlmostCorrect
        {
            get => almostCorrect;
            set
            {
                if (!IsCheated)
                    throw new InvalidOperationException("You can change if answer is correct in cheated mode");
                almostCorrect = value;
            }
        }
        private bool almostCorrect;

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
    }
}
