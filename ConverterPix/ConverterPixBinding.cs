using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows.UI;
using Wpf.Ui.Input;

namespace SCS_Mod_Helper.ConverterPix {
	public class ConverterPixBinding: BaseBinding {

		public string mPackPath = "";
		public string PackPath {
			get => mPackPath;
			set {
				mPackPath = value;
				InvokeChange();
				InvokeChange(nameof(PathSet));
			}
		}

		private bool mCustomDestPath = false;
		public bool CustomDestPath {
			get => mCustomDestPath;
			set {
				mCustomDestPath = value;
				if (!CustomDestPath)
					DestPath = Instances.ProjectLocation;
				InvokeChange();
			}
		}

		private string mDestPath = Instances.ProjectLocation;
		public string DestPath {
			get => mDestPath;
			set {
				mDestPath = value;
				InvokeChange();
			}
		}

		public bool PathSet => PackPath.Length > 0;

		public string PackDir => DirList.LastOrDefault()?.DirName ?? "/";

		private readonly ObservableCollection<PIXFile> mFileList = [];

		public ObservableCollection<PIXFile> FileList => mFileList;

		private PIXFile? mSelectedFile = null;
		public PIXFile? SelectedFile {
			get => mSelectedFile;
			set {
				mSelectedFile = value;
				InvokeChange();
			}
		} 


		public void ReadDir() {
			string args = $"-b \"{PackPath}\" -listdir \"{PackDir}\"";
			Process p = CallPix(args);

			p.Start();

			bool clearList = true;
			string? line;
			while ((line = p.StandardOutput.ReadLine()) != null) {
				Debug.WriteLine(line);
				if (line.StartsWith("<error>")) {
					var reason = line[(line.IndexOf(':') + 1)..];
					MessageBox.Show($"Unable to read the file: {reason}");
					break;
				}
				if (clearList) {
					clearList = false;
					FileList.Clear();
					SelectedFile = null;
				}
				if (line.Contains("Done"))
					break;
				if (line.StartsWith("[D]")) {
					FileList.Add(new PIXFile(line[4..], true));
				} else if (line.StartsWith("[F]")) {
					FileList.Add(new PIXFile(line[4..], false));
				}
			}
			p.WaitForExit();
			p.Close();
		}

		public ICommand OpenFileCommand => new RelayCommand<object>(OpenFile);

		public void OpenFile(object? parameter) {
			if (parameter == null)
				return;
			if (parameter is PIXFile pixFile) {
				var filename = pixFile.Filename;
				if (pixFile.IsDir) {
					DirList.Add(new PackDir(filename));
					ReadDir();
				}
			}
		}

		public void OpenSelectedFile() => OpenFile(SelectedFile);

		private readonly ObservableCollection<PackDir> mDirList = [];

		public ObservableCollection<PackDir> DirList => mDirList;

		public void DirBack(int index) {
			while(DirList.Count > index + 1) {
				DirList.RemoveAt(index + 1);
			}
			ReadDir();
		}

		private static Process CallPix(string args) {
			Process p = new();
			p.StartInfo.FileName = Instances.ConverterPixPath;
			p.StartInfo.Arguments = args;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			return p;
		}

