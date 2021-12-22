using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventofcode.Utility
{
    public class Attributes
    {
        [AttributeUsage(AttributeTargets.Class)]
        public class ProblemDate : Attribute
        {
            public int Year;
            public int Date;

            public ProblemDate(int year, int date)
            {
                this.Year = year;
                this.Date = date;
            }
        }
    }
}
