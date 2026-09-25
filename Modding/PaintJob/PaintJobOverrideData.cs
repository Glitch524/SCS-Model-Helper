using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Trucks;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Modding.PaintJob {
	public class PaintJobOverrideData(int index): BaseBinding {

		public override string ToString() => OvrName;

		private int mIndex = index;
		public int Index {
			get => mIndex;
			set {
				mIndex = value;
				InvokeChange();
				InvokeChange(nameof(OvrName));
			}
		}

		public string OvrName => ".ovr" + Index;

		private string mAccTexture = "";
		public string AccTex {
			get => mAccTexture;
			set {
				mAccTexture = value;
				InvokeChange();
			}
		}

		protected BitmapSource? mAccTextureImage = null;
		public BitmapSource? AccTexImage {
			get => mAccTextureImage;
			set {
				mAccTextureImage?.Freeze();
				mAccTextureImage = value;
				InvokeChange();
			}
		}

		private float mAccFlakeUVScale = 32f;

		public float AccFlakeUVScale {
			get => mAccFlakeUVScale;
			set {
				mAccFlakeUVScale = value;
				InvokeChange();
			}
		}

		private float mAccFlakeVRatio = 1f;
		public float AccFlakeVRatio {
			get => mAccFlakeVRatio;
			set {
				mAccFlakeVRatio = value;
				InvokeChange();
			}
		}

		public readonly ObservableCollection<Accessory> AccList = [];

		public void CheckAccList(bool check) {
			foreach(var acc in AccList) {
				acc.Check = check;
			}
		}
	}
}