		public void TestExtract(Window window, PIXFile pixFile) {
			StringBuilder args = new($"-b \"{PackPath}\" -e \"{DestPath}\"");
			int type = 0;
			switch (pixFile.Extension.ToLower()) {
				case "pmd":
				case "pmg":
				case "pma"://可能出现没有pmd的情况，需要找pmd
				case "pmc":
					type = 1;
					var name = pixFile.Filename;
					name = name[..^4];
					args.Append(" -m \"").Append(name).Append('"');
					if (SearchAnim) {
						var anim = SearchAnimExe(name);
						if (anim.Count > 0)
							foreach (string a in anim) {
								args.Append(" \"").Append(a).Append('"');
								break;
							}
					}
					break;
				case "tobj":
					type = 2;
					args.Append(" -t \"").Append(pixFile.Filename).Append('"');
					break;
				default:
					args.Append(" -extract_f \"").Append(pixFile.Filename).Append('"');
					break;
			}
			Process p = CallPix(args.ToString());
			p.Start();
			p.WaitForExit();

			string? line;
			bool success = true;
			List<string> errors = [];
			while((line = p.StandardOutput.ReadLine()) != null) {
				Debug.WriteLine(line);

				if (line.Trim().StartsWith('*') || line.Length == 0)
					continue;
				if (line.StartsWith("<error>")) {
					success = false;
					line = line[7..];
					errors.Add(line);
				}
			}
			p.Close();
			if (success) {
				string osFilename = pixFile.Filename.Replace('/', '\\');
				if (OpenExtracted) {
					if (type == 1) {
						osFilename = osFilename[..^4] + ".pim";
					}
					string path = $"D:\\test{osFilename}";
					if (File.Exists(path)) {
						Process.Start("explorer.exe", $"/select,\"{path}\"");
						return;
					}
				}
				string resultMessage = type switch {
					1 => $"Model extracted and decrypted successfully to D:\\test{osFilename[..^4]}",
					2 => $"Tobj extracted and decrypted successfully to D:\\test{osFilename}",
					_ => $"File extracted successfully to D:\\test{osFilename}",
				};
				MessageBox.Show(window, resultMessage, "Result");
			} else {
				string resultMessage = $"Failed to extract and decrypt the file. \nErrors:\n{string.Join(Environment.NewLine, errors)}";
				MessageBox.Show(window, resultMessage, "Result");
			}
		}


		private bool mSearchAnim = false;
		public bool SearchAnim {
			get => mSearchAnim;
			set {
				mSearchAnim = value;
				InvokeChange();
			}
		}

		private bool mOpenExtracted = false;
		public bool OpenExtracted {
			get => mOpenExtracted;
			set {
				mOpenExtracted = value;
				InvokeChange();
			}
		}

		public List<string> SearchAnimExe(string modelPath) {
			string args = $"-b \"{PackPath}\" --find-model-animations \"{modelPath}\"";

			Process p = CallPix(args);

			p.Start();
			p.WaitForExit();
			List<string> anims = [];
			string? line;
			while((line = p.StandardOutput.ReadLine()) != null) {
				if (line.StartsWith('*') || line.Length == 0)
					continue;
				if (line.EndsWith(".pma")) {
					anims.Add(line[..^4]);
				}
			}
			Debug.WriteLine(p.StandardOutput.ReadToEnd());
			p.Close();
			return anims;
		}

		//可以用来查看文字文件内容，比如sii、mat、cfg，无法打开其他文件，tobj也不行 即使能读，也只有一点点就没有了
		public void TestShow(Window window, PIXFile pixFile) {
			StringBuilder args = new($"-b \"{PackPath}\" -show_f \"{pixFile.Filename}\"");

			Process p = CallPix(args.ToString());

			p.Start();

			string? line;
			bool get = false;
			List<string> datas = [];
			while ((line = p.StandardOutput.ReadLine()) != null) {
				if (line.Trim().StartsWith('*') || line.Length == 0)
					continue;
				if (line.StartsWith("Unable to open file to read")) {
					MessageBox.Show(window, "Unable to read this file", pixFile.Filename);
					break;
				}
				if (line.Trim().Contains('-')) {
					if (get) {
						break;
					} else {
						get = true;
						continue;
					}
				}
				if (get) {
					datas.Add(line);
				}
			}
			if (datas.Count > 0) {
				Debug.WriteLine(string.Join(Environment.NewLine, datas));
				MessageBox.Show(window, string.Join(Environment.NewLine, datas), pixFile.Filename);
			}

			p.WaitForExit();
			p.Close();
		}

		public void TestExtractFD(PIXFile pixFile) {
			StringBuilder args = new($"-b \"{PackPath}\" -e \"{DestPath}\"");
			if (pixFile.IsDir) {
				args.Append(" -extract_d \"").Append(pixFile.Filename).Append('"');
			} else {
				args.Append(" -extract_f \"").Append(pixFile.Filename).Append('"');
			}
			Process p = CallPix(args.ToString());

			p.Start();
			p.WaitForExit();
			Debug.WriteLine(p.StandardOutput.ReadToEnd());
			p.Close();
		}
	}
}
