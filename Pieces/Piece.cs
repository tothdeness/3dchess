using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
	public  class Piece
	{
		//VERTICAL POS 3 (Y)
		public Node node { get; set; }
	  
		public string position;

		protected int y = 3;

		public bool firstMove = true;

		public Vector3 pos_vector { get; set; }
	
		//-1 black, 1 white
		public int team { get; set; }

		public bool pressed = false;

		public List<AvailableMove> moves = new List<AvailableMove>();

		public bool enPassant = false;

		public Pawn enPassantRight;

		public Pawn enPassantLeft;

		public int covered = 0;

		public int hit = 0;

		public virtual List<AvailableMove> CheckValidMoves(bool special) { throw new NotImplementedException(); }


		public virtual  List<AvailableMove> CheckValidMovesWithCover() { throw new NotImplementedException(); }


		public virtual  List<AvailableMove> CheckValidMovesWithKingProtection() { throw new NotImplementedException(); }


		public void ShowValidMoves()
		{
			if (!pressed) {

				var watch = new Stopwatch();
				




				watch.Start();

				moves.AddRange(TableController.calculateVisualizers(CheckValidMovesWithKingProtection()));

				//for(int i = 0;i < 30000; i++)
				//{
				//	CheckValidMovesWithKingProtection();
				//}


				watch.Stop();

				GD.Print(watch.ElapsedMilliseconds);

				//GD.Print( (float) watch.ElapsedMilliseconds / 30000);

				pressed = true;
				ViewController.pressed.Add(this);
			}



			TableController.showVisualizers(moves, this);
	
			
		}

		public void DeleteVisualizers() {

			TableController.removeVisualizers();

		}

		public void Move(Vector3 vector,AvailableMove move) {




			this.pos_vector = TableController.reversePosition(vector);


			this.position = TableController.convertReverse(pos_vector);


			vector.Y = y;


			this.node.Set("position", vector);

			if(this is Pawn)
			{
				Pawn p = (Pawn) this;

				p.promotePawn();

			}



			if (this is King && (move.kingSideCastling || move.queenSideCastling))

			{ move.target.Move(TableController.calculatePosition(move.rookNewPos), new AvailableMove(move.target, move.rookNewPos, false)); }

			emptyEnpassant();

			ViewController.deletePressed();

			if (firstMove)
			{
				firstMove = false;

				if (move.enPassant)
				{

					if(move.canEnPassantLeft is not null)
					{
						move.canEnPassantLeft.enPassantLeft = (Pawn)this;
						move.canEnPassantLeft.enPassant = true;
						TableController.enPassant.Add(move.canEnPassantLeft);
					}

					if(move.canEnPassantRight is not null)
					{
						move.canEnPassantRight.enPassantRight = (Pawn)this;
						move.canEnPassantRight.enPassant = true;
						TableController.enPassant.Add(move.canEnPassantRight);
					}



				}

			}



		}




		public void Delete()
		{
			this.node.QueueFree();

			DeleteVisualizers();

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
					return new AvailableMove(this,ij, false);
				}
				else if (t.num == 2)
				{
					return new AvailableMove(this,ij, true, t.p);
				}

			}
			else
			{

				pos_vector = ij;



				if (t.num == 0 && !kingIsInCheckSame())
				{
					pos_vector = old;
					return new AvailableMove(this, ij, false);
				}
				else if (t.num == 2 && !kingIsInCheckSame(t.p))
				{
					pos_vector = old;
					return new AvailableMove(this, ij, true, t.p);
				}else if(t.num == 1)
				{
					pos_vector = old;
					return new AvailableMove(null,new Vector3(999,999,999), false);
				}else if(t.num == 2)
				{
					pos_vector = old;
					return new AvailableMove(null, new Vector3(999, 999, 999), false);
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
				return new AvailableMove(this, ij, false);
			}
			else if (t.num == 2)
			{
				return new AvailableMove(this, ij, true, t.p);
			}

			return new AvailableMove(this,ij, false, true);
		}


		struct ans
		{
			public AvailableMove move;
			public bool stop;

			public ans(AvailableMove move, bool stop)
			{
				this.move = move;
				this.stop = stop;
			}
		}



		private ans diagnolStraightmovesHelper(Vector3 ij,target t,bool kingProtect,bool cover,bool one_square)
		{

			ans ans = new ans(null, false);

			if (ij.Z == 9 || ij.Z == 0) { ans.stop = true; return ans; }

			AvailableMove a = check(ij, kingProtect);

			if (cover) { a = checkWithCover(ij); }

			if (a != null)
			{
				ans.move = a;
				if (a.attack || a.cover || a.move.X == 999) { if (!(cover && a.target is King)) { ans.stop = true; } }
			}
			else { if (!kingProtect) { ans.stop = true; } }


			if (one_square) { ans.stop = true; };

			return ans;
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

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) {break;}


				ij.Z++;
				
			}

			ij.Z = pos_vector.Z - 1;
			ij.X = pos_vector.X;

			for (int i = (int)ij.X - 1; i >= 1; i--)
			{
				ij.X = i;

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }

				ij.Z--;

			}


			ij.Z = pos_vector.Z - 1;
			ij.X = pos_vector.X;

			for (int i = (int)ij.X + 1; i <= 8; i++)
			{
				ij.X = i;

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }

				ij.Z--;

			}

			ij.Z = pos_vector.Z + 1;
			ij.X = pos_vector.X;

			for (int i = (int)ij.X - 1; i >= 1; i--)
			{
				ij.X = i;

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }

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

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }

			}

			ij.Z = pos_vector.Z - 1;


			for (int i = (int)ij.Z; i > 0; i--)
			{

				ij.Z = i;

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }
			}

			ij.Z = pos_vector.Z;
			ij.X = pos_vector.X + 1;

			for (int i = (int)ij.X; i < 9; i++)
			{

				ij.X = i;

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }
			}

			ij.X = pos_vector.X - 1;


			for (int i = (int)ij.X; i > 0; i--)
			{

				ij.X = i;

				ans ans = diagnolStraightmovesHelper(ij, t, kingProtect, cover, one_square);

				if (ans.move != null) { results.Add(ans.move); }

				if (ans.stop) { break; }
			}




			return results;
		}



		protected bool kingIsInCheckEnemy()
		{

			foreach (Piece piec in TableController.table)
			{
				if (this.team == piec.team)
				{
					foreach(AvailableMove move in piec.CheckValidMoves(false))
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



		//improve this functions
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
					foreach (AvailableMove move in piec.CheckValidMoves(false))
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
				if (this.team != piec.team && !(piec is King))
				{
					foreach (AvailableMove move in piec.CheckValidMoves(false))
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

		protected List<AvailableMove> kingMoves(bool calculateCastle = false)
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

					if (results[i].move.X == na.move.X && results[i].move.Z == na.move.Z)
					{
						toRemove.Add(results[i]);
					}


				}
			}

			pos_vector = old;

			if (calculateCastle && firstMove && !kingCheckWithoutKing())
			{
				foreach(Piece rook in TableController.table)
				{
					if(team == 1)
					{

						//KINGSIDE WHITE
						if(rook.team == 1 && rook.firstMove && rook.position is "H1")
						{

							foreach(AvailableMove m in rook.straightMoves(false,true))
							{
							
								if (m.move.X == pos_vector.X && pos_vector.Z == m.move.Z && kingCastlingKingSideWhite(notavailable))
								{

									AvailableMove whiteKingSideCastle = new AvailableMove(this,new Vector3(1,0,7),rook,new Vector3(1,0,6));

									whiteKingSideCastle.kingSideCastling = true;

									results.Add(whiteKingSideCastle);


								}

							}
						}

						//QUEENSIDE WHITE

						if (rook.team == 1 && rook.firstMove && rook.position is "A1")
						{
							foreach (AvailableMove m in rook.straightMoves(false, true))
							{

								if (m.move.X == pos_vector.X && pos_vector.Z == m.move.Z)
								{
									GD.Print("King can castle!" + (team == 1 ? " White " : " Black ") + " QUEENSIDE WHITE AVAILABLE");
								}
								
							}
						}

					}

					if(team == -1)
					{

						//KINGSIDE BLACK

						if (rook.team == -1 && rook.firstMove && rook.position is "H8")
						{
							GD.Print("King can castle!" + (team == 1 ? " White " : " Black ") + " KINGSIDE BLACK AVAILABLE");
						}


						//QUEENSIDE BLACK

						if (rook.team == -1 && rook.firstMove && rook.position is "A8")
						{
							GD.Print("King can castle!" + (team == 1 ? " White " : " Black ") + " QUEENSIDE BLACK AVAILABLE");
						}

					}

				}

			}






			foreach (AvailableMove move in toRemove)
			{
				results.Remove(move);
			}

			return results;
		}


		private bool kingCastlingKingSideWhite(List<AvailableMove> moves)
		{
			foreach(AvailableMove move in moves)
			{

				if(move.move.X == 1 && move.move.Z == 6 || move.move.X == 1 && move.move.Z == 7)
				{
					return false;
				}

			}

			return true;
		}


		private AvailableMove kingCastlingQueenSideWhite(List<AvailableMove> moves)
		{
			AvailableMove ans = null;




			return ans;
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
					return new AvailableMove(this,vec, false);

				}
				else if (second && move.num == 2 || cover && second)
				{

					return new AvailableMove(this, vec, true, move.p);
				}

			}
			else
			{


				if (first && move.num == 0 && !kingIsInCheckSame())
				{

					pos_vector = old;
					return new AvailableMove(this, vec, false);

				}
				else if (second && move.num == 2 && !kingIsInCheckSame(move.p))
				{
		
					pos_vector = old;
					return new AvailableMove(this, vec, true, move.p);
				}

			}


			if (kingProtect) { pos_vector = old; };

			return null;

		}


		protected List<AvailableMove> pawnMoves(bool cover,bool kingProtection,bool special)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			Vector3 old = this.pos_vector;


			bool checkKing = false;

			Vector3 vec = new Vector3(pos_vector.X + (1 * team), pos_vector.Y, pos_vector.Z);

			target move = TableController.find(vec, this);




			if (firstMove)
			{


				Vector3 vec2 = new Vector3(pos_vector.X + (2 * team), pos_vector.Y, pos_vector.Z);

				target move2 = TableController.find(vec2, this);


				if (kingProtection)
				{
					pos_vector = vec2;
					checkKing = kingIsInCheckSame();
				}

			
				if (!kingProtection)
				{
					if (vec2.X < 9 && move2.num == 0 && move.num == 0 && !cover)
					{ 
						results.Add(new AvailableMove(this, vec2, false));
					}
				}
				else
				{

					if (vec2.X < 9 && move2.num == 0 && move.num == 0 && !checkKing)
					{

						AvailableMove m = new AvailableMove(this, vec2, false);


						checkEnPassant(vec2, m);


						results.Add(m);

					}
					pos_vector = old;

				}




			}


			if (enPassantRight != null || enPassantLeft != null)
			{

				if (enPassantLeft != null)
				{
					Vector3 vec2 = new Vector3(pos_vector.X + (1 * team), pos_vector.Y, pos_vector.Z + 1);

					target move3 = TableController.find(vec2, this);

					if (kingProtection)
					{
						pos_vector = vec2;
						checkKing = kingIsInCheckSame();
					}

					if (!kingProtection)
					{

						if (vec.X < 9 && vec.Z > 0 || vec.X < 9 && vec.Z < 9 && cover)
						{
							results.Add(new AvailableMove(this, vec2, true, enPassantLeft));

						}

					}
					else
					{

						pos_vector = vec;
						checkKing = kingIsInCheckSame(move3.p);

						if (vec.X < 9 && vec.Z > 0 && !checkKing)
						{
							results.Add(new AvailableMove(this, vec2, true, enPassantLeft));

						}

						pos_vector = old;

					}





				}

				if (enPassantRight != null)
				{
					Vector3 vec2 = new Vector3(pos_vector.X + (1 * team), pos_vector.Y, pos_vector.Z - 1);
					target move3 = TableController.find(vec2, this);


					if (kingProtection)
					{
						pos_vector = vec2;
						checkKing = kingIsInCheckSame();
					}

					if (!kingProtection)
					{

						if (vec.X < 9 && vec.Z > 0 || vec.X < 9 && vec.Z < 9 && cover)
						{
							results.Add(new AvailableMove(this, vec2, true, enPassantRight));

						}

					}
					else
					{

						pos_vector = vec;
						checkKing = kingIsInCheckSame(move3.p);

						if (vec.X < 9 && vec.Z > 0 && !checkKing)
						{
							results.Add(new AvailableMove(this, vec2, true, enPassantRight));

						}

						pos_vector = old;

					}





				}


			}


			if (kingProtection)
			{
				pos_vector = vec;
				checkKing = kingIsInCheckSame();
			}





			if (!kingProtection)
			{
				if (vec.X < 9 && move.num == 0 && !cover)
				{
					results.Add(new AvailableMove(this, vec, false));
				}
			}
			else
			{

				if (vec.X < 9 && move.num == 0 && !checkKing)
				{
					results.Add(new AvailableMove(this, vec, false));
				}
				pos_vector = old;

			}


			vec = new Vector3(pos_vector.X + (1 * team), pos_vector.Y, pos_vector.Z + 1);
			move = TableController.find(vec, this);



			if (!kingProtection)
			{
				if (vec.X < 9 && vec.Z < 9 && move.num == 2 || vec.X < 9 && vec.Z < 9 && cover)
				{
					results.Add(new AvailableMove(this, vec, true, move.p));

				}
			}
			else
			{

				pos_vector = vec;
				checkKing = kingIsInCheckSame(move.p);

				if (vec.X < 9 && vec.Z < 9 && move.num == 2 && !checkKing)
				{
					results.Add(new AvailableMove(this, vec, true, move.p));

				}
				pos_vector = old;

			}

			vec = new Vector3(pos_vector.X + (1 * team), pos_vector.Y, pos_vector.Z - 1);
			move = TableController.find(vec, this);


			if (!kingProtection)
			{

				if (vec.X < 9 && vec.Z > 0 && move.num == 2 || vec.X < 9 && vec.Z < 9 && cover)
				{
					results.Add(new AvailableMove(this, vec, true, move.p));

				}

			}
			else
			{

				pos_vector = vec;
				checkKing = kingIsInCheckSame(move.p);

				if (vec.X < 9 && vec.Z > 0 && move.num == 2 && !checkKing)
				{
					results.Add(new AvailableMove(this,	vec, true, move.p));

				}

				pos_vector = old;

			}



			return results;
		}





		private void checkEnPassant(Vector3 vector,  AvailableMove availablemove)
		{


			vector.Z--;

			Pawn p = findPawn(vector, team);

			if(p!=null) {



				availablemove.enPassant = true;

				availablemove.canEnPassantLeft = p;

			}


			vector.Z += 2;

			p = findPawn(vector, team);


			if (p != null)
			{


				availablemove.enPassant = true;

				availablemove.canEnPassantRight = p;

			}


		}


	}
}
