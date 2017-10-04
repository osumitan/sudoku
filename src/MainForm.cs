using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Fsi.Osumimas.Sudoku {

	public class MainForm : Form {
		private const int CELL_WIDTH = 36;
		private const int CELL_HEIGHT = 36;
		private const double REPEAT_INTERVAL = 1000.0;
		
		private Table table;
		private DataGridView grid;
		private Button loadButton;
		private Button nextButton;
		private Button pauseButton;
		private Button repeatButton;
		private TextBox logTextBox;
		private FormStatus status;
		private System.Timers.Timer repeatTimer;
		
		public MainForm() {
			this.Text = "sudoku";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			
			this.repeatTimer = new System.Timers.Timer();
			this.repeatTimer.Elapsed += (sender, e) => { Solve(); };
			this.repeatTimer.Interval = REPEAT_INTERVAL;
			this.repeatTimer.SynchronizingObject = this;
			
			this.grid = InitGrid();
			this.Controls.Add(this.grid);
			this.table = new Table(this.grid);
			
			this.loadButton = InitLoadButton();
			this.Controls.Add(this.loadButton);
			
			this.nextButton = InitNextButton();
			this.Controls.Add(this.nextButton);
			
			this.pauseButton = InitPauseButton();
			this.Controls.Add(this.pauseButton);
			
			this.repeatButton = InitRepeatButton();
			this.Controls.Add(this.repeatButton);
			
			this.logTextBox = InitLogTextBox();
			this.Controls.Add(this.logTextBox);
			
			this.Size = Sudoku.FormSize(this.grid, this.logTextBox);
			
			this.Load += (sender, e) => {
				Clear();
				Sudoku.Log("Initialized");
			};
		}
		
		private DataGridView InitGrid() {
			DataGridView grid = new DataGridView();
			grid.AllowUserToAddRows = false;
			grid.AllowUserToDeleteRows = false;
			grid.AllowUserToOrderColumns = false;
			grid.AllowUserToResizeColumns = false;
			grid.AllowUserToResizeRows = false;
			grid.ReadOnly = true;
			grid.RowHeadersVisible = false;
			grid.ColumnHeadersVisible = false;
			grid.DefaultCellStyle.Font = new Font("Meiryo UI", 16, FontStyle.Bold);
			grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			grid.SelectionChanged += (sender, e) => { this.grid.CurrentCell.Selected = false; };
			grid.CellPainting += (sender, e) => {
				if(e.RowIndex != 0 && e.RowIndex % Table.CELL_COUNT_SQRT == 0) {
					e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.InsetDouble;
				}
				if(e.ColumnIndex != 0 && e.ColumnIndex % Table.CELL_COUNT_SQRT == 0) {
					e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.InsetDouble;
				}
			};
			grid.ScrollBars = ScrollBars.None;
			grid.Location = Sudoku.Location(null, null);
			grid.Width = CELL_WIDTH * Table.CELL_COUNT;
			grid.Height = CELL_HEIGHT * Table.CELL_COUNT;
			for(int i = 0 ; i < Table.CELL_COUNT ; i++) {
				grid.Columns.Add("", "");
				grid.Columns[i].Width = CELL_WIDTH;
			}
			for(int i = 0 ; i < Table.CELL_COUNT ; i++) {
				grid.Rows.Add();
				grid.Rows[i].Height = CELL_HEIGHT;
			}
			return grid;
		}
		
		private Button InitLoadButton() {
			Button loadButton = new Button();
			loadButton.Enabled = false;
			loadButton.Text = "load";
			loadButton.Location = Sudoku.Location(null, this.grid);
			loadButton.Size = Sudoku.ButtonSize(loadButton);
			loadButton.Click += (sender, e) => { new LoadForm(this, this.table.Export()).ShowDialog(); };
			return loadButton;
		}
		
		private Button InitNextButton() {
			Button nextButton = new Button();
			nextButton.Enabled = false;
			nextButton.Text = ">";
			nextButton.Location = Sudoku.Location(this.loadButton, this.grid);
			nextButton.Size = Sudoku.ButtonSize(nextButton);
			nextButton.Click += (sender, e) => { Solve(); };
			return nextButton;
		}
		
		private Button InitPauseButton() {
			Button pauseButton = new Button();
			pauseButton.Enabled = false;
			pauseButton.Text = "||";
			pauseButton.Location = Sudoku.Location(this.nextButton, this.grid);
			pauseButton.Size = Sudoku.ButtonSize(pauseButton);
			pauseButton.Click += (sender, e) => { Status = FormStatus.Available; };
			return pauseButton;
		}
		
		private Button InitRepeatButton() {
			Button repeatButton = new Button();
			repeatButton.Enabled = false;
			repeatButton.Text = ">>";
			repeatButton.Location = Sudoku.Location(this.pauseButton, this.grid);
			repeatButton.Size = Sudoku.ButtonSize(repeatButton);
			repeatButton.Click += (sender, e) => { Status = FormStatus.Repeating; };
			return repeatButton;
		}
		
		private TextBox InitLogTextBox() {
			TextBox logTextBox = new TextBox();
			logTextBox.Multiline = true;
			logTextBox.ScrollBars = ScrollBars.Vertical;
			logTextBox.ReadOnly = true;
			logTextBox.Width = this.grid.Width;
			logTextBox.Height = 100;
			logTextBox.Location = Sudoku.Location(null, this.loadButton);
			return logTextBox;
		}
		
		private void Clear() {
			Status = FormStatus.Unloaded;
		}
		
		public void Import(string[] lines) {
			Clear();
			try {
				this.table.Import(lines,
					cell => { cell.Type = cell.Blank ? CellType.Answer : CellType.Question; },
					cell => { cell.Type = CellType.Error; }
				);
				Status = this.table.Blank ? FormStatus.Unloaded : FormStatus.Available;
			} catch(Exception e) {
				Status = FormStatus.Unloaded;
				Sudoku.Log("Load Error: {0}", e.Message);
			}
			Sudoku.Log("Loaded");
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Solve() {
			try {
				bool canSolve = this.table.Solve();
				if(this.table.Completed) {
					Status = FormStatus.Unloaded;
					Sudoku.Log("Completed");
				} else if(!canSolve) {
					Status = FormStatus.Unloaded;
					Sudoku.Log("Can not solve");
				}
			} catch(Exception ex) {
				Status = FormStatus.Unloaded;
				Sudoku.Log("Next Error: {0}", ex);
			}
		}
		
		private FormStatus Status {
			get { return this.status; }
			set {
				this.status = value;
				if(value == FormStatus.Unloaded) {
					this.loadButton.Enabled = true;
					this.nextButton.Enabled = false;
					this.pauseButton.Enabled = false;
					this.repeatButton.Enabled = false;
					if(this.repeatTimer.Enabled) this.repeatTimer.Stop();
				} else if(value == FormStatus.Available) {
					this.loadButton.Enabled = true;
					this.nextButton.Enabled = true;
					this.pauseButton.Enabled = false;
					this.repeatButton.Enabled = true;
					if(this.repeatTimer.Enabled) this.repeatTimer.Stop();
				} else if(value == FormStatus.Repeating) {
					this.loadButton.Enabled = false;
					this.nextButton.Enabled = false;
					this.pauseButton.Enabled = true;
					this.repeatButton.Enabled = false;
					this.repeatTimer.Start();
				}
			}
		}
		
		public void Log(string format, params object[] prms) {
			this.logTextBox.AppendText(string.Format(format, prms) + "\r\n");
		}
	}
	
	public enum FormStatus { Unloaded, Available, Repeating }
}
