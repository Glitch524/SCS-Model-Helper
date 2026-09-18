using System.Text;
using System.Xml;

namespace SCS_Mod_Helper.Base;

public abstract class AppIO {
	protected XmlDocument doc = new();
	public AppIO() {

	}

	protected void CreateXmlDeclaration() {
		doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
	}

	//protected XmlElement CreateElement(string elementName) {
	//	return doc.CreateElement(elementName);
	//}

	//protected void CreateAttribute(XmlElement element, string attrName, string value) {
	//	XmlAttribute attr = doc.CreateAttribute(attrName);
	//	attr.Value = value;
	//	element.Attributes.Append(attr);
	//}

	protected XmlElement? currentNode = null;
	protected void WriteElement(string name, Action action) {
		var formerNode = currentNode;
		var element = doc.CreateElement(name);
		if (currentNode == null)
			doc.AppendChild(element);
		else
			currentNode.AppendChild(element);
		currentNode = element;
		action();
		currentNode = formerNode;
	}
	protected void WriteValueElement(string name, string value) {
		if (SkipCreatingIFEmpty && string.IsNullOrEmpty(value)) 
			return;
		var element = doc.CreateElement(name);
		element.InnerText = value;
		if (currentNode == null)
			doc.AppendChild(element);
		else
			currentNode.AppendChild(element);
	}

	protected void WriteValueElement(string name, int value) => WriteValueElement(name, value.ToString());
	protected void WriteValueElement(string name, long value) => WriteValueElement(name, value.ToString());

	protected void WriteAttribute(string attrName, string value) {
		if (SkipCreatingIFEmpty && string.IsNullOrEmpty(value))
			return;
		if (currentNode == null)
			return;
		XmlAttribute attr = doc.CreateAttribute(attrName);
		attr.Value = value;
		currentNode.Attributes.Append(attr);
	}
	protected void WriteAttribute(string name, int value) => WriteAttribute(name, value.ToString());
	protected void WriteAttribute(string name, long value) => WriteAttribute(name, value.ToString());
	protected void WriteAttribute(string name, bool value) => WriteAttribute(name, value.ToString());

	protected bool SkipCreatingIFEmpty = false;
	protected XmlWriterSettings settings = new() {
		Indent = true,
		NewLineOnAttributes = true,
		IndentChars = "\t",
		NewLineChars = "\r\n",
		Encoding = Encoding.UTF8
	};

	protected void SaveDocument(string path) {
		using var writer = XmlWriter.Create(path, settings);
		doc.Save(writer);
	}

	protected string GetAttribute(XmlNode node, string attrProperty) => ((XmlElement)node).GetAttribute(attrProperty);
	protected bool GetAttributeBool(XmlNode node, string attrProperty) => bool.Parse(GetAttribute(node, attrProperty));
	protected int GetAttributeInt(XmlNode node, string attrProperty) => int.Parse(GetAttribute(node, attrProperty));
	protected long GetAttributeLong(XmlNode node, string attrProperty) => long.Parse(GetAttribute(node, attrProperty));
}
