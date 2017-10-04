using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace Fsi.Osumimas.Sudoku {

	public class LoadForm : Form {
	
		private static readonly Regex REGEX_LINE;
		private static readonly string SAMPLE;
		private static readonly string DEFAULT;
		
		static LoadForm() {
			REGEX_LINE = new Regex("^[1-9_]{9}$");
			StringBuilder sbSample = new StringBuilder();
			for(int i = 0 ; i <= Table.CELL_COUNT ; i++) {
				if(i > 0) sbSample.Append("\r\n");
				for(int j = 0 ; j <= Table.CELL_COUNT ; j++) {
					sbSample.Append("_");
				}
			}
			SAMPLE = sbSample.ToString();
			StringBuilder sbDefault = new StringBuilder();
			for(int i = 0 ; i < Table.CELL_COUNT ; i++) {
				if(i > 0) sbDefault.Append("\r\n");
				for(int j = 0 ; j < Table.CELL_COUNT ; j++) {
					sbDefault.Append("_");
				}
			}
			DEFAULT = sbDefault.ToString();
		}
		
		private MainForm mainForm;
		private TextBox importText;
		private Button loadButton;
		private Button clearButton;
		
		public LoadForm(MainForm mainForm, string[] lines) {
			this.mainForm = mainForm;
			
			this.Text = "load";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			
			this.importText = InitImportText(lines);
			this.Controls.Add(this.importText);
			
			this.loadButton = InitLoadButton();
			this.Controls.Add(this.loadButton);
			
			this.clearButton = InitClearButton();
			this.Controls.Add(this.clearButton);
			
			this.Size = Sudoku.FormSize(this.clearButton, this.clearButton);
		}
		
		private TextBox InitImportText(string[] lines) {
			TextBox importText = new TextBox();
			importText.Multiline = true;
			importText.AcceptsReturn = true;
			importText.Font = new Font("‚l‚r ƒSƒVƒbƒN", 12);
			importText.Size = TextRenderer.MeasureText(SAMPLE, importText.Font);
			importText.Location = Sudoku.Location(null, null);
			StringBuilder sb = new StringBuilder();
			foreach(string line in lines) {
				if(sb.Length >= 1) {
					sb.Append("\r\n");
				}
				sb.Append(line);
			}
			importText.Text = sb.ToString();
			return importText;
		}
		
		private Button InitLoadButton() {
			Button loadButton = new Button();
			loadButton.Text = "load";
			loadButton.Location = Sudoku.Location(null, this.importText);
			loadButton.Size = Sudoku.ButtonSize(loadButton);
			loadButton.Click += ClickLoadButton;
			return loadButton;
		}
		
		private void ClickLoadButton(object sender, EventArgs e) {
			string[] lines = this.importText.Text.Replace("\r\n","\n").Split('\n');
			if(lines.Length != Table.CELL_COUNT) {
				MessageBox.Show(String.Format("value must be {0} lines.", Table.CELL_COUNT));
				return;
			}
			for(int i = 0 ; i < lines.Length ; i++) {
				if(!REGEX_LINE.Match(lines[i]).Success) {
					MessageBox.Show(String.Format("line[{0}] is invalid.\n'{1}'", i, lines[i]));
					return;
				}
			}
			this.mainForm.Import(lines);
			Dispose();
		}
		
		private Button InitClearButton() {
			Button clearButton = new Button();
			clearButton.Text = "clear";
			clearButton.Location = Sudoku.Location(this.loadButton, this.importText);
			clearButton.Size = Sudoku.ButtonSize(clearButton);
			clearButton.Click += (sender, e) => { this.importText.Text = DEFAULT; };
			return clearButton;
		}
	}
}
