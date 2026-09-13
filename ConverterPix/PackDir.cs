using SCS_Mod_Helper.Base;

namespace SCS_Mod_Helper.ConverterPix
{
    public class PackDir(string dirName, string? title): BaseBinding {

        public PackDir(string dirName) : this(dirName, null) {
		}

		private string mDirName = dirName;
        public string DirName {
            get => mDirName;
            set {
                mDirName = value;
                InvokeChange();
            }
        }

        public string Title {
            get {
                if (title != null)
                    return title;
				int lastSlash = mDirName.LastIndexOf('/');
                if (lastSlash >= 0)
                    return mDirName[(lastSlash + 1)..];
                return mDirName;
            }
		}
	}
}
