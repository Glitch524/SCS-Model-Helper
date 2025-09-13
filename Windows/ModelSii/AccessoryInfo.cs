using Def_Writer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Def_Writer.Windows.ModelSii {
	public class AccessoryInfo(string accessory, string name, bool forEts2, bool forAts): INotifyPropertyChanged {
		private string MAccessory = accessory;
		private string MName = name;
		private bool MForETS2 = forEts2;
		private bool MForATS = forAts;

		public event PropertyChangedEventHandler? PropertyChanged;

		public AccessoryInfo(string accessory, bool forEts2, bool forAts) : this(accessory, Util.GetString($"Acc.{accessory}"), forEts2, forAts) {

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

		public bool ForETS2 {
			get => MForETS2;
			set {
				MForETS2 = value;
				InvokeChange(nameof(ForETS2));
			}
		}

		public bool ForATS {
			get => MForATS;
			set {
				MForATS = value;
				InvokeChange(nameof(ForATS));
			}
		}

		public void InvokeChange(string name) {
			PropertyChanged?.Invoke(this, new(name));
		}
	}
}
