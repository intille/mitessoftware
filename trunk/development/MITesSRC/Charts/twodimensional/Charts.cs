using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;
using OpenNETCF.GDIPlus;

namespace Charts.twodimensional
{
    // Base class for drawing a chart
    public abstract class Chart : Control
    {
        public bool DataIsSorted = true;
        public bool DataIsGrouped = true;
        public bool IsStretch = false;

        // The chart data to be drawn to the Graphics 
        public IDictionary Data
        {
            set
            {
                data = new StringInt64PairedArray(value);
                if (DataIsSorted) data.SortValuesDesc();
                if (DataIsGrouped) data.GroupValues(MINIMUM_PIECE,
                                                    SMALLEST_DISPLAY_IN_PERCENT);
            }
        }

        // The constant for grouping data. Grouping data is to join
        // data together that has small size. 
        protected const int MINIMUM_PIECE = 1;
        protected const int SMALLEST_DISPLAY_IN_PERCENT = 2;

        // The constant for drawing the chart
        protected const int CHART_WIDTH_MAX = 200;
        protected const float SPACE_RATIO = 0.05f;
        protected const float LEFT_SECTION_RATIO = 0.55f;

        // The data after being sorted and grouped. 
        protected StringInt64PairedArray data;

        // The variables for drawing the chart
        protected int ChartWidth;
        protected Graphics graphics;
        protected BrushPlus[] brush = 
			   {Brushes.Red, Brushes.Orange, Brushes.Green, Brushes.Blue, 
				Brushes.HotPink, Brushes.Crimson, Brushes.DarkKhaki,
				Brushes.Olive, Brushes.MediumSeaGreen, Brushes.LightCoral,
				Brushes.Silver, Brushes.Chocolate};

        private bool _firstDraw = true;

        public Chart()
        {
            Width = 300;
            Height = 100;
        }

        public void Draw()
        {
            // Make the chart resizes when the contaider resizes.
            if (_firstDraw && Parent != null)
            {
                ((Control)Parent).Resize += new EventHandler(ParentResized);
                _firstDraw = false;
            }

            // Raise the Paint event to draw chart on Graphics
            Invalidate();
        }

