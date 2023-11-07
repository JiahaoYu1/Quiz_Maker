using System;
using System.Collections.Generic;
using System.Text;

namespace PIIIProject
{
    public class Results
    {

        public Results(int points, string answer)
        {
            Points = points;
            Answer = answer;
        }
        
        public int Points { get; set; }
        public string Answer { get; set; }


    }
}
