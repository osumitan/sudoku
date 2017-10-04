using System.Collections.Generic;

namespace Fsi.Osumimas.Sudoku {

	public class Box : Group {
	
		public Box(Table table) : base(table) {}
		
		public override int Index { get { return base.table.Boxes.IndexOf(this); } }
	}
	
	public class Boxes : Groups<Box> {
	
		public Boxes(Table table) : base(table) {}
		
		internal override Box createGroup() {
			return new Box(base.table);
		}
		
		internal override void AddCell(Cell cell) {
			int br = cell.Row.Index / Table.CELL_COUNT_SQRT;
			int bc = cell.Col.Index / Table.CELL_COUNT_SQRT;
			Box box = this[br * Table.CELL_COUNT_SQRT + bc];
			box.Add(cell);
			cell.Box = box;
		}
	}
}
