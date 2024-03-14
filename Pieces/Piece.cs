using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using static Godot.HttpRequest;

namespace test.Pieces
{
	internal abstract class Piece
	{
		//VERTICAL POS 3 (Y)
		public Node node { get; set; }
	  
		private string position;


		
		public Vector3 pos_vector { get; set; }

		
		//0 black, 1 white
		private int team { get; set; }


		public abstract List<Vector3> CheckValidMoves();

		public void ShowValidMoves()
		{
			TableController.showVisualizers(TableController.calculateVisualizers(CheckValidMoves()),this);
		}

		public void DeleteVisualizers() {

			TableController.removeVisualizers();

		}

		public void Move(Vector3 vector) {

			this.pos_vector = TableController.reversePosition(vector);


			this.position = TableController.convertReverse(pos_vector);
			

			this.node.Set("position", vector);

	
			GD.Print("Uj pozizicio: " + position);
			GD.Print("test: "+pos_vector.X +"  "+ pos_vector.Z);
		}

		protected Piece(string position, int team)
		{
			this.position = position;
			this.team = team;

			TableController.table.Add(this);

			Vector3 p = TableController.convert(position);

			p.Y = 3;

			pos_vector = p;
			
		

		}

		protected List<Vector3> diagnolMoves()
		{
			List<Vector3> results = new List<Vector3>();

			Vector3 ij = pos_vector;


			ij.Z = pos_vector.Z + 1;

			GD.Print("Current pos: " +ij.X + " " + ij.Z);


			for (int i = (int)ij.X  + 1; i <= 8; i++)
			{
				ij.X = i;


				if (ij.Z == 9)
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

				ij.Z++;
				
			}

			ij.Z = pos_vector.Z - 1;
			ij.X = pos_vector.X;

			for (int i = (int)ij.X - 1; i >= 1; i--)
			{
				ij.X = i;
				if (ij.Z == 0)
				{
					break;
				}

				if (!TableController.find(ij))
				{
					results.Add(ij);
					GD.Print(ij.X + " " + ij.Z);
				}
				else
				{
					break;
				}

				ij.Z--;

			}


			ij.Z = pos_vector.Z - 1;
			ij.X = pos_vector.X;

			for (int i = (int)ij.X + 1; i <= 8; i++)
			{
				ij.X = i;
				if (ij.Z == 0)
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

				ij.Z--;

			}

			ij.Z = pos_vector.Z + 1;
			ij.X = pos_vector.X;

			for (int i = (int)ij.X - 1; i >= 1; i--)
			{
				ij.X = i;

				if (ij.Z == 9)
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

				ij.Z++;

			}

			return results;

		}


		protected List<Vector3> straightMoves()
		{
			List<Vector3> results = new List<Vector3>();

			Vector3 ij = new Vector3();


			ij.Z = pos_vector.Z + 1;
			ij.X = pos_vector.X;

			for(int i = (int) ij.Z; i < 9; i++ ) {

				ij.Z = i;

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

			}

			ij.Z = pos_vector.Z - 1;


			for (int i = (int)ij.Z; i > 0; i--)
			{

				ij.Z = i;

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

			}

			ij.Z = pos_vector.Z;
			ij.X = pos_vector.X + 1;

			for (int i = (int)ij.X; i < 9; i++)
			{

				ij.X = i;

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

			}

			ij.X = pos_vector.X - 1;


			for (int i = (int)ij.X; i > 0; i--)
			{

				ij.X = i;

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

			}




			return results;
		}

	}
}
