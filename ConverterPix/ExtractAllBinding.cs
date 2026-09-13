using SCS_Mod_Helper.Base;
using SCS_Mod_Helper.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCS_Mod_Helper.ConverterPix
{
    class ExtractAllBinding: BaseBinding
    {
        private string mDialogMessage = Util.GetString("MessageGatherFiles");
        public string DialogMessage {
            get => mDialogMessage;
            set {
                mDialogMessage = value;
                InvokeChange();
            }
        }

        private bool mPrepareMode = true;
        public bool PrepareMode {
            get => mPrepareMode;
            set {
                mPrepareMode = value;
                InvokeChange();
            }
        }

		private int mStepCurrent = 0;
        public int StepCurrent {
            get => mStepCurrent;
            set {
                mStepCurrent = value;
                InvokeChange();
                InvokeChange(nameof(StepText));
            }
		}

		private int mStepMax = 5;
		public int StepMax {
			get => mStepMax;
			set {
				mStepMax = value;
				InvokeChange();
				InvokeChange(nameof(StepText));
			}
		}
        public string StepText => $"{StepCurrent}/{StepMax}";

        private string mCurrentFile = "";
        public string CurrentFile {
            get => mCurrentFile;
            set {
                mCurrentFile = value;
                InvokeChange();
            }
        }

        private int mProgressCurrent = 0;
        public int ProgressCurrent {
            get => mProgressCurrent;
            set {
                mProgressCurrent = value;
                InvokeChange();
                InvokeChange(nameof(ProgressText));
            }
        }
        private int mProgressMax = 1;
        public int ProgressMax {
            get => mProgressMax;
            set {
                mProgressMax = value;
                InvokeChange();
				InvokeChange(nameof(ProgressText));
			}
        }

        public delegate void DPrepareMode(bool prepareMode);
        public delegate void DStepValue(int current);
        public delegate void DMessage(string message);
        public delegate void DProgress(int current, int max);
		public delegate void DProgressValue(int current);
		public delegate void DCurrentFile(string file);
        public delegate void DOnFinished();

        public string ProgressText => $"{ProgressCurrent}/{ProgressMax}";
	}
}
