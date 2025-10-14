using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Mod_Helper.Main;
public class BasicInfo {
	private string mProjectLocation = ModBasic.Default.ProjectLocation;
	public string ProjectLocation {
		get => mProjectLocation; set => mProjectLocation = value;
	}

	private string mAuthor = ModBasic.Default.Author;
	public string Author {
		get => mAuthor; set => mAuthor = value;
	}

	public void Save() {
		ModBasic.Default.ProjectLocation = ProjectLocation;
		ModBasic.Default.Author = Author;
		ModBasic.Default.Save();
	}
}
