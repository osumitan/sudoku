using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fsi.Osumimas.Sudoku {

	public class Table {
		public const int CELL_COUNT_SQRT = 3;
		public const int CELL_COUNT = CELL_COUNT_SQRT * CELL_COUNT_SQRT;
		
		private DataGridView grid;
		private Cells cells;
		private Rows rows;
		private Cols cols;
		private Boxes boxes;
		private Stack<Backup> backups;
		
		public Table(DataGridView grid) {
			this.grid = grid;
			this.cells = new Cells(this);
			this.rows = new Rows(this);
			this.cols = new Cols(this);
			this.boxes = new Boxes(this);
			this.backups = new Stack<Backup>();
		}
		
		public void Clear() {
			this.cells.Clear();
			this.backups.Clear();
		}
		
		public bool Blank {
			get { return this.cells.Blank; }
		}
		
		public bool Completed {
			get { return this.cells.Completed; }
		}
		
		public bool Solve() {
			Cell cell = null;
			try {
				if(cell == null) cell = this.cells.SolveSingleCandidate();
				if(cell == null) cell = this.rows.SolveSingleCandidate();
				if(cell == null) cell = this.cols.SolveSingleCandidate();
				if(cell == null) cell = this.boxes.SolveSingleCandidate();
				if(cell == null) {
					Cell retryCell = this.cells.RetryCell;
					if(retryCell != null) {
						Backup backup = new Backup(retryCell, Export());
						this.backups.Push(backup);
						cell = backup.TryNext();
					}
				}
			} catch(ContradictionException e) {
				Sudoku.Log(e.Message);
				if(this.backups.Count >= 1) {
					Backup backup = this.backups.Pop();
					Import(backup.Lines);
					cell = backup.TryNext();
					if(backup.HasNext) {
						this.backups.Push(backup);
					}
				}
			} finally {
				if(cell != null) {
					cell.Flash();
					Sudoku.Log("{0} Solved value={1}", cell, cell.Value);
				}
			}
			return cell != null;
		}
		
		public void Import(string[] lines) {
			Import(lines, null, null);
		}
		public void Import(string[] lines, Cell.AfterImport succeeded, Cell.AfterImport failed) {
			Clear();
			this.rows.Import(lines, succeeded, failed);
		}
		
		public string[] Export() {
			return this.rows.Export();
		}
		
		public DataGridView Grid { get{ return this.grid; } }
		public Cells Cells { get { return this.cells; } }
		public Rows Rows { get { return this.rows; } }
		public Cols Cols { get { return this.cols; } }
		public Boxes Boxes { get { return this.boxes; } }
	}
	
	public class Backup {
		public const int MAX_RETRY_CANDIDATES = 2;
		
		private Cell cell;
		private int[] candidate;
		private int index;
		private string[] lines;
		
		public Backup(Cell cell, string[] lines) {
			this.cell = cell;
			this.candidate = this.cell.CandidateValue;
			this.index = 0;
			this.lines = lines;
		}
		
		public bool HasNext { get { return this.index < this.candidate.Length; } }
		
		public Cell TryNext() {
			if(HasNext) {
				Sudoku.Log("{0} Try {1}/{2}", this.cell, (this.index + 1), this.candidate.Length);
				this.cell.Value = this.candidate[this.index++];
				return this.cell;
			} else {
				return null;
			}
		}
		
		public Cell Cell { get { return this.cell; } }
		public string[] Lines { get { return this.lines; } }
	}
}
