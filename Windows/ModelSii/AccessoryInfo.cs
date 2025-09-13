using Def_Writer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Def_Writer.Windows.ModelSii {
	public class AccessoryInfo(string accessory, string name, bool ets2, bool ats): INotifyPropertyChanged {
		private string MAccessory = accessory;
		private string MName = name;
		private bool METS2 = ets2;
		private bool MATS = ats;

		public event PropertyChangedEventHandler? PropertyChanged;

		public AccessoryInfo(string accessory, bool ets2, bool ats) : this(accessory, Util.GetString($"Acc.{accessory}"), ets2, ats) {

		}

		public string Accessory {
			get => MAccessory;
			set {
				MAccessory = value;
				InvokeChange(nameof(Accessory));
			}
		}

		public string Name {
			get => MName;
			set {
				MName = value;
				InvokeChange(nameof(Name));
			}
		}
		public void RefreshName() => Name = Util.GetString($"Acc.{Accessory}");

		public bool ETS2 {
			get => METS2;
			set {
				METS2 = value;
				InvokeChange(nameof(ETS2));
			}
		}

		public bool ATS {
			get => MATS;
			set {
				MATS = value;
				InvokeChange(nameof(ATS));
			}
		}

		public void InvokeChange(string name) {
			PropertyChanged?.Invoke(this, new(name));
		}
	}
}
