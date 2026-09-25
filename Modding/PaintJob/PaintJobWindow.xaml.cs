using ColorPicker;
using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Localization;
using SCS_Mod_Helper.Modding.Accessories;
using SCS_Mod_Helper.Trucks;
using SCS_Mod_Helper.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SCS_Mod_Helper.Modding.PaintJob {
	/// <summary>
	/// PaintJobWindow.xaml 的交互逻辑
	/// </summary>
	public partial class PaintJobWindow: BaseWindow {
		private readonly PaintJobBinding Binding = new();

		private readonly ContextMenu MenuStringRes;
		public PaintJobWindow() {
			InitializeComponent();

			GridMain.DataContext = Binding;

			MenuStringRes = (ContextMenu)Resources["MenuStringRes"];
			MenuStringRes.PlacementTarget = ButtonChooseRes;
			MenuStringRes.DataContext = Binding;
		}


		public static Color ColorMask(
		Color baseColor,
		Color paintJobColor,
		Color maskR,
		Color maskG,
		Color maskB) {
			float remain = 1f;

			float percentR = paintJobColor.R / 255f;
			float percentG = paintJobColor.G / 255f;
			float percentB = paintJobColor.B / 255f;
			Debug.WriteLine($"pjFloat = {paintJobColor.R}={percentR}%, {paintJobColor.G}={percentG}%, {paintJobColor.B}={percentB}%");

			float percent = percentR;
			remain -= percent;

			Color colorR = Color.FromRgb(
				(byte)(percent * maskR.R),
				(byte)(percent * maskR.G),
				(byte)(percent * maskR.B));
			Debug.WriteLine($"blendR={colorR} percent={percent}");


			percent = percentG * remain;
			remain -= percent;

			Color colorG = Color.FromRgb(
				(byte)(percent * maskG.R),
				(byte)(percent * maskG.G),
				(byte)(percent * maskG.B));
			Debug.WriteLine($"blendG={colorG} percent={percent}");


			percent = percentB * remain;
			remain -= percent;

			Color colorB = Color.FromRgb(
				(byte)(percent * maskB.R),
				(byte)(percent * maskB.G),
				(byte)(percent * maskB.B));
			Debug.WriteLine($"blendB={colorB} percent={percent}");

			Color colorBase = Color.FromRgb(
				(byte)(remain * baseColor.R),
				(byte)(remain * baseColor.G),
				(byte)(remain * baseColor.B));

			Color result = Color.FromRgb(
				(byte)(colorR.R + colorG.R + colorB.R + colorBase.R),
				(byte)(colorR.G + colorG.G + colorB.G + colorBase.G),
				(byte)(colorR.B + colorG.B + colorB.B + colorBase.B));

			return result;
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var answer = MessageBox.Show(this, $"Selected Tab: {((TabItem)((TabControl)sender).SelectedItem).Header}");
			Debug.WriteLine($"Selected Tab: {((TabItem)((TabControl)sender).SelectedItem).Header} answer={answer}");
		}


		private void OnMenuClicked(object sender, RoutedEventArgs e) {
			MenuItem item = (MenuItem)sender;
			ContextMenu cm = (ContextMenu)item.Parent;
			if (cm == MenuStringRes) {
				var menuName = (string)item.CommandParameter;
				cm = menuName switch {
					"MenuStringRes" => MenuStringRes,
					_ => throw new NotImplementedException(),
				};
				if (cm == MenuStringRes) {
					var tag = (string)item.Tag;
					if (tag.Equals("openLocalization")) {
						StringResUtil.OpenLocalization(this);
					} else {
						StringResUtil.ApplyStringRes(TextDisplayName, tag);
					}
				}
			}
		}





		private void NumberOnly(object sender, TextCompositionEventArgs e) => TextControl.NumberOnly(sender, e);

		private void FloatOnly(object sender, TextCompositionEventArgs e) => TextControl.FloatOnly(sender, e);

		private void HexCheck(object sender, TextCompositionEventArgs e) {
			if (sender is TextBox textBox) {
				if (textBox.Text.Length >= 6 && textBox.SelectionLength == 0) {
					e.Handled = true;
					return;
				}
				char c = e.Text[0];
				if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')) {
					e.Handled = false;
				} else
					e.Handled = true;
			}
		}

		private void OnPasted(object sender, DataObjectPastingEventArgs e) {
			if (sender is TextBox textBox) {
				if (e.DataObject.GetDataPresent(DataFormats.UnicodeText)) {
					string pasted = (string)e.DataObject.GetData(DataFormats.UnicodeText);
					if (pasted.StartsWith('#'))
						pasted = pasted[1..];
					if (pasted.Length > 8) {
						e.CancelCommand();
						FlyoutWrongHex.Show();
					}
					pasted = pasted.ToUpper();
					for (int i = 0; i < pasted.Length; i++) {
						char c = pasted[i];
						if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')) {
							continue;
						} else {
							e.CancelCommand();
							FlyoutWrongHex.Show();
							return;
						}
					}
					e.CancelCommand();
					if (pasted.Length > 6) {
						pasted = pasted[^6..];
					} else
						pasted = pasted.PadLeft(6, '0');
					textBox.Text = pasted;
					textBox.SelectionStart = 0;
					textBox.SelectionLength = pasted.Length;
				}
			}
		}

		private void ButtonChooseTruck(object sender, RoutedEventArgs e) {

		}

		private void CheckStringRes(object sender, RoutedEventArgs e) => Binding.CheckNameStringRes();
		private void ChooseStringRes(object sender, RoutedEventArgs e) => MenuStringRes.IsOpen = true;

		private void ButtonChooseIcon(object sender, RoutedEventArgs e) => Binding.ChooseIcon(this);

		private void ButtonChooseColor(object sender, RoutedEventArgs e) {
			PopupColorPicker.PlacementTarget = (UIElement)sender;
			PopupColorPicker.IsOpen = true;
			string property;
			if (sender == ButtonBaseColor) {
				property = "BaseColor";
			} else if (sender == ButtonVariantBase) {
				property = "VariantColorBase";
			} else if (sender == ButtonVariant1) {
				property = "VariantColor1";
			} else if (sender == ButtonVariant2) {
				property = "VariantColor2";
			} else if (sender == ButtonVariant3) {
				property = "VariantColor3";
			} else if (sender == ButtonFlipColor) {
				property = "FlipColor";
			} else if (sender == ButtonFlakeColor) {
				property = "FlakeColor";
			} else if (sender == ButtonMaskR) {
				property = "MaskRColor";
			} else if (sender == ButtonMaskG) {
				property = "MaskGColor";
			} else if (sender == ButtonMaskB) {
				property = "MaskBColor";
			} else
				throw new ArgumentException("");
			Binding colorBinding = new() {
				Source = Binding,
				Path = new PropertyPath(property),
				Mode = BindingMode.TwoWay
			};
			TruckColorPicker.SetBinding(PickerControlBase.SelectedColorProperty, colorBinding);
		}

		private void ButtonClearClick(object sender, RoutedEventArgs e) {
			if (sender == ButtonIconNameClear) {
				Binding.IconName = "";
				Binding.ModelIcon = null;
			} else if (sender == ButtonPaintJobTexClear) {
				Binding.PaintJobTex = "";
				Binding.PaintJobTexImage = null;
			} else if (sender == ButtonBaseTexOverrideClear) {
				Binding.BaseTexOverride = "";
			} else if (sender == ButtonFlakeNoiseClear) {
				Binding.FlakeNoise = AccPaintJobData.DefaultFlakeNoise;
			} else if (sender == ButtonAccTexClear) {
				Binding.AccTex = "";
				Binding.AccTexImage = null;
			}
		}

		private void ButtonChooseTextureClick(object sender, RoutedEventArgs e) {
			int type;
			if (sender == ButtonPaintJob) {
				type = PaintJobBinding.TEX_PAINT_JOB;
			} else if (sender == ButtonBaseTexOverride) {
				type = PaintJobBinding.TEX_BASE_TEX_OVR;
			} else if (sender == ButtonChooseFlakeNoise) {
				type = PaintJobBinding.TEX_FLAKE_NOISE;
			} else if (sender == ButtonChooseAccTex) {
				type = PaintJobBinding.TEX_ACC_TEX;
			} else { return; }
			Binding.ChooseTex(this, type);
		}

		private void ColorVariantClick(object sender, RoutedEventArgs e) {
			if (PopupVariantList.IsOpen)
				return;
			PopupVariantList.IsOpen = true;
		}

		private void VariantColorSelected(object sender, SelectionChangedEventArgs e) {
			if (sender is ListBox) {
				if (PopupVariantList.IsOpen)
					PopupVariantList.IsOpen = false;
			}
		}

		private void AddVariantClick(object sender, RoutedEventArgs e) {
			Binding.AddColorVariant();
		}

		private void RemoveVariantClick(object sender, RoutedEventArgs e) {
			if (sender is Button button) {
				var variant = (ColorVariant) button.DataContext;
				Binding.RemoveColorVariant(variant);
			}
		}

		private void OvrTabClick(object sender, RoutedEventArgs e) {
			if (sender is Button button) {
				var ovrData = (PaintJobOverrideData)button.DataContext;
				Binding.ControlOvrTab(ovrData);
			}
		}

		private void AddOvrClick(object sender, RoutedEventArgs e) => Binding.AddOvr();

		private void AccessoryChecked(object sender, RoutedEventArgs e) {
			if (sender is CheckBox checkBox) {
				var acc = (Accessory)checkBox.DataContext;
				Binding.AccessoryChecked(acc);
			}
		}

		private void AccessoryUnchecked(object sender, RoutedEventArgs e) {
			if (sender is CheckBox checkBox) {
				var acc = (Accessory)checkBox.DataContext;
				Binding.AccessoryUnchecked(acc);
			}
		}

		private void FileDrop(object sender, DragEventArgs e) {

		}

		private void ButtonPaintJobPreviewClick(object sender, RoutedEventArgs e) {
			Binding.OpenPaintJobPreview(this);
		}

		private void ButtonStartClick(object sender, RoutedEventArgs e) {
			Binding.CreatePaintJobSii(this);
		}

		private void ListBoxItemRequestBringIntoViewHandler(object sender, RequestBringIntoViewEventArgs e) {
			e.Handled = true;
		}
    }
}


