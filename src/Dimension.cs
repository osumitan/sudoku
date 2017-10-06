using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Fsi.Osumimas.Sudoku {

	public enum Dimension { D2 = 2, D3 = 3, D4 = 4 }
	
	public static class DimensionExt {
		private static readonly Dictionary<Dimension, Regex> LOAD_REGEX_LINE;
		private static readonly Dictionary<Dimension, string> LOAD_CHARS;
		private static readonly Dictionary<Dimension, string> LOAD_SAMPLE;
		private static readonly Dictionary<Dimension, string> LOAD_DEFAULT;
		private static readonly Dictionary<Dimension, Font> CELL_FONT;
		private static readonly Dictionary<Dimension, int> CELL_WIDTH;
		private static readonly Dictionary<Dimension, int> CELL_HEIGHT;
		
		static DimensionExt() {
			LOAD_REGEX_LINE = new Dictionary<Dimension, Regex>();
			LOAD_REGEX_LINE[Dimension.D2] = new Regex("^[_1-4_]{4}$");
			LOAD_REGEX_LINE[Dimension.D3] = new Regex("^[_1-9_]{9}$");
			LOAD_REGEX_LINE[Dimension.D4] = new Regex("^[_1-9A-G_]{16}$");
			
			LOAD_CHARS = new Dictionary<Dimension, string>();
			LOAD_CHARS[Dimension.D2] = "_1234";
			LOAD_CHARS[Dimension.D3] = "_123456789";
			LOAD_CHARS[Dimension.D4] = "_123456789ABCDEFG";
			
			LOAD_SAMPLE = new Dictionary<Dimension, string>();
			foreach(Dimension dimension in Enum.GetValues(typeof(Dimension))) {
				StringBuilder sb = new StringBuilder();
				for(int i = 0 ; i <= dimension.CellCount() ; i++) {
					if(i > 0) sb.Append("\r\n");
					for(int j = 0 ; j <= dimension.CellCount() ; j++) {
						sb.Append("_");
					}
				}
				LOAD_SAMPLE[dimension] = sb.ToString();
			}
			
			LOAD_DEFAULT = new Dictionary<Dimension, string>();
			foreach(Dimension dimension in Enum.GetValues(typeof(Dimension))) {
				StringBuilder sb = new StringBuilder();
				for(int i = 0 ; i < dimension.CellCount() ; i++) {
					if(i > 0) sb.Append("\r\n");
					for(int j = 0 ; j < dimension.CellCount() ; j++) {
						sb.Append("_");
					}
				}
				LOAD_DEFAULT[dimension] = sb.ToString();
			}
			
			CELL_FONT = new Dictionary<Dimension, Font>();
			CELL_FONT[Dimension.D2] = new Font("Meiryo UI", 20, FontStyle.Bold);
			CELL_FONT[Dimension.D3] = new Font("Meiryo UI", 16, FontStyle.Bold);
			CELL_FONT[Dimension.D4] = new Font("Meiryo UI", 11, FontStyle.Bold);
			
			CELL_WIDTH = new Dictionary<Dimension, int>();
			CELL_WIDTH[Dimension.D2] = 40;
			CELL_WIDTH[Dimension.D3] = 36;
			CELL_WIDTH[Dimension.D4] = 32;
			
			CELL_HEIGHT = new Dictionary<Dimension, int>();
			CELL_HEIGHT[Dimension.D2] = 40;
			CELL_HEIGHT[Dimension.D3] = 36;
			CELL_HEIGHT[Dimension.D4] = 32;
		}
		
		public static int Value(this Dimension dimension) {
			return (int)dimension;
		}
		
		public static int CellCount(this Dimension dimension) {
			return dimension.Value() * dimension.Value();
		}
		
		public static string Text(this Dimension dimension) {
			return string.Format("{0}x{0}", dimension.Value());
		}
		
		public static Regex LoadRegexLine(this Dimension dimension) {
			return LOAD_REGEX_LINE[dimension];
		}
		
		public static string LoadChars(this Dimension dimension) {
			return LOAD_CHARS[dimension];
		}
		
		public static string LoadSample(this Dimension dimension) {
			return LOAD_SAMPLE[dimension];
		}
		
		public static string LoadDefault(this Dimension dimension) {
			return LOAD_DEFAULT[dimension];
		}
		
		public static Font CellFont(this Dimension dimension) {
			return CELL_FONT[dimension];
		}
		
		public static int CellWidth(this Dimension dimension) {
			return CELL_WIDTH[dimension];
		}
		
		public static int CellHeight(this Dimension dimension) {
			return CELL_HEIGHT[dimension];
		}
	}
}
