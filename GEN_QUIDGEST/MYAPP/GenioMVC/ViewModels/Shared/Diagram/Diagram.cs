using System;
using System.Collections.Generic;

namespace GenioMVC.ViewModels.Shared
{
	public class Diagram
	{
		public List<Element> elements;

		public string name { get; set; }

		public void AddElement(Element elem)
		{
			this.elements.Add(elem);
		}

		public Diagram()
		{
			this.elements = new List<Element>();
		}
	}
}
