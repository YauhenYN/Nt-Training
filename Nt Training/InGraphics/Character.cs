using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nt_Training.InGraphics
{
    public class Character
    {
        public int speed { get; set; }
        private byte direction;
        Point place;
        public byte directionDegrees { get { return direction; } set { if (value < 90) direction = value; } }
        Color color;
        public Character(Color colorOfCharacher, int startSpeed, Point place)
        {
            direction = 45;
            speed = startSpeed;
            this.place = place;
        }
        public void Spawn()
        {

        }
        public void ToGo()
        {
            
        }
        public void FindObstacle(int Degrees)
        {

        }
    }
}
