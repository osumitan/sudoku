using System;
using System.Collections.Generic;
using System.Text;

namespace Fsi.Osumimas.Sudoku {

	public class Candidate : List<int> {
	
		private Cell cell;
		
		public Candidate(Cell cell) {
			this.cell = cell;
			Init();
		}
		
		public new void Clear() {
			base.Clear();
			RefreshToolTip();
		}
		
		public void Init() {
			Clear();
			for(int i = 0 ; i < Table.CELL_COUNT ; i++) {
				Add(i + 1);
			}
			RefreshToolTip();
		}
		
		public int SingleCandidate {
			get {
				if(Single) {
					return this[0];
				} else {
					string msg = string.Format("{0} candidate is not single ({1}).", this.cell, Count);
					throw new InvalidOperationException(msg);
				}
			}
		}
		
		public bool Single {
			get { return Count == 1; }
		}
		
		public new void Remove(int value) {
			if(!Contains(value)) {
				return;
			}
			base.Remove(value);
			RefreshToolTip();
			if(Count == 0) {
				throw new HasNoCandidateException(this.cell);
			}
		}
		
		public int[] CandidateValue {
			get {
				List<int> ret = new List<int>();
				foreach(int c in this) {
					ret.Add(c);
				}
				return ret.ToArray();
			}
		}
		
		private void RefreshToolTip() {
			cell.GridCell.ToolTipText = Count > 0 ? string.Format("[{0}]", string.Join(",", this)) : "";
		}
	}
}
