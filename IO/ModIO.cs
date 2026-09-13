using SCS_Mod_Helper.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Mod_Helper.IO {
	public abstract class ModIO() {
		protected const string FileHeader = "SiiNunit";
		protected int TabCount = 0;

		protected void WriteFileStructure(StreamWriter sw, Action action) {
			TabCount = 0;
			sw.WriteLine(FileHeader);
			BraceIn(sw);
			action();
			BraceOut(sw);
		}

		protected void BraceIn(StreamWriter sw) {//将括号下的内容缩进
			sw.Write(new string('\t', TabCount));
			sw.WriteLine('{');
			TabCount++;
		}
		protected void BraceOut(StreamWriter sw) {
			TabCount--;
			if (TabCount < 0)
				TabCount = 0;
			sw.Write(new string('\t', TabCount));
			sw.WriteLine('}');
		}

		protected abstract bool IsArray(string name);

		protected abstract bool HasQuote(string name);

		protected static void WriteEmptyLine(StreamWriter sw) => sw.WriteLine("");

		protected void WriteLine(StreamWriter sw, string name, object? valueObj, object skipValue) {
			if (valueObj == null)
				return;
			var vals = valueObj.ToString();
			var skips = skipValue.ToString();
			if (vals == skips)
				return;
			WriteLine(sw, name, valueObj);
		}

		protected void WriteLine(StreamWriter sw, string name, object? valueObj) {
			if (valueObj == null)
				return;
			if (valueObj is string s) {
				if (s.Length == 0)
					return;
			}
			var isArray = IsArray(name);
			var hasQuote = HasQuote(name);

			string value;
			if (valueObj is float?[] floats) {
				if (floats.Length == 2) {
					if (floats[0] == null && floats[1] == null)
						return;
					value = $"({FloatToString(floats[0])},{FloatToString(floats[1])})";
				} else {
					if (floats[0] == null && floats[1] == null && floats[2] == null)
						return;
					value = $"({FloatToString(floats[0])},{FloatToString(floats[1])},{FloatToString(floats[2])})";
				}
			} else if (valueObj is float[] floatsN) {
				if (floatsN.Length == 2) {
					value = $"({FloatToString(floatsN[0])},{FloatToString(floatsN[1])})";
				} else
					value = $"({FloatToString(floatsN[0])},{FloatToString(floatsN[1])},{FloatToString(floatsN[2])})";
			} else if (valueObj is float f) {
				value = FloatToString(f);
			} else
				value = valueObj.ToString() ?? "";
			sw.Write(new string('\t', TabCount));
			sw.Write(name);
			if (isArray)
				sw.Write("[]");
			sw.Write(": ");
			if (hasQuote) {
				sw.WriteLine($"\"{value}\"");
			} else
				sw.WriteLine(value);
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
