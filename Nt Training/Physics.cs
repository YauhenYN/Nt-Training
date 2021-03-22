using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nt_Training
{
    public static class Physics
    {
        public static void Wind(ref int degrees, ref double speed, double power, int windDegrees)
        {
            int coef;
            if (degrees > 90) coef = 1; //90 - обратно пропорционально 270
            else coef = -1;
            int commonDegree = Convert.ToInt32(degrees + (coef * (degrees - windDegrees) * (0.01 / 0.2)));
            degrees = commonDegree % 360;
            double coeff;
            if (degrees / 90 == 0)
            {
                coeff = 0;
                coeff += (degrees % 90) * 0.01;
            }
            else if (degrees / 90 == 1)
            {
                coeff = 1;
                coeff -= (degrees % 90) * 0.01;
            }
            else if (degrees / 90 == 2)
            {
                coeff = 0;
                coeff -= (degrees % 90) * 0.01;
            }
            else
            {
                coeff = -1;
                coeff += (degrees % 90) * 0.01;
            }
            speed += coeff * power;
        }
    }
}