        public void ParentResized(Object o, EventArgs e)
        {
            // Raise the Paint event to draw chart on Graphics
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            // Raise the Paint event to draw chart on Graphics
            //            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private string currentActivity = "";
        private int currentActivityHours = 0;
        private int currentActivityMinutes = 0;
        private int currentActivitySeconds = 0;
        private int totalCalories = 0;
        private int currentCalories = 0;

        Bitmap offScreenBmp;
        // This method is a Template Method. It provides a common steps
        // to draw a complete chart
        protected override void OnPaint(PaintEventArgs e)
        {
            if (data != null)
            {
                if (IsStretch)
                {
                    Width = Parent.Width;
                    Height = Parent.Height;
                }


                //Double Buffering
                offScreenBmp = new Bitmap(this.Width, this.Height);

                graphics = Graphics.FromImage(offScreenBmp);
                IntPtr hdc = graphics.GetHdc();
                using (GraphicsPlus g = new GraphicsPlus(hdc))
                {
                    g.SetSmoothingMode(SmoothingMode.SmoothingModeAntiAlias);// = SmoothingMode.AntiAlias;

                }
                graphics.ReleaseHdc(hdc);
                graphics.Clear(Color.White);

                DrawChart();
                //fill the bottom for real-time info
                graphics.FillRectangle(new SolidBrush(Color.Aqua), 0, Height - 120, Parent.Width, 120);
                DrawLegend();
                e.Graphics.DrawImage(offScreenBmp, 0, 0);
                offScreenBmp.Dispose();
            }
        }

        protected virtual void DrawChart() { }

        public void SetActivity(string name)
        {
            currentActivity = name;
        }

        public void SetTime(int hours, int minutes, int seconds)
        {
            this.currentActivityHours = hours;
            this.currentActivityMinutes = minutes;
            this.currentActivitySeconds = seconds;
        }

        public void SetCalories(int totalCalories, int currentCalories)
        {
            this.totalCalories = totalCalories;
            this.currentCalories = currentCalories;
        }

        // Method to draw the chart legend/information
        protected virtual void DrawLegend()
        {
            int X = (int)(SPACE_RATIO * Width) + ChartWidth;
            int Y = 60;

            //Title for the summary
            graphics.DrawString(DateTime.Now.ToString("dddd, MMMM dd, yyyy"), new Font("Arial", 10, global::System.Drawing.FontStyle.Bold | global::System.Drawing.FontStyle.Underline), new SolidBrush(Color.Black), 20, 5);
            graphics.DrawString("Now:", new Font("Arial", 10, global::System.Drawing.FontStyle.Bold), new SolidBrush(Color.Black), 5, Height - 100);
            graphics.DrawString("Calories:", new Font("Arial", 10, global::System.Drawing.FontStyle.Bold), new SolidBrush(Color.Black), 5, Height - 50);

            if (currentActivity.Length > 0)
            {
                graphics.DrawString(currentActivity, new Font("Arial", 14, global::System.Drawing.FontStyle.Bold), new SolidBrush(Color.Black), 90, Height - 110);
                graphics.DrawString(currentActivityHours.ToString("00") + ":" + currentActivityMinutes.ToString("00") + ":" + currentActivitySeconds.ToString("00"), new Font("Arial", 12, global::System.Drawing.FontStyle.Regular), new SolidBrush(Color.Black), (Width / 2) + 80, Height - 110);
            }
            else
            {
                graphics.DrawString("Unknown", new Font("Arial", 14, global::System.Drawing.FontStyle.Bold), new SolidBrush(Color.Black), 90, Height - 110);
                graphics.DrawString("00:00:00", new Font("Arial", 12, global::System.Drawing.FontStyle.Regular), new SolidBrush(Color.Black), (Width / 2) + 80, Height - 110);

            }

            if (this.totalCalories > 0)
            {
                graphics.DrawString(this.totalCalories + " Kcals", new Font("Arial", 12, global::System.Drawing.FontStyle.Regular), new SolidBrush(Color.Black), 130, Height - 55);
            }
            else
            {
                graphics.DrawString("0 Kcals", new Font("Arial", 12, global::System.Drawing.FontStyle.Regular), new SolidBrush(Color.Black), 130, Height - 55);
            }
            float totalPercentage = 0;
            for (int i = 0; i < data.Length; i++)
            {
                //skip empty entries;
                if (data.Keys[i].ToLower().StartsWith("empty"))
                    continue;
                if (data.Values[i] != 0)
                {
                    float percent = data.Values[i] * 100 / (float)data.TotalValue;
                    totalPercentage += percent;
                    string strPercent = percent.ToString("#0.00") + "%";
                    string nameText = (data.Keys[i] != "") ? data.Keys[i] : "Without extension";
                    string legendText = nameText + " (" + strPercent + ")";

                    BrushPlus theBrush = (!data.IsGrouped | i < data.Keys.Length - 1) ?
                        brush[i % brush.Length] : Brushes.Gold;

                    IntPtr hdc = graphics.GetHdc();
                    using (GraphicsPlus g = new GraphicsPlus(hdc))
                    {
                        g.FillRectangle(theBrush, X, Y, 14, 14);
                    }
                    graphics.ReleaseHdc(hdc);
                    graphics.DrawString(legendText, new Font("Arial", 8, global::System.Drawing.FontStyle.Regular), new SolidBrush(Color.Black), X + 20, Y - 5);
                    Y += 20;
                }

            }

            if (totalPercentage < 100)
            {
                float percent = 100 - totalPercentage;
                string strPercent = percent.ToString("#0.00") + "%";
                string nameText = "Empty";
                string legendText = nameText + " (" + strPercent + ")";

                BrushPlus theBrush = Brushes.Gray;
                IntPtr hdc = graphics.GetHdc();
                using (GraphicsPlus g = new GraphicsPlus(hdc))
                {
                    g.FillRectangle(theBrush, X, Y, 14, 14);
                }
                graphics.ReleaseHdc(hdc);
                graphics.DrawString(legendText, new Font("Arial", 8, global::System.Drawing.FontStyle.Regular), new SolidBrush(Color.Black), X + 20, Y - 5);
                Y += 20;
            }

        }
    }


