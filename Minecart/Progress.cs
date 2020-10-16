using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecart
{
    public class Progress
    {
        public delegate void ProgressStepHandler();
        public event ProgressStepHandler OnStep;

        public readonly int maxValue;
        public readonly int minValue;
        public int value { get; private set; }
        public Progress(int maxValue)
        {
            minValue = 0;
            this.maxValue = maxValue;
        }
        public void Step() { value++; OnStep?.Invoke(); }
    }
}
