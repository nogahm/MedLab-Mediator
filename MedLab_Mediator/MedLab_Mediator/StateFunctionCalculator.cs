using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLab_Mediator
{
    enum func { plus, minus, mult, div}
    class StateFunctionCalculator
    {
        List<DataPoint> DB1;
        List<DataPoint> DB2;
        func function;

        //constructor
        public StateFunctionCalculator(List<DataPoint> DB1, List<DataPoint> DB2, func function)
        {
            this.DB1 = DB1;
            this.DB2 = DB2;
            this.function = function;
        }

        //calculate
        public StringBuilder calculateFunction()
        {
            //List<IntersectionPeriod> ans=new List<IntersectionPeriod>();
            StringBuilder sb = new StringBuilder();

            DB1.Sort((x, y) => DateTime.Compare(x.StartTime, y.StartTime));
            DB2.Sort((x, y) => DateTime.Compare(x.EndTime, y.EndTime));

            int j = 0;
            for(int i=0 ; i<DB1.Count ; i++)
            {
                DataPoint d1 = DB1.ElementAt(i);
                DataPoint d2 = DB2.ElementAt(j);

                //go throw DB2 starting from j

                //find the first item that can be overlap
                while ((j < DB2.Count-1) && (DateTime.Compare(d1.StartTime, d2.EndTime) > 0))
                {
                    if (j < (DB2.Count-1))
                    {
                        j++;
                        d2 = DB2.ElementAt(j);
                    }
                }

                int index = j;
                while(index < DB2.Count && (DateTime.Compare(d2.StartTime, d1.EndTime) < 0))
                {
                    //there is an overlap - case 1: s1,s2.e1,e2 (or same times)
                    if ((DateTime.Compare(d1.StartTime, d2.StartTime)<=0) && (DateTime.Compare(d1.EndTime, d2.EndTime)<=0) && (DateTime.Compare(d1.EndTime, d2.StartTime) > 0))
                    {
                        IntersectionPeriod ip = new IntersectionPeriod(d2.StartTime, d1.EndTime, function, d1, d2);
                        sb.AppendLine(ip.ToString());
                    }
                    //case 2: s1,s2,e2,e1
                    else if ((DateTime.Compare(d1.StartTime, d2.StartTime) <= 0) && (DateTime.Compare(d1.EndTime, d2.EndTime) >= 0))
                    {
                        IntersectionPeriod ip = new IntersectionPeriod(d2.StartTime, d2.EndTime, function, d1, d2);
                        sb.AppendLine(ip.ToString());
                    }
                    //case 3: s2,s1,e2,e1
                    else if ((DateTime.Compare(d1.StartTime, d2.StartTime) >= 0) && (DateTime.Compare(d1.EndTime, d2.EndTime) >= 0))
                    {
                        IntersectionPeriod ip = new IntersectionPeriod(d1.StartTime, d2.EndTime, function, d1, d2);
                        sb.AppendLine(ip.ToString());
                    }
                    //case 4: s2,s1,e1,e2
                    else if ((DateTime.Compare(d1.StartTime, d2.StartTime) >= 0) && (DateTime.Compare(d1.EndTime, d2.EndTime) <= 0))
                    {
                        IntersectionPeriod ip = new IntersectionPeriod(d1.StartTime, d1.EndTime, function, d1, d2);
                        sb.AppendLine(ip.ToString());
                    }
                    index++;
                    if(index < DB2.Count)
                        d2 = DB2.ElementAt(index);

                }
            }

            return sb;
        }
    }

    class IntersectionPeriod
    {
        DateTime start;
        DateTime end;
        double value=0;

        public IntersectionPeriod(DateTime start, DateTime end, func function, DataPoint d1, DataPoint d2)
        {
            //find start and end of the overlap
            this.start = start;
            this.end = end;

            //calculate new value
            switch(function)
            {
                case func.plus:
                    {
                        value = d1.getValue() + d2.getValue();
                        break;
                    }
                case func.minus:
                    {
                        value = d1.getValue() - d2.getValue();
                        break;
                    }
                case func.mult:
                    {
                        value = d1.getValue() * d2.getValue();
                        break;
                    }
                case func.div:
                    {
                        value = d1.getValue() / d2.getValue();
                        break;
                    }
            }
        }

        //toString
        public override string ToString()
        {
            return ("Start: " + start.ToString() + " End: " + end.ToString() + " Value Calculated: " + value);
        }
    }
}
