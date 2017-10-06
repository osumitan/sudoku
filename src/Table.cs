using System.Windows.Forms;

namespace Fsi.Osumimas.Sudoku {

	public class Table {
		private Dimension dimension;
		private DataGridView grid;
		private Cells cells;
		private Rows rows;
		private Cols cols;
		private Boxes boxes;
		private Backups backups;
		
		public Table(DataGridView grid) {
			this.grid = grid;
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
		
		public Dimension Dimension {
			get { return this.dimension; }
			set {
				this.dimension = value;
				this.grid.DefaultCellStyle.Font = value.CellFont();
				this.grid.Width = value.CellWidth() * value.CellCount();
				this.grid.Height = value.CellHeight() * value.CellCount();
				this.grid.Columns.Clear();
				this.grid.Rows.Clear();
				for(int i = 0 ; i < value.CellCount() ; i++) {
					this.grid.Columns.Add("", "");
					this.grid.Columns[i].Width = value.CellWidth();
				}
				for(int i = 0 ; i < value.CellCount() ; i++) {
					this.grid.Rows.Add();
					this.grid.Rows[i].Height = value.CellHeight();
				}
				this.cells = new Cells(this);
				this.rows = new Rows(this);
				this.cols = new Cols(this);
				this.boxes = new Boxes(this);
				this.backups = new Backups();
			}
		}
		
		public DataGridView Grid { get{ return this.grid; } }
		public Cells Cells { get { return this.cells; } }
		public Rows Rows { get { return this.rows; } }
		public Cols Cols { get { return this.cols; } }
		public Boxes Boxes { get { return this.boxes; } }
	}
}
