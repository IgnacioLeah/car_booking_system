using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CarBookRequest
{
    public class GradientPanel : Panel
    {
        public Color ColorTop { get; set; } = Color.DeepSkyBlue;
        public Color ColorBottom { get; set; } = Color.MediumPurple;
        public int Angle { get; set; } = 90;

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                this.ColorTop,
                this.ColorBottom,
                Angle))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            base.OnPaint(e);
        }
    }
}