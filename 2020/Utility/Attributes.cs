using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2020.Utility
{
    public class Attributes
    {
        [AttributeUsage(AttributeTargets.Class)]
        public class ProblemDate : Attribute
        {
            public int Date;

            public ProblemDate(int date)
            {
                this.Date = date;
            }
        }
    }
}
