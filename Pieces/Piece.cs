using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
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


		public abstract List<AvailableMove> CheckValidMovesWithCover();


		public abstract List<AvailableMove> CheckValidMovesWithKingProtection();


		public void ShowValidMoves()
		{
			TableController.showVisualizers(TableController.calculateVisualizers(CheckValidMovesWithKingProtection()),this);
		}

		public void DeleteVisualizers() {

			TableController.removeVisualizers();

		}

		public void Move(Vector3 vector) {

			this.pos_vector = TableController.reversePosition(vector);


			this.position = TableController.convertReverse(pos_vector);


			vector.Y = y;


			this.node.Set("position", vector);


			kingIsInCheckEnemy();

	
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


		private AvailableMove check(Vector3 ij,bool kingProtect = false)
		{

			var t = TableController.find(ij, this);


			Vector3 old = pos_vector;

			if (!kingProtect)
			{

				if (t.num == 0)
				{
					return new AvailableMove(ij, false);
				}
				else if (t.num == 2)
				{
					return new AvailableMove(ij, true, t.p);
				}

			}
			else
			{

				pos_vector = ij;



				if (t.num == 0 && !kingIsInCheckSame())
				{
					pos_vector = old;
					return new AvailableMove(ij, false);
				}
				else if (t.num == 2 && !kingIsInCheckSame(t.p))
				{
					pos_vector = old;
					return new AvailableMove(ij, true, t.p);
				}else if(t.num == 1)
				{
					pos_vector = old;
					return new AvailableMove(new Vector3(999,999,999), false);
				}




			}





			pos_vector = old;

			return null;
		}



		private AvailableMove checkWithCover(Vector3 ij)
		{

			var t = TableController.find(ij, this);

			if (t.num == 0)
			{
				return new AvailableMove(ij, false);
			}
			else if (t.num == 2)
			{
				return new AvailableMove(ij, true, t.p);
			}

			return new AvailableMove(ij, false, true);
		}




		protected List<AvailableMove> diagnolMoves(bool one_square,bool cover,bool kingProtect = false)
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


				AvailableMove a = check(ij,kingProtect);


				if (cover) { a = checkWithCover(ij);}

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } } 
			

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


				AvailableMove a = check(ij, kingProtect);
				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }


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

				AvailableMove a = check(ij, kingProtect);
				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);

					if(a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }


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

				AvailableMove a = check(ij, kingProtect);
				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }

				if (one_square) { break; };

				ij.Z++;

			}

			return results;

		}


		protected List<AvailableMove> straightMoves(bool one_square,bool cover,bool kingProtect = false)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			Vector3 ij = new Vector3();

			target t = new target();


			ij.Z = pos_vector.Z + 1;
			ij.X = pos_vector.X;

			for(int i = (int) ij.Z; i < 9 ; i++ ) {

				ij.Z = i;

				AvailableMove a = check(ij, kingProtect);


				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }



				if (one_square) { break; };

			}

			ij.Z = pos_vector.Z - 1;


			for (int i = (int)ij.Z; i > 0; i--)
			{

				ij.Z = i;

				AvailableMove a = check(ij, kingProtect);


				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }

				if (one_square) { break; };
			}

			ij.Z = pos_vector.Z;
			ij.X = pos_vector.X + 1;

			for (int i = (int)ij.X; i < 9; i++)
			{

				ij.X = i;

				AvailableMove a = check(ij, kingProtect);


				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }

				if (one_square) { break; };
			}

			ij.X = pos_vector.X - 1;


			for (int i = (int)ij.X; i > 0; i--)
			{

				ij.X = i;

				AvailableMove a = check(ij, kingProtect);


				if (cover) { a = checkWithCover(ij); }

				if (a != null)
				{
					results.Add(a);
					if (a.attack || a.cover || a.move.X == 999) { break; }
				}
				else { if (!kingProtect) { break; } }


				if (one_square) { break; };
			}




			return results;
		}



		protected bool kingIsInCheckEnemy()
		{

			foreach (Piece piec in TableController.table)
			{
				if (this.team == piec.team)
				{
					foreach(AvailableMove move in piec.CheckValidMoves())
					{


						if(move.target is King)
						{
			
							return true;
						}
					}
				}

			}

		
			return false;
		}


		protected bool kingIsInCheckSame(Piece target = null)
		{

			if(target != null)
			{
				TableController.table.Remove(target);
			}


			foreach (Piece piec in TableController.table)
			{
				if (this.team != piec.team)
				{

	


					foreach (AvailableMove move in piec.CheckValidMoves())
					{

						if (move.target is King)
						{

							if (target != null)
							{
								TableController.table.Add(target);
							}
							
							return true;
						}
					}
				}

			}


			if (target != null)
			{
				TableController.table.Add(target);
			}

			return false;
		}



		protected bool kingCheckWithoutKing()
		{

			foreach (Piece piec in TableController.table)
			{
				if (this.team != piec.team && !(piec is King) )
				{
					foreach (AvailableMove move in piec.CheckValidMoves())
					{

	

						if (move.target is King)
						{
							return true;
						}
					}
				}

			}

			return false;
		}








		protected List<AvailableMove> kingMoves()
		{
			List<AvailableMove> results = new List<AvailableMove>();
			List<AvailableMove> notavailable = new List<AvailableMove>();
			List<AvailableMove> toRemove = new List<AvailableMove>();

			results.AddRange(straightMoves(true, false));
			results.AddRange(diagnolMoves(true, false));

			Vector3 old = pos_vector;

			foreach (Piece piec in TableController.table)
			{
				if (this.team != piec.team)
				{
					notavailable.AddRange(piec.CheckValidMovesWithCover());
				}
			}

			for (int i = 0; i < results.Count(); i++)
			{
				foreach (AvailableMove na in notavailable)
				{

					pos_vector = new Vector3(results[i].move.X,3, results[i].move.Z);

					if (results[i].move.X == na.move.X && results[i].move.Z == na.move.Z || kingCheckWithoutKing() )
					{
						toRemove.Add(results[i]);
					}
				}
			}

			pos_vector = old;

			foreach (AvailableMove move in toRemove)
			{
				results.Remove(move);
			}

			return results;
		}


		protected List<AvailableMove> horseMoves(bool cover, bool kingProtect = false)
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


						AvailableMove a = IsValidPosition(newPosition,cover,kingProtect);
						
						if(a != null)
						{
							results.Add(a);
						}

						

					}
				}
			}

			return results;
		}



		private AvailableMove IsValidPosition(Vector3 position,bool cover, bool kingProtect = false)
		{
			int newX = (int)position.X;
			int newZ = (int)position.Z;

			Vector3 old = new Vector3();

			if (kingProtect) { old = pos_vector; }


			Vector3 vec = new Vector3(newX, pos_vector.Y, newZ);


			target move = TableController.find(vec, this);


			if (kingProtect) { pos_vector = vec; }

			bool first = newX >= 1 && newX <= 8 && newZ >= 1 && newZ <= 8;

			bool second = newX >= 1 && newX <= 8 && newZ >= 1 && newZ <= 8;


			if (!kingProtect)
			{

				if (first && move.num == 0 || cover && first)
				{
					return new AvailableMove(vec, false);

				}
				else if (second && move.num == 2 || cover && second)
				{

					return new AvailableMove(vec, true, move.p);
				}

			}
			else
			{


				if (first && move.num == 0 && !kingIsInCheckSame())
				{

					pos_vector = old;
					return new AvailableMove(vec, false);

				}
				else if (second && move.num == 2 && !kingIsInCheckSame(move.p))
				{
		
					pos_vector = old;
					return new AvailableMove(vec, true, move.p);
				}

			}


			if (kingProtect) { pos_vector = old; };

			return null;

		}


		protected List<AvailableMove> pawnMoves(bool cover,bool kingProtection)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			Vector3 old = this.pos_vector;

			Vector3 vec = new Vector3(pos_vector.X + 1, pos_vector.Y, pos_vector.Z);

			target move = TableController.find(vec,this);


			bool checkKing = false;

			if (kingProtection)
			{
				pos_vector = vec;
				checkKing = kingIsInCheckSame();
			}


			if (!kingProtection)
			{
				if (vec.X < 9 && move.num == 0 && !cover)
				{
					results.Add(new AvailableMove(vec, false));
				}
			}
			else
			{

				if (vec.X < 9 && move.num == 0 && !checkKing)
				{
					results.Add(new AvailableMove(vec, false));
				}
				pos_vector = old;

			}

		


			vec = new Vector3(pos_vector.X + 1, pos_vector.Y, pos_vector.Z + 1);
			move = TableController.find(vec, this);



			if (!kingProtection)
			{
				if (vec.X < 9 && vec.Z < 9 && move.num == 2 || vec.X < 9 && vec.Z < 9 && cover)
				{
					results.Add(new AvailableMove(vec, true, move.p));

				}
			}
			else
			{

				pos_vector = vec;
				checkKing = kingIsInCheckSame(move.p);

				if (vec.X < 9 && vec.Z < 9 && move.num == 2 && !checkKing)
				{
					results.Add(new AvailableMove(vec, true, move.p));

				}
				pos_vector = old;

			}

			vec = new Vector3(pos_vector.X + 1, pos_vector.Y, pos_vector.Z - 1);
			move = TableController.find(vec, this);


			if (!kingProtection)
			{

				if (vec.X < 9 && vec.Z > 0 && move.num == 2 || vec.X < 9 && vec.Z < 9 && cover)
				{
					results.Add(new AvailableMove(vec, true, move.p));

				}

			}
			else
			{

				pos_vector = vec;
				checkKing = kingIsInCheckSame(move.p);

				if (vec.X < 9 && vec.Z > 0 && move.num == 2 && !checkKing)
				{
					results.Add(new AvailableMove(vec, true, move.p));

				}

				pos_vector = old;

			}



			return results;
		}







	}
}
