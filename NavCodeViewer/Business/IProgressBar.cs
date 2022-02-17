using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NavCodeViewer
{

    public enum ProgressBarDisplayText
    {
        Percentage,
        CustomText
    }
    public class ProgressBarInfo
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }


        //public class CustomProgressBar : ToolStripProgressBar
        //{
        //    //[DllImportAttribute("uxtheme.dll")]
        //    //private static extern int SetWindowTheme(IntPtr hWnd, string appname, string idlist);


        //    //Property to set to decide whether to print a % or Text
        //    public ProgressBarDisplayText DisplayStyle1 { get; set; }

        //    //Property to hold the custom text
        //    public String CustomText { get; set; }

        //    public CustomProgressBar()
        //    {
        //        // Modify the ControlStyles flags
        //        //http://msdn.microsoft.com/en-us/library/system.windows.forms.controlstyles.aspx
        //        //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        //        //this.ProgressBar.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        //    }
        //    //protected override void OnHandleCreated(EventArgs e)
        //    //{
        //    //    SetWindowTheme(this.ProgressBar.Handle, "", "");
        //    //    base.OnHandleCreated(e);
        //    //}
        //    protected override void OnPaint(PaintEventArgs e)
        //    {
        //        Rectangle rect =this.ProgressBar.ClientRectangle;
        //        Graphics g = e.Graphics;

        //        ProgressBarRenderer.DrawHorizontalBar(g, rect);
        //        rect.Inflate(-3, -3);
        //        if (Value > 0)
        //        {
        //            // As we doing this ourselves we need to draw the chunks on the progress bar
        //            Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
        //            ProgressBarRenderer.DrawHorizontalChunks(g, clip);
        //        }

        //        // Set the Display text (Either a % amount or our custom text
        //        string text = DisplayStyle1 == ProgressBarDisplayText.Percentage ? Value.ToString() + '%' : CustomText;


        //        using (Font f = new Font(FontFamily.GenericSerif, 10))
        //        {

        //            SizeF len = g.MeasureString(text, f);
        //            // Calculate the location of the text (the middle of progress bar)
        //            // Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));
        //            Point location = new Point(Convert.ToInt32((Width / 2) - len.Width / 2), Convert.ToInt32((Height / 2) - len.Height / 2));
        //            // The commented-out code will centre the text into the highlighted area only. This will centre the text regardless of the highlighted area.
        //            // Draw the custom text
        //            g.DrawString(text, f, Brushes.Red, location);
        //        }
        //    }
        //}
    }

