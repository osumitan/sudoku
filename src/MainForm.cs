using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Fsi.Osumimas.Sudoku {

	public class MainForm : Form {
		private const Dimension DEFAULT_DIMENSION = Dimension.D3;
		private const int LOG_TEXT_HEIGHT = 100;
		private const double REPEAT_INTERVAL = 1000.0;
		
		private Table table;
		private DataGridView grid;
		private Button loadButton;
		private Button nextButton;
		private Button pauseButton;
		private Button repeatButton;
		private ComboBox dimensionComboBox;
		private TextBox logTextBox;
		private FormStatus status;
		private System.Timers.Timer repeatTimer;
		
		public MainForm() {
			this.Text = "sudoku";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			
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
			
			this.dimensionComboBox = InitDimensionComboBox();
			this.Controls.Add(this.dimensionComboBox);
			
			this.logTextBox = InitLogTextBox();
			this.Controls.Add(this.logTextBox);
			
			this.Load += (sender, e) => {
				this.Dimension = DEFAULT_DIMENSION;
				Sudoku.Log("Initialized");
			};
		}
		
		private void Init() {
			this.grid.Location = Sudoku.Location(null, null);
			this.loadButton.Location = Sudoku.Location(null, this.grid);
			this.nextButton.Location = Sudoku.Location(this.loadButton, this.grid);
			this.pauseButton.Location = Sudoku.Location(this.nextButton, this.grid);
			this.repeatButton.Location = Sudoku.Location(this.pauseButton, this.grid);
			this.dimensionComboBox.Location = Sudoku.Location(this.repeatButton, this.grid);
			this.logTextBox.Location = Sudoku.Location(null, this.loadButton);
			this.logTextBox.Width = this.grid.Width;
			this.Size = Sudoku.FormSize(this);
			Clear();
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
			grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			grid.SelectionChanged += (sender, e) => { this.grid.CurrentCell.Selected = false; };
			grid.CellPainting += (sender, e) => {
				if(e.RowIndex != 0 && e.RowIndex % this.table.Dimension.Value() == 0) {
					e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.InsetDouble;
				}
				if(e.ColumnIndex != 0 && e.ColumnIndex % this.table.Dimension.Value() == 0) {
					e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.InsetDouble;
				}
			};
			grid.ScrollBars = ScrollBars.None;
			return grid;
		}
		
		private Button InitLoadButton() {
			Button loadButton = new Button();
			loadButton.Enabled = false;
			loadButton.Text = "load";
			loadButton.Size = Sudoku.ButtonSize(loadButton);
			loadButton.Click += (sender, e) => { new LoadForm(this, this.table).ShowDialog(); };
			return loadButton;
		}
		
		private Button InitNextButton() {
			Button nextButton = new Button();
			nextButton.Enabled = false;
			nextButton.Text = ">";
			nextButton.Size = Sudoku.ButtonSize(nextButton);
			nextButton.Click += (sender, e) => { Solve(); };
			return nextButton;
		}
		
		private Button InitPauseButton() {
			Button pauseButton = new Button();
			pauseButton.Enabled = false;
			pauseButton.Text = "||";
			pauseButton.Size = Sudoku.ButtonSize(pauseButton);
			pauseButton.Click += (sender, e) => { Status = FormStatus.Available; };
			return pauseButton;
		}
		
		private Button InitRepeatButton() {
			Button repeatButton = new Button();
			repeatButton.Enabled = false;
			repeatButton.Text = ">>";
			repeatButton.Size = Sudoku.ButtonSize(repeatButton);
			repeatButton.Click += (sender, e) => { Status = FormStatus.Repeating; };
			return repeatButton;
		}
		
		private ComboBox InitDimensionComboBox() {
			ComboBox dimensionComboBox = new ComboBox();
			dimensionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			int width = 0;
			List<KeyValuePair<string, Dimension>> dataSource = new List<KeyValuePair<string, Dimension>>();
			foreach(Dimension dimension in Enum.GetValues(typeof(Dimension))) {
				dataSource.Add(new KeyValuePair<string, Dimension>(dimension.Text(), dimension));
				width = Math.Max(width, TextRenderer.MeasureText(dimension.Text(), dimensionComboBox.Font).Width);
			}
			dimensionComboBox.Width = width + 20;
			dimensionComboBox.DisplayMember = "Key";
			dimensionComboBox.ValueMember = "Value";
			dimensionComboBox.DataSource = dataSource;
			dimensionComboBox.SelectedIndexChanged += (sender, e) => {
				this.table.Dimension = this.Dimension;
				Init();
			};
			return dimensionComboBox;
		}
		
		private TextBox InitLogTextBox() {
			TextBox logTextBox = new TextBox();
			logTextBox.Multiline = true;
			logTextBox.ScrollBars = ScrollBars.Vertical;
			logTextBox.ReadOnly = true;
			logTextBox.Size = new Size(this.grid.Width, LOG_TEXT_HEIGHT);
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
					this.dimensionComboBox.Enabled = true;
				} else if(value == FormStatus.Available) {
					this.loadButton.Enabled = true;
					this.nextButton.Enabled = true;
					this.pauseButton.Enabled = false;
					this.repeatButton.Enabled = true;
					if(this.repeatTimer.Enabled) this.repeatTimer.Stop();
					this.dimensionComboBox.Enabled = true;
				} else if(value == FormStatus.Repeating) {
					this.loadButton.Enabled = false;
					this.nextButton.Enabled = false;
					this.pauseButton.Enabled = true;
					this.repeatButton.Enabled = false;
					this.repeatTimer.Start();
					this.dimensionComboBox.Enabled = false;
				}
			}
		}
		
		public void Log(string format, params object[] prms) {
			this.logTextBox.AppendText(string.Format(format, prms) + "\r\n");
		}
		
		public Dimension Dimension {
			get { return (Dimension)this.dimensionComboBox.SelectedValue; }
			set { this.dimensionComboBox.SelectedValue = value; }
		}
	}
	
	public enum FormStatus { Unloaded, Available, Repeating }
}
