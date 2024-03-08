using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces
{
	internal abstract class Piece
	{

		protected Node node { get; set; }
	  
		private string position;

		public int x { get; set; }
		
		public int y { get; set; }

		//0 black, 1 white
		private int team { get; set; }

		public static TableController tableController;

		public abstract List<Vector2> CheckValidMoves();

		public abstract void ShowValidMoves();

		public void DeleteVisualizers() {

			tableController.removeVisualizers();

		}

		public abstract void Move(int x, int y);

		protected Piece(string position, int team)
		{
			this.position = position;
			this.team = team;

			tableController.table.Add(this); 

			x = (int)TableController.convert(position).X;
			y = (int)TableController.convert(position).Y;



			GD.Print(" Numbers: " + x + " " + y);

		}

		protected List<Vector2> diagnolMoves()
		{
			List<Vector2> results = new List<Vector2>();

	
			for(int i = x+1; i <= 8; i++)
			{
				for(int j = y+1; j <= 8; j++)
				{
					if (!tableController.find(i,j))
					{
						results.Add(new Vector2(i, j));
					}
					else
					{
						break;
					}

				}
			}



			return results;
		}




	}
}
