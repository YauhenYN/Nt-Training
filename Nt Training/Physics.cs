using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt_Training
{
    public static class Physics
    {
        public static void Wind(ref int degrees, ref double speed, double power)
        {
            int commonDegree = Convert.ToInt32(degrees * power);
            degrees = commonDegree;
            speed = Convert.ToInt32(speed * (power / 3));
        }
    }
}
