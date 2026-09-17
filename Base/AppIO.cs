using System.Text;
using System.Xml;

namespace SCS_Mod_Helper.Base;

public abstract class AppIO {
	public AppIO() {

	}
	protected XmlDocument doc = new();

	protected XmlElement CreateElement(string elementName) {
		return doc.CreateElement(elementName);
	}

	protected void CreateAttribute(XmlElement element, string attrName, string value) {
		XmlAttribute attr = doc.CreateAttribute(attrName);
		attr.Value = value;
		element.Attributes.Append(attr);
	}


	protected XmlWriterSettings settings = new() {
		Indent = true,
		IndentChars = "\t",
		NewLineChars = "\r\n",
		Encoding = Encoding.UTF8
	};

	protected void SaveDocument(string path) {
		using var writer = XmlWriter.Create(path, settings);
		doc.Save(writer);
	}

	protected string GetAttribute(XmlNode node, string attrProperty) => ((XmlElement)node).GetAttribute(attrProperty);
}
