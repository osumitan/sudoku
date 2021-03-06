using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Fsi.Osumimas.Sudoku {

	public class Sudoku {
	
		public const int MARGIN = 5;
		private static readonly MainForm mainForm;
		
		static Sudoku() {
			try {
				mainForm = new MainForm();
			} catch(Exception e) {
				StreamWriter writer = new StreamWriter("@error.txt", true);
				writer.WriteLine(e);
				writer.Close();
			}
		}
		
		public static void Main(string[] args) {
			try {
				Application.Run(mainForm);
			} catch(Exception e) {
				StreamWriter writer = new StreamWriter("@error.txt", true);
				writer.WriteLine(e);
				writer.Close();
			}
		}
		
		public static void Log(string format, params object[] prms) {
			if(mainForm != null) mainForm.Log(format, prms);
		}
		
		public static int Right(Control c) {
			return c == null ? 0 : c.Location.X + c.Width;
		}
		
		public static int Bottom(Control c) {
			return c == null ? 0 : c.Location.Y + c.Height;
		}
		
		public static int Neighbor(Control c) {
			return Right(c) + MARGIN;
		}
		
		public static int Under(Control c) {
			return Bottom(c) + MARGIN;
		}
		
		public static Point Location(Control xc, Control yc) {
			return new Point(Neighbor(xc), Under(yc));
		}
		
		public static Size FormSize(Form form) {
			int w = 0;
			int h = 0;
			foreach(Control c in form.Controls) {
				w = Math.Max(w, Neighbor(c) + MARGIN);
				h = Math.Max(h, SystemInformation.CaptionHeight + Under(c) + MARGIN);
			}
			return new Size(w, h);
		}
		
		public static Size ButtonSize(Button button) {
			return new Size(TextRenderer.MeasureText(" " + button.Text + " ", button.Font).Width, button.Height);
		}
	}
}
