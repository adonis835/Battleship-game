using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExamsFirstApp
{
    public class Ship
    {
        public int Size { get; set; }
        public bool Horizontal { get; set; }
        public Point StartPos { get; set; }
        public List<PictureBox> EnemyBoxes { get; set; }
        public List<PictureBox> PlayerBoxes { get; set; }
        public Ship(int size, Point startpos, bool horizontal)
        {
            this.Size = size;
            this.StartPos = startpos;
            this.Horizontal = horizontal;
            EnemyBoxes = new List<PictureBox>();
            PlayerBoxes = new List<PictureBox>();
        }
    }
}
