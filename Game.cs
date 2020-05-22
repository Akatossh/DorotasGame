using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorotas_Game_Server_v_0._3
{
    public class Game
    {
        public int AmountOfPlayers;
        public int amountOfquestions;
        public int amountOfAnswers;
        public string[] questions = new string[64];
        public string[] answers = new string[1024];
        public List<string> questionsList = new List<string>();
        public List<string> ansewrsList = new List<string>();


        public void addingQuestions(string questions)
        {
            int index = questions.IndexOf(";");
            int i = 0;

            while (index > 0)
            {
                this.questions[this.amountOfquestions] = questions.Substring(0, index);
                questionsList.Add(this.questions[this.amountOfquestions]);
                questions = questions.Substring(index + 1, (questions.Length - this.questions[this.amountOfquestions].Length) - 1);
                index = questions.IndexOf(";");

                i++;
                this.amountOfquestions++;
            }
            //this.amountOfquestions++;
            //this.questions[this.amountOfquestions] = questions.Substring(0, questions.Length);


        }

        public void removingQuestion(int index)
        {
            if(questionsList.Count!=0)
            {
                this.questionsList.RemoveAt(index);
                this.amountOfquestions--;
            }
            else
            {

            }
        }

        public void removingAnswer(int index)
        {
            if (ansewrsList.Count != 0)
            {
                this.ansewrsList.RemoveAt(index);
                this.amountOfAnswers--;
            }
            else
            {

            }
        }

        public void addingAnswers(string answers)
        {
            int index = answers.IndexOf(";");
            int i = 0;

            while (index > 0)
            {
                this.answers[this.amountOfAnswers] = answers.Substring(0, index);
                ansewrsList.Add(this.answers[this.amountOfAnswers]);
                answers = answers.Substring(index + 1, (answers.Length - this.answers[this.amountOfAnswers].Length) - 1);
                index = answers.IndexOf(";");

                i++;
                this.amountOfAnswers++;
            }
            //this.amountOfAnswers++;
            //this.answers[this.amountOfAnswers] = answers.Substring(0, answers.Length);


        }


        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string endGame()
        {
            if((ansewrsList.Count == 0 )||( questionsList.Count == 0))
            {
                return "END";
            }
            else
                return null;
        }

        public void ReRoll(string msg)
        {
            ansewrsList.Add(msg);
            amountOfAnswers++;
        }

    }



}
