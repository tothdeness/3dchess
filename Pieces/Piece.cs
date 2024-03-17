using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

		private int y = 3;
		
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


			vector.Y = y;


			this.node.Set("position", vector);



	
			//GD.Print("Uj pozizicio: " + position);
			//GD.Print("test: "+pos_vector.X +"  "+ pos_vector.Z);
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

		protected List<Vector3> diagnolMoves(bool one_square)
		{
			List<Vector3> results = new List<Vector3>();

			Vector3 ij = pos_vector;


			ij.Z = pos_vector.Z + 1;

		


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


				if (one_square) { break; };

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

				if (one_square) { break; };

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

				if (one_square) { break; };

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

				if (one_square) { break; };

				ij.Z++;

			}

			return results;

		}


		protected List<Vector3> straightMoves(bool one_square)
		{
			List<Vector3> results = new List<Vector3>();

			Vector3 ij = new Vector3();


			ij.Z = pos_vector.Z + 1;
			ij.X = pos_vector.X;

			for(int i = (int) ij.Z; i < 9 ; i++ ) {

				ij.Z = i;

				if (!TableController.find(ij))
				{
					results.Add(ij);
				}
				else
				{
					break;
				}

				if (one_square) { break; };

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
				if (one_square) { break; };
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
				if (one_square) { break; };
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
				if (one_square) { break; };
			}




			return results;
		}






		protected List<Vector3> kingMoves()
		{
			List<Vector3> results = new List<Vector3>();

			results.AddRange(straightMoves(true));
			results.AddRange(diagnolMoves(true));

			return results;
		}

		protected List<Vector3> horseMoves()
		{
			List<Vector3> results = new List<Vector3>();

			int[] offsets = { 1, 2, -1, -2 };

			foreach (int x in offsets)
			{
				foreach (int z in offsets)
				{
					if (Math.Abs(x) != Math.Abs(z)) // Ensure L-shaped moves
					{
						Vector3 newPosition = new Vector3(pos_vector.X + x, pos_vector.Y, pos_vector.Z + z);

						if (IsValidPosition(newPosition))
						{
							results.Add(newPosition);
						}
					}
				}
			}

			return results;
		}

		private bool IsValidPosition(Vector3 position)
		{
			int newX = (int)position.X;
			int newZ = (int)position.Z;

			return newX >= 1 && newX <= 8 && newZ >= 1 && newZ <= 8 && !TableController.find(position);
		}






	}
}
