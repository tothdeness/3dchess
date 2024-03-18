using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using test.Controllers;
using static Godot.HttpRequest;
using static test.Controllers.TableController;

namespace test.Pieces
{
	public abstract class Piece
	{
		//VERTICAL POS 3 (Y)
		public Node node { get; set; }
	  
		private string position;

		protected int y = 3;
		
		public Vector3 pos_vector { get; set; }

		
		//0 black, 1 white
		public int team { get; set; }


		public abstract List<AvailableMove> CheckValidMoves();

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

		public void Delete()
		{
			this.node.QueueFree();

			TableController.table.Remove(this);

		}


		protected Piece(string position, int team)
		{
			this.position = position;
			this.team = team;

			TableController.table.Add(this);

			Vector3 p = TableController.convert(position);
			p.Y = y;



			pos_vector = p;
			
		}

		 protected void setColor()
		{

			Dummy d = (Dummy)node;


			if(team == 1) {
				d.setColorWhite();
			}
			else
			{
				d.setColorBlack();
			}
			

		}



		protected List<AvailableMove> diagnolMoves(bool one_square)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			Vector3 ij = pos_vector;

			target t = new target();

			ij.Z = pos_vector.Z + 1;

			for (int i = (int)ij.X  + 1; i <= 8; i++)
			{
				ij.X = i;


				if (ij.Z == 9)
				{
					break;
				}

				t = TableController.find(ij, this);

					if ( t.num == 0)
					{
						results.Add(new AvailableMove(ij,false));
					}
					else if(t.num == 2)
					{
					results.Add(new AvailableMove(ij, true,t.p));
					break;
					}
					else { break; }


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

				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }


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

				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }

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

				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }

				if (one_square) { break; };

				ij.Z++;

			}

			return results;

		}


		protected List<AvailableMove> straightMoves(bool one_square)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			Vector3 ij = new Vector3();

			target t = new target();


			ij.Z = pos_vector.Z + 1;
			ij.X = pos_vector.X;

			for(int i = (int) ij.Z; i < 9 ; i++ ) {

				ij.Z = i;


				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }

				if (one_square) { break; };

			}

			ij.Z = pos_vector.Z - 1;


			for (int i = (int)ij.Z; i > 0; i--)
			{

				ij.Z = i;

				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }



				if (one_square) { break; };
			}

			ij.Z = pos_vector.Z;
			ij.X = pos_vector.X + 1;

			for (int i = (int)ij.X; i < 9; i++)
			{

				ij.X = i;

				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }

				if (one_square) { break; };
			}

			ij.X = pos_vector.X - 1;


			for (int i = (int)ij.X; i > 0; i--)
			{

				ij.X = i;

				t = TableController.find(ij, this);

				if (t.num == 0)
				{
					results.Add(new AvailableMove(ij, false));
				}
				else if (t.num == 2)
				{
					results.Add(new AvailableMove(ij, true, t.p));
					break;
				}
				else { break; }


				if (one_square) { break; };
			}




			return results;
		}






		protected List<AvailableMove> kingMoves()
		{
			List<AvailableMove> results = new List<AvailableMove>();

			results.AddRange(straightMoves(true));
			results.AddRange(diagnolMoves(true));

			return results;
		}

		protected List<AvailableMove> horseMoves()
		{
			List<AvailableMove> results = new List<AvailableMove>();

			int[] offsets = { 1, 2, -1, -2 };

			foreach (int x in offsets)
			{
				foreach (int z in offsets)
				{
					if (Math.Abs(x) != Math.Abs(z)) // Ensure L-shaped moves
					{
						
						
						Vector3 newPosition = new Vector3(pos_vector.X + x, pos_vector.Y, pos_vector.Z + z);

						
						AvailableMove a = IsValidPosition(newPosition);
						
						if(a != null)
						{
							results.Add(a);
						}



					}
				}
			}

			return results;
		}



		private AvailableMove IsValidPosition(Vector3 position)
		{
			int newX = (int)position.X;
			int newZ = (int)position.Z;


			Vector3 vec = new Vector3(newX, pos_vector.Y, newZ);

			target move = TableController.find(vec, this);



			if (newX >= 1 && newX <= 8 && newZ >= 1 && newZ <= 8 && move.num == 0)
			{
				return new AvailableMove(vec, false);

			}else if(newX >= 1 && newX <= 8 && newZ >= 1 && newZ <= 8 && move.num == 2)
			{
				
				return new AvailableMove(vec, true, move.p);
			}

			return null;

		}


		protected List<AvailableMove> pawnMoves()
		{
			List<AvailableMove> results = new List<AvailableMove>();


			Vector3 vec = new Vector3(pos_vector.X + 1, pos_vector.Y, pos_vector.Z);

			target move = TableController.find(vec,this);


			if (vec.X < 9 && move.num == 0)
			{
				results.Add(new AvailableMove(vec,false));

			}


			vec = new Vector3(pos_vector.X + 1, pos_vector.Y, pos_vector.Z + 1);
			move = TableController.find(vec, this);


			if (vec.X < 9 &&  vec.Z < 9 && move.num == 2)
			{
				results.Add(new AvailableMove(vec, true, move.p));

			}



			vec = new Vector3(pos_vector.X + 1, pos_vector.Y, pos_vector.Z - 1);
			move = TableController.find(vec, this);

			if (vec.X < 9 && vec.Z > 0 && move.num == 2)
			{
				results.Add(new AvailableMove(vec, true, move.p));

			}



			return results;
		}




	}
}
