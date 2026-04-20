using System;
using System.Collections.Generic;

namespace GenioMVC.ViewModels.Shared
{
	public struct Coord
	{
		public int X { get; set; }

		public int Y { get; set; }

		public Coord(double c) : this()
		{
			this.X = (int)Math.Floor(c);

			// Uma espécie de batota to obter a parte decimal como um inteiro, mas foi a única forma que encontrei de funcionar
			// (int) (c - Math.Floor(c)) NÃO funciona em todos os casos, nem com floor nem truncate. Usar uma precisão arbitrária é menos eficiente
			if (c == 0)
				this.Y = 0;
			else
			{
				string strAux = c.ToString();
				string[] parts = strAux.Split(new string[] { System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator }, StringSplitOptions.None);
				this.Y = (parts.Length > 1) ? Int32.Parse(parts[1]) : Int32.Parse(parts[0]);
			}
		}

		public Coord(int x, int y) : this()
		{
			this.X = x;
			this.Y = y;
		}
	}

	public class Element
	{
		public string Cod { get; set; }

		public string Acronym { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public string Form { get; set; }

		public Coord Position { get; set; }

		public Coord Predecessor { get; set; }

		public IList<Element> entrance;

		public IList<Element> exit;

		public Element()
		{
			this.entrance = new List<Element>();
			this.exit = new List<Element>();
		}

		public void AddElementEntrance(Element elem)
		{
			this.entrance.Add(elem);
		}

		public void AddElementExit(Element elem)
		{
			this.exit.Add(elem);
		}
	}
}
