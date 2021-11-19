using System.Drawing;
using System.Windows.Forms;

namespace MakeTheCheck
{
    public class MyButton : Button
    {
        public MyButton()
        {
            UseVisualStyleBackColor = false;
            TextImageRelation = TextImageRelation.ImageAboveText;
        }
        public override string Text
        {
            get { return ""; }
            set { base.Text = value; }
        }
        public string TitleText { get; set; }
        public string DescText { get; set; }
        public string PriceText { get; set; }
        public object Item { get; set; }

        public Font TitleFont = new Font(FontFamily.Families[5], 18, FontStyle.Bold);
        public Font DescFont = new Font(DefaultFont.FontFamily, 13, FontStyle.Regular);
        public Font PriceFont = new Font(DefaultFont.FontFamily, 12, FontStyle.Regular);
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Rectangle rect = ClientRectangle;
            Rectangle TitleRect = new Rectangle(new Point(rect.X + Size.Width / 3, rect.Y + 10), new Size(2 * rect.Width / 3 - 10, 30));
            Rectangle DescRect = new Rectangle(new Point(TitleRect.X, TitleRect.Y + TitleRect.Height + 10), new Size(TitleRect.Width, 210));
            Rectangle PriceRect = new Rectangle(new Point(DescRect.X, DescRect.Y + DescRect.Height + 10), new Size(DescRect.Width, rect.Height - (TitleRect.Height + DescRect.Height + 40)));
            rect.Inflate(-5, -5);
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
            {
                using (Brush brush = new SolidBrush(ForeColor))
                {
                    pevent.Graphics.DrawString(TitleText, TitleFont, brush, TitleRect, sf);
                    pevent.Graphics.DrawString(DescText, DescFont, brush, DescRect, sf);
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Far;
                    pevent.Graphics.DrawString(PriceText, PriceFont, brush, PriceRect, sf);
                }
            }
        }
    }
}
