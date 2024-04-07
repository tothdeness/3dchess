using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;

namespace test.Controllers
{
	public static class ViewController
	{
		public static List<Piece> pressed = new List<Piece>();


		public static void deletePressed()
		{

			foreach(var move in pressed)
			{
				move.pressed = false;
				move.moves.Clear();
			}

			pressed.Clear();

		}




	}
}
