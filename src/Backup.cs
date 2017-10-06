using System.Collections.Generic;

namespace Fsi.Osumimas.Sudoku {

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
	
	public class Backups : Stack<Backup> {}
}
