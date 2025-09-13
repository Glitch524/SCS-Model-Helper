using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Def_Writer.Windows
{
    class AccessoryItem {
		public AccessoryItem() {
		}

		private string MDisplayName;
		public string DisplayName {
			get {
				return MDisplayName;
			}
			set {
				MDisplayName = value;
				InvokeChange(nameof(DisplayName));
			}
		}

		private string MPrice;
		public string Price {
			get {
				return MPrice;
			}
			set {
				MPrice = value;
				InvokeChange(nameof(Price));
			}
		}

		private string MUnlockLevel;
		public string UnlockLevel {
			get {
				return MUnlockLevel;
			}
			set {
				MUnlockLevel = value;
				InvokeChange(nameof(UnlockLevel));
			}
		}

		private string MCollisionPath;
		public string CollisionPath {
			get {
				return MCollisionPath;
			}
			set {
				MCollisionPath = value;
				InvokeChange(nameof(CollisionPath));
			}
		}

		private string MModelName;
		public string ModelName {
			get {
				return MModelName;
			}
			set {
				MModelName = value;
				InvokeChange(nameof(ModelName));
			}
		}

		private string MIconName;
		public string IconName {
			get {
				return MIconName;
			}
			set {
				MIconName = value;
				InvokeChange(nameof(IconName));
			}
		}

		private string MLook;
		public string Look {
			get {
				return MLook;
			}
			set {
				MLook = value;
				InvokeChange(nameof(Look));
			}
		}

		private string MVariant;
		public string Variant {
			get {
				return MVariant;
			}
			set {
				MVariant = value;
				InvokeChange(nameof(Variant));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public void InvokeChange(string name) {
			PropertyChanged?.Invoke(this, new(name));
		}
	}
}
