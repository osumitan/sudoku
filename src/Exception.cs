using System;

namespace Fsi.Osumimas.Sudoku {

	public class SudokuException : Exception {
		public SudokuException(string message) : base(message) {}
	}
	
	public class ContradictionException : SudokuException {
		public ContradictionException(string message) : base(message) {}
	}
	
	public class HasNoCandidateException : ContradictionException {
		private static string MSG(Cell cell) {
			return string.Format("{0} has no candidate", cell);
		}
		public HasNoCandidateException(Cell cell) : base(MSG(cell)) {}
	}
	
	public class ValueNotAcceptedException : ContradictionException {
		private static string MSG(Cell cell, int value) {
			return string.Format("{0} does not accept {1}", cell, value);
		}
		public ValueNotAcceptedException(Cell cell, int value) : base(MSG(cell, value)) {}
	}
}
