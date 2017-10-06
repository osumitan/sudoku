using System.Collections.Generic;
using System.Text;

namespace Fsi.Osumimas.Sudoku {

	public class Row : Group {
	
		public Row(Table table) : base(table) {}
		
		public void Import(string line, Cell.AfterImport succeeded, Cell.AfterImport failed) {
			foreach(Cell cell in this) {
				cell.Import(line[cell.RowIndex], succeeded, failed);
			}
		}
		
		public string Export() {
			StringBuilder sb = new StringBuilder();
			foreach(Cell cell in this) {
				sb.Append(cell.Export());
			}
			return sb.ToString();
		}
		
		public override int Index { get { return base.table.Rows.IndexOf(this); } }
	}
	
	public class Rows : Groups<Row> {
	
		public Rows(Table table) : base(table) {}
		
		internal override Row createGroup() {
			return new Row(base.table);
		}
		
		internal override void AddCell(Cell cell) {
			Row row = this[cell.Index / base.table.Dimension.CellCount()];
			row.Add(cell);
			cell.Row = row;
		}
		
		public void Import(string[] lines, Cell.AfterImport succeeded, Cell.AfterImport failed) {
			foreach(Row row in this) {
				row.Import(lines[row.Index], succeeded, failed);
			}
		}
		
		public string[] Export() {
			List<string> list = new List<string>();
			foreach(Row row in this) {
				list.Add(row.Export());
			}
			return list.ToArray();
		}
	}
}
