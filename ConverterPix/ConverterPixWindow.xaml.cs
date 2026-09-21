using Microsoft.Win32;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SCS_Mod_Helper.ConverterPix {
	/// <summary>
	/// ConverterPixWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ConverterPixWindow: BaseWindow {
		private readonly ConverterPixBinding Binding;

		public ConverterPixWindow() {
			InitializeComponent();

			Binding = new(ListBoxScrollToTop);
			GridMain.DataContext = Binding;
		}

		public string PackPath {
			get => Binding.PackPath;
			set => Binding.PackPath = value;
		}

		public string PackDir => Binding.PackDir;
		public ObservableCollection<PIXFile> FileList => Binding.FileList;
		public ObservableCollection<PackDir> DirList => Binding.DirList;//地址栏的列表

		private void ButtonChoosePack(object sender, RoutedEventArgs e) {
			OpenFileDialog openFileDialog = new() {
				Filter = Util.GetFilter("DialogFilterMod"),
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == true) {
				PackPath = openFileDialog.FileName;
				DirList.Clear();
				DirList.Add(new("/", openFileDialog.SafeFileName));
				Binding.ReadDir();
			}
		}

		private void ButtonChooseDestClick(object sender, RoutedEventArgs e) {
			OpenFolderDialog openFolderDialog = new() {
				Multiselect = false
			};
			if (openFolderDialog.ShowDialog() == true) {
				Binding.DestPath = openFolderDialog.FolderName;
			}
		}

		private void ButtonBackClick(object sender, RoutedEventArgs e) => FolderBack();

		private void FolderBack() {
			if (DirList.Count > 1) {
				Binding.DirBack(DirList.Count - 2);
			}
		}

		private void BreadcrumbBarItemClicked(Wpf.Ui.Controls.BreadcrumbBar _, Wpf.Ui.Controls.BreadcrumbBarItemClickedEventArgs args) {
			var index = args.Index;
			Binding.DirBack(index);
		}

		public void ListBoxScrollToTop(PIXFile file) => ListFiles.ScrollIntoView(file);

		private void ButtonExtractFileClick(object sender, RoutedEventArgs e) {
			if (Binding.SelectedFile is PIXFile pixFile) {
				Binding.ExtractFile(this, pixFile);
			}
		}

		private void ButtonShowFileClick(object sender, RoutedEventArgs e) {
			if (Binding.SelectedFile is PIXFile pixFile) {
				if (pixFile.IsDir)
					return;
				Binding.ShowFile(this, pixFile);
			}
		}

		private void ListBoxKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				Binding.OpenSelectedFile();
			} else if (e.Key == Key.Back) {
				FolderBack();
				e.Handled = true;//没有这个会导致在返回时莫名其妙选中第一个文件
			}
		}

		private void ButtonEDF(object sender, RoutedEventArgs e) {
			var extractTask = new ExtractAllTask(PackPath, Binding.DestPath) {
				Owner = this
			};
			extractTask.ShowDialog();
		}
	}
}
