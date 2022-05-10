using System.Collections;
using System.Collections.Generic;
using System;

namespace RCG
{
    public class FisherYatesRandomizer
    {
        public string Question { get; set; } = "";
        public List<string> Choices { get; set; } = new List<string>();
        public List<int> Questions { get; set; } = new List<int>();
        public string Answer { get; set; } = "";

        public FisherYatesRandomizer() { }
        public FisherYatesRandomizer(string question, List<string> choices, string answer)
        {
            Question = question;
            Choices = choices;
            Answer = answer;
        }

        public FisherYatesRandomizer(List<int> questions)
        {
            Questions = questions;
        }

        public void ShuffleChoices()
        {
            // Creating a object
            // for Random class
            Random r = new Random();

            // Start from the last element and
            // swap one by one. We don't need to
            // run for the first element
            // that's why i > 0
            for (int i = Choices.Count - 1; i > 0; i--)
            {

                // Pick a random index
                // from 0 to i
                int j = r.Next(0, i + 1);

                // Swap arr[i] with the
                // element at random index
                string temp = Choices[i];
                Choices[i] = Choices[j];
                Choices[j] = temp;
            }
        }
    }
}

