using System.Collections.Generic;

namespace Fsi.Osumimas.Sudoku {

	public class Col : Group {
	
		public Col(Table table) : base(table) {}
		
		public override int Index { get { return base.table.Cols.IndexOf(this); } }
	}
	
	public class Cols : Groups<Col> {
	
		public Cols(Table table) : base(table) {}
		
		internal override Col createGroup() {
			return new Col(base.table);
		}
		
		internal override void AddCell(Cell cell) {
			Col col = this[cell.Index % Table.CELL_COUNT];
			col.Add(cell);
			cell.Col = col;
		}
	}
}
