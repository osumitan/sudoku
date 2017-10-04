using System.Collections.Generic;

namespace Fsi.Osumimas.Sudoku {

	public abstract class Group : List<Cell> {
		internal Table table;
		
		public Group(Table table) {
			this.table = table;
		}
		
		public void RemoveCandidate(int value) {
			foreach(Cell cell in this) {
				cell.RemoveCandidate(value);
			}
		}
		
		public Cell SolveSingleCandidate() {
			for(int value = 1 ; value <= Table.CELL_COUNT ; value++) {
				List<Cell> c = new List<Cell>();
				foreach(Cell cell in this) {
					if(cell.ContainsCandidate(value)) {
						c.Add(cell);
					}
				}
				if(c.Count == 1) {
					c[0].Value = value;
					return c[0];
				}
			}
			return null;
		}
		
		public Table Table { get { return this.table; } }
		public abstract int Index { get; }
	}
	
	public abstract class Groups<G> : List<G> where G : Group {
		internal Table table;
		
		public Groups(Table table) {
			this.table = table;
			for(int i = 0 ; i < Table.CELL_COUNT ; i++) {
				Add(createGroup());
			}
			foreach(Cell cell in this.table.Cells) {
				AddCell(cell);
			}
		}
		
		public Cell SolveSingleCandidate() {
			foreach(Group group in this) {
				Cell cell = group.SolveSingleCandidate();
				if(cell != null) {
					return cell;
				}
			}
			return null;
		}
		
		internal abstract G createGroup();
		internal abstract void AddCell(Cell cell);
	}
}
