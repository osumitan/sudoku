using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace Fsi.Osumimas.Sudoku {

	public class LoadForm : Form {
	
		private MainForm mainForm;
		private Table table;
		private TextBox importText;
		private Button loadButton;
		private Button clearButton;
		
		public LoadForm(MainForm mainForm, Table table) {
			this.mainForm = mainForm;
			this.table = table;
			
			this.Text = "load";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			
			this.importText = InitImportText(this.table.Export());
			this.Controls.Add(this.importText);
			
			this.loadButton = InitLoadButton();
			this.Controls.Add(this.loadButton);
			
			this.clearButton = InitClearButton();
			this.Controls.Add(this.clearButton);
			
			this.Size = Sudoku.FormSize(this);
		}
		
		private TextBox InitImportText(string[] lines) {
			TextBox importText = new TextBox();
			importText.Multiline = true;
			importText.AcceptsReturn = true;
			importText.Font = new Font("‚l‚r ƒSƒVƒbƒN", 12);
			importText.Size = TextRenderer.MeasureText(this.table.Dimension.LoadSample(), importText.Font);
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
			if(lines.Length != this.table.Dimension.CellCount()) {
				MessageBox.Show(String.Format("value must be {0} lines.", this.table.Dimension.CellCount()));
				return;
			}
			for(int i = 0 ; i < lines.Length ; i++) {
				if(!this.table.Dimension.LoadRegexLine().Match(lines[i]).Success) {
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
			clearButton.Click += (sender, e) => { this.importText.Text = this.table.Dimension.LoadDefault(); };
			return clearButton;
		}
	}
}
