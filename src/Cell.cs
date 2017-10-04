using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fsi.Osumimas.Sudoku {

	public class Cell {
		private const int BLANK = 0;
		private const int MAX_FLASH_LEVEL = 16;
		private const double FLASH_INTERVAL = 50.0;
		
		private Table table;
		private Row row;
		private Col col;
		private Box box;
		private Candidate candidate;
		private CellType type;
		private DataGridViewCell cell;
		private System.Timers.Timer flashTimer;
		private int flashLevel;
		
		public Cell(Table table, DataGridViewCell cell) {
			this.table = table;
			this.cell = cell;
			this.candidate = new Candidate(this);
			
			this.flashTimer = new System.Timers.Timer();
			this.flashTimer.Elapsed += Flash;
			this.flashTimer.Interval = FLASH_INTERVAL;
			this.flashTimer.SynchronizingObject = this.table.Grid;
			
			Clear();
		}
		
		public override string ToString() {
			return string.Format("cell[{0},{1}]", Row.Index, Col.Index);
		}
		
		public void Clear() {
			this.cell.Value = "";
			this.candidate.Init();
			ResetFlashTimer();
		}
		
		public CellType Type {
			get { return this.type; }
			set {
				this.type = value;
				this.cell.Style.ForeColor = value.ForeColor();
			}
		}
		
		public bool Blank {
			get { return Value == BLANK; }
		}
		
		public int Value {
			get { int r; return int.TryParse(this.cell.Value.ToString(), out r) ? r : BLANK; }
			set {
				if(value < 1 || Table.CELL_COUNT < value) {
					throw new ArgumentException(String.Format("{0} <= {1}", this, value));
				}
				cell.Value = value.ToString();
				if(!ContainsCandidate(value)) {
					throw new ValueNotAcceptedException(this, value);
				}
				this.candidate.Clear();
				this.row.RemoveCandidate(value);
				this.col.RemoveCandidate(value);
				this.box.RemoveCandidate(value);
			}
		}
		
		public int CandidateCount { get { return this.candidate.Count; } }
		public int[] CandidateValue { get { return this.candidate.CandidateValue; } }
		
		public void RemoveCandidate(int value) {
			this.candidate.Remove(value);
		}
		
		public bool ContainsCandidate(int value) {
			return this.candidate.Contains(value);
		}
		
		public bool SolveSingleCandidate() {
			if(Blank && this.candidate.Single) {
				Value = this.candidate.SingleCandidate;
				return true;
			} else {
				return false;
			}
		}
		
		public delegate void AfterImport(Cell cell);
		public void Import(char c, AfterImport succeeded, AfterImport failed) {
			try {
				if(c != '_') {
					Value = Convert.ToInt32(c) - Convert.ToInt32('0');
				}
				if(succeeded != null) {
					succeeded(this);
				}
			} catch(SudokuException e) {
				if(failed != null) {
					failed(this);
				}
				throw e;
			}
		}
		
		public char Export() {
			return Blank ? '_' : Convert.ToChar(Value + Convert.ToInt32('0'));
		}
		
		public void Flash() {
			ResetFlashTimer();
			this.flashLevel = MAX_FLASH_LEVEL;
			this.flashTimer.Start();
		}
		public void Flash(object sender, System.Timers.ElapsedEventArgs e) {
			int color = 256 - Math.Max(this.flashLevel * 256 / MAX_FLASH_LEVEL, 1);
			this.cell.Style.BackColor = Color.FromArgb(255, 255, color);
			if(this.flashLevel <= 0) {
				this.flashTimer.Stop();
			}
			this.flashLevel--;
		}
		private void ResetFlashTimer() {
			this.flashLevel = 0;
			this.cell.Style.BackColor = Color.White;
			this.flashTimer.Stop();
		}
		
		public Table Table { get { return this.table; } }
		public int Index { get { return this.table.Cells.IndexOf(this); } }
		public Row Row { get { return this.row; } set { this.row = value; } }
		public int RowIndex { get { return this.row.IndexOf(this); } }
		public Col Col { get { return this.col; } set { this.col = value; } }
		public int ColIndex { get { return this.col.IndexOf(this); } }
		public Box Box { get { return this.box; } set { this.box = value; } }
		public int BoxIndex { get { return this.box.IndexOf(this); } }
		public DataGridViewCell GridCell { get { return this.cell; } }
	}
	
	public enum CellType { Question, Answer, Error }
	public static class CellTypeExt {
		public static Color ForeColor(this CellType type) {
			if(type == CellType.Question) {
				return Color.Black;
			} else if(type == CellType.Answer) {
				return Color.Blue;
			} else if(type == CellType.Error) {
				return Color.Red;
			} else {
				return Color.Green;
			}
		}
	}
	
	public class Cells : List<Cell> {
		private Table table;
		
		public Cells(Table table) {
			this.table = table;
			for(int c = 0 ; c < this.table.Grid.Columns.Count ; c++) {
				for(int r = 0 ; r < this.table.Grid.Rows.Count ; r++) {
					Add(new Cell(this.table, this.table.Grid[r, c]));
				}
			}
		}
		
		public new void Clear() {
			foreach(Cell cell in this) {
				cell.Clear();
			}
		}
		
		public bool Blank {
			get {
				foreach(Cell cell in this) {
					if(!cell.Blank) return false;
				}
				return true;
			}
		}
		
		public bool Completed {
			get {
				foreach(Cell cell in this) {
					if(cell.Blank) return false;
				}
				return true;
			}
		}
		
		public Cell SolveSingleCandidate() {
			foreach(Cell cell in this) {
				if(cell.SolveSingleCandidate()) {
					return cell;
				}
			}
			return null;
		}
		
		public Cell RetryCell {
			get {
				for(int count = 2 ; count <= Backup.MAX_RETRY_CANDIDATES ; count++) {
					foreach(Cell cell in this) {
						if(cell.CandidateCount == count) {
							return cell;
						}
					}
				}
				return null;
			}
		}
	}
}
