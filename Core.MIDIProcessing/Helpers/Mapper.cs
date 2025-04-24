using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MIDIProcessing.Helpers
{
    public class Mapper
    {
        public static double MapToRange(int velocity, double minValue, double maxValue)
        {
            return minValue + velocity / 127.0 * (maxValue - minValue);
        }
    }
}
