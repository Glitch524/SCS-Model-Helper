using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace SCS_Mod_Helper.ConverterPix {
	public class ConverterPixBinding(Action<PIXFile> listBoxScrollToTop): BaseBinding {
		private readonly Action<PIXFile> ScrollToFile = listBoxScrollToTop;

		private void ScrollToTop() => ScrollToFile(FileList[0]);

		public string mPackPath = "";
		public string PackPath {
			get => mPackPath;
			set {
				mPackPath = value;
				InvokeChange();
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


		public void ReadDir(string? focusPath = null) {
			string args = $"-b \"{PackPath}\" -listdir \"{PackDir}\"";
			Process p = CallPix(args);

			p.Start();
			PIXFile? focusFile = null;
			bool error = false;
			bool clearList = true;
			string? line;
			while ((line = p.StandardOutput.ReadLine()) != null) {
				Debug.WriteLine(line);
				if (line.StartsWith("<error>")) {
					var reason = line[(line.IndexOf(':') + 1)..];
					MessageBox.Show($"Unable to read the file: {reason}");
					error = true;
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
					string path = line[4..];
					var file = new PIXFile(path, true);
					if (focusPath == path)
						focusFile = file;
					FileList.Add(file);
				} else if (line.StartsWith("[F]")) {
					FileList.Add(new PIXFile(line[4..], false));
				}
			}
			p.WaitForExit();
			p.Close();
			if (!error) {
				if (focusFile == null)
					ScrollToTop();
				else
					ScrollToFile(focusFile);
			}
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
			string focusFile = DirList[index + 1].DirName;
			while (DirList.Count > index + 1) {
				DirList.RemoveAt(index + 1);
			}
			ReadDir(focusFile);
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

		public void ExtractFile(Window window, PIXFile pixFile) {
			StringBuilder args = new($"-b \"{PackPath}\" -e \"{DestPath}\"");
			int type = 0;
			if (pixFile.IsDir) {
				args.Append(" -extract_d \"").Append(pixFile.Filename).Append('"');
			} else {
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
			}
			Process p = CallPix(args.ToString());
			p.Start();

			string? line;
			bool success = true;
			List<string> errors = [];
			while ((line = p.StandardOutput.ReadLine()) != null) {
				Debug.WriteLine(line);

				if (line.Trim().StartsWith('*') || line.Length == 0)
					continue;
				if (line.StartsWith("<error>")) {
					success = false;
					line = line[7..];
					errors.Add(line);
				}
			}
			p.WaitForExit();
			p.Close();
			if (success) {
				string osFilename = pixFile.Filename.Replace('/', '\\');
				string filePath = DestPath + osFilename;
				if (OpenExtracted) {
					if (type == 1) {
						filePath = filePath[..^4] + ".pim";
					}
					if (File.Exists(filePath)) {
						Process.Start("explorer.exe", $"/select,\"{filePath}\"");
						return;
					}
				}

				string key = type switch {
					1 => "MessageExtractedModel",
					2 => "MessageExtractedTobj",
					_ => "MessageExtractedFile",
				};
				string resultMessage = $"{Util.GetString(key)} {filePath}";
				MessageBox.Show(window, resultMessage, "Result");
			} else {
				string resultMessage = $"{Util.GetString("MessageExtractFail")}\n{string.Join(Environment.NewLine, errors)}";
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
			List<string> anims = [];
			string? line;
			while ((line = p.StandardOutput.ReadLine()) != null) {
				if (line.StartsWith('*') || line.Length == 0)
					continue;
				if (line.EndsWith(".pma")) {
					anims.Add(line[..^4]);
				}
			}
			Debug.WriteLine(p.StandardOutput.ReadToEnd());
			p.WaitForExit();
			p.Close();
			return anims;
		}

		//可以用来查看文字文件内容，比如sii、mat、cfg，无法打开其他文件，tobj也不行 即使能读，也只有一点点就没有了
		public async Task ShowFile(Window window, PIXFile pixFile) {//todo 根据这个修改其他shell process
			StringBuilder args = new($"-b \"{PackPath}\" -show_f \"{pixFile.Filename}\"");

			var startInfo = new ProcessStartInfo {
				FileName = Instances.ConverterPixPath,
				Arguments = args.ToString(),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};

			List<string> datas = [];
			bool get = false;
			bool fileReadError = false;

			using var p = new Process { StartInfo = startInfo };
			p.OutputDataReceived += (sender, e) => {
				if (e.Data == null)
					return;
				var line = e.Data;
				if (line.Trim().StartsWith('*') || line.Length == 0)
					return;
				if (line.StartsWith("Unable to open file to read")) {
					fileReadError = true;
					return;
				}
				if (line.Trim().Contains('-')) {
					if (get)
						return;
					get = true;
					return;
				}
				if (get)
					datas.Add(line);
			};

			var errorString = new StringBuilder();
			p.ErrorDataReceived += (sender, e) => {
				if (e.Data != null)
					errorString.AppendLine(e.Data);
			};

			p.Start();
			p.BeginOutputReadLine();
			p.BeginErrorReadLine();

			await p.WaitForExitAsync();
			if (fileReadError) {
				window.Dispatcher.Invoke(() => 
					MessageBox.Show(window, GetString("MessageReadFileFailed"), pixFile.Filename));
				return;
			}
			if (datas.Count > 0) {
				Debug.WriteLine(string.Join(Environment.NewLine, datas));
				window.Dispatcher.Invoke(() =>
					MessageBox.Show(window, string.Join(Environment.NewLine, datas), pixFile.Filename));
			}
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
			Debug.WriteLine(p.StandardOutput.ReadToEnd());
			p.WaitForExit();
			p.Close();
		}
	}
}
