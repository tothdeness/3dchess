using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;

namespace test.Controllers
{
	internal class TableController
	{
		private int tableSize;
		private Piece[][] table;



















		public int[] convert(string coordinate)
		{
			int[] result = new int[2];

			result[0] = coordinate[0] - 'A' + 1;

			result[1] = int.Parse(coordinate[1].ToString());

			return result;
		}

		public string convertReverse(int[] cord)
		{
			return $"{(char)cord[0]}{cord[1]}";
		}



	}
}
