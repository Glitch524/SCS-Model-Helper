using SCS_Mod_Helper.Base;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace SCS_Mod_Helper.Modding.PaintJob {
	public class PaintJobOverrideData(string ovrName): BaseBinding {
		public string OvrName = ovrName;

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

		private readonly ObservableCollection<string> mAccList = [];
		public ObservableCollection<string> AccList => mAccList;
	}
}
