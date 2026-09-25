using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SCS_Mod_Helper.Base {
	public class SCSStreamWriter(string path, SCSStreamWriter.IAttrDetail attrDetail): StreamWriter(path) {

		public interface IAttrDetail {
			bool IsArray(string name);
			bool HasQuote(string name);
		}
		private readonly IAttrDetail AttrDetail = attrDetail;

		private const string FileHeader = "SiiNunit";
		private int TabCount = 0;

		public void WriteFileStructure(Action action) {
			TabCount = 0;
			WriteLine(FileHeader);
			BraceIn();
			action();
			BraceOut();
		}

		private void BraceIn() {//将括号下的内容缩进
			Write(new string('\t', TabCount));
			WriteLine('{');
			TabCount++;
		}
		private void BraceOut() {
			TabCount--;
			if (TabCount < 0)
				TabCount = 0;
			Write(new string('\t', TabCount));
			WriteLine('}');
		}
		public void WriteEmptyLine() => WriteLine("");

		protected void WriteLine(string name, string? value) {
			if (value == null)
				return;
			if (value.Length == 0)
				return;
			Write(name, value);
		}
		protected void WriteLine(string name, string? value, string defaultValue) {
			if (value == defaultValue)
				return;
			WriteLine(name, value);
		}

		protected void WriteLine(string name, bool value) => Write(name, value.ToString());

		protected void WriteLine(string name, bool value, bool defaultValue) {
			if (value == defaultValue)
				return;
			WriteLine(name, value);
		}

		protected void WriteLine(string name, float? value) {
			if (value == null)
				return;
			Write(name, FloatToString(value));
		}

		protected void WriteLine(string name, float? value, float defaultValue) {
			if (value == defaultValue)
				return;
			WriteLine(name, value);
		}

		protected void WriteLine(string name, float?[] floats) {
			if (floats == null)
				return;
			string value;

			switch (floats.Length) {
				case 2:
					if (floats[0] == null && floats[1] == null)
						return;
					value = $"({FloatToString(floats[0])},{FloatToString(floats[1])})";
					break;
				case 3:
					if (floats[0] == null && floats[1] == null && floats[2] == null)
						return;
					value = $"({FloatToString(floats[0])},{FloatToString(floats[1])},{FloatToString(floats[2])})";
					break;
				default:
					throw new ArgumentException("");
			}
			Write(name, value);
		}

		protected void WriteLine(string name, float?[] floats, float[] defaultValue) {
			if (Array.Equals(floats, defaultValue))
				return;
			WriteLine(name, floats);
		}

		protected void WriteLine(string name, float[]? floats) {
			if (floats == null)
				return;
			string value = floats.Length switch {
				2 => $"({FloatToString(floats[0])},{FloatToString(floats[1])})",
				3 => $"({FloatToString(floats[0])},{FloatToString(floats[1])},{FloatToString(floats[2])})",
				_ => throw new ArgumentException(""),
			};
			Write(name, value);
		}

		protected void WriteLine(string name, float[]? floats, float[] defaultValue) {
			if (Array.Equals(floats, defaultValue))
				return;
			WriteLine(name, floats);
		}

		private void Write(string name, string value) {
			var isArray = AttrDetail.IsArray(name);
			var hasQuote = AttrDetail.HasQuote(name);

			Write(new string('\t', TabCount));
			Write(name);
			if (isArray)
				Write("[]");
			Write(": ");
			if (hasQuote) {
				WriteLine($"\"{value}\"");
			} else
				WriteLine(value);
		}
		protected static string FloatToString(float? f) => f?.ToString("0.0#####") ?? "0.0";

		protected static string ClipValue(string value) {
			if (value.Contains('#'))//部分行会有注释
				value = value[..value.IndexOf('#')];
			if (value.Contains('"')) {//string 类数值会有双引号
				value = value[(value.IndexOf('"') + 1)..value.LastIndexOf('"')];
			} else
				value = value.Trim();
			if (value.EndsWith('{'))
				value = value[..^1].Trim();
			return value;
		}

		protected static void FloatParse(string value, Action<int, float> setter) {
			value = value[1..^1];
			var split = value.Split(',');
			for (int i = 0; i < split.Length; i++) {
				if (split[i].EndsWith('f'))
					split[i] = split[i][..^1];
				float f;
				try { f = float.Parse(split[i]); } catch { f = 0.0f; }
				setter(i, f);
			}
		}
	}
}