    public class PieChart : Chart
    {
        public int Diameter { get { return _diameter; } set { _diameter = value; } }
        private int _diameter = 100;

        // Method to draw the pie chart 
        protected override void DrawChart()
        {
            // Calculate the size
            int d = (int)Math.Min(LEFT_SECTION_RATIO * Width, 0.9 * Height);
            _diameter = (int)Math.Min(d, CHART_WIDTH_MAX);
            ChartWidth = _diameter;

            int topX = (int)(SPACE_RATIO * Width / 2);
            int topY = (int)((Height - _diameter) / 2);
            int startAngle = -90;
            int sweepAngle = 0;

            //if (data.TotalValue != 100)            
            //  this.Data=nAdd("", 100 - data.TotalValue);            

            // Loop to draw the Pies
            for (int i = 0; i < data.Length; i++)
            {
                BrushPlus theBrush = brush[i % brush.Length];

                if (i < data.Keys.Length - 1)
                    sweepAngle = (int)Math.Round((float)data.Values[i] * 360 / data.TotalValue);
                else
                {
                    sweepAngle = 270 - startAngle;
                    //theBrush = Brushes.Gray;
                    if (data.IsGrouped)
                        theBrush = Brushes.Gold;
                }

                if (data.Keys[i].ToLower().StartsWith("empty"))
                    theBrush = Brushes.Gray;

                IntPtr hdc = graphics.GetHdc();
                using (GraphicsPlus g = new GraphicsPlus(hdc))
                {
                    g.FillPie(theBrush, topX, topY,
                        _diameter, _diameter, startAngle, sweepAngle);
                }
                graphics.ReleaseHdc(hdc);
                startAngle += (int)sweepAngle;
                startAngle = (startAngle >= 360) ? startAngle - 360 : startAngle;
            }
        }
    }


    public class BarChart : Chart
    {
        // Method to draw the bar chart 
        protected override void DrawChart()
        {
            if (data.Length > 0)
            {
                const int BAR_WIDTH = 20;

                // Calculate the size
                int d = (int)Math.Min(LEFT_SECTION_RATIO * this.Width, 0.9 * this.Height);
                int width = (int)Math.Min(d, CHART_WIDTH_MAX);
                int height = (int)Math.Min((int)(0.7f * this.Height), 400);
                int baseX = 10;
                int baseY = (int)((this.Height - height) / 2) + height;

                ChartWidth = width;

                // Draw the axis
                Pen thePen = new Pen(Color.Black, 1.5f);
                graphics.DrawLine(thePen, baseX, baseY, baseX, baseY - height);
                graphics.DrawLine(thePen, baseX, baseY, baseX + width, baseY);

                // Find max value
                long maxValue = 0;
                foreach (long v in data.Values)
                    if (v > maxValue) maxValue = v;

                // Loop to draw the bars
                int w = (int)Math.Min((int)(width / data.Length), BAR_WIDTH);
                for (int i = 0; i < data.Length; i++)
                {
                    BrushPlus theBrush = (!data.IsGrouped | i < data.Keys.Length - 1) ?
                                    brush[i % brush.Length] : Brushes.Gold;

                    int h = (int)Math.Round((float)data.Values[i] * height / maxValue);
                    IntPtr hdc = graphics.GetHdc();
                    using (GraphicsPlus g = new GraphicsPlus(hdc))
                    {
                        g.FillRectangle(theBrush, baseX + (i * w), baseY - h, w, h);
                    }
                    graphics.ReleaseHdc(hdc);
                }
            }
        }
    }
}
