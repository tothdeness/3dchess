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

		public Node node { get; set; }
	  
		private string position;

		public int x { get; set; }
		
		public int y { get; set; }

		//0 black, 1 white
		private int team { get; set; }


		public abstract List<Vector2> CheckValidMoves();

		public void ShowValidMoves()
		{
			TableController.showVisualizers(TableController.calculateVisualizers(CheckValidMoves()),this);
		}

		public void DeleteVisualizers() {

			TableController.removeVisualizers();

		}

		public void Move(Vector3 vector) {

			Vector2 vec = TableController.reversePosition(vector);

			GD.Print(vector.Y + " | " + vector.X + " | " + position);

			this.position = TableController.convertReverse(vec);
			this.x = (int) vec.X;
			this.y = (int) vec.Y;
			this.node.Set("position", vector);

			GD.Print(this.y + " | " + this.x +" | "+position);


		}

		protected Piece(string position, int team)
		{
			this.position = position;
			this.team = team;

			TableController.table.Add(this); 

			x = (int) TableController.convert(position).X;
			y = (int) TableController.convert(position).Y;



			GD.Print(" Numbers: " + x + " " + y);

		}

		protected List<Vector2> diagnolMoves()
		{
			List<Vector2> results = new List<Vector2>();

			Vector2 ij = new Vector2();

			int j = y + 1;

			ij.Y = j;
			

			for (int i = x + 1; i <= 8; i++)
			{
				ij.X = i;


				if (ij.Y == 9)
				{
					break;
				}

					if (!TableController.find(ij))
					{
						results.Add(ij);
					}
					else
					{
						break;
					}

				ij.Y++;
				
			}

			ij.Y = y - 1;

			for (int i = x - 1; i >= 1; i--)
			{
				ij.X = i;
				if (ij.Y == 0)
				{
					break;
				}

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

				ij.Y--;

			}


			ij.Y = y - 1;

			for (int i = x + 1; i <= 8; i++)
			{
				ij.X = i;
				if (ij.Y == 0)
				{
					break;
				}

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

				ij.Y--;

			}

			ij.Y = y + 1;

			for (int i = x - 1; i >= 1; i--)
			{
				ij.X = i;

				if (ij.Y == 9)
				{
					break;
				}

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

				ij.Y++;

			}

			return results;

		}




	}
}
