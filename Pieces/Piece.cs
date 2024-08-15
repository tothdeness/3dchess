using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using test.Controllers;
using test.Pieces.Resources;
using static Godot.HttpRequest;
using static test.Controllers.TableController;

namespace test.Pieces
{
    public  class Piece
	{
		//VERTICAL POS 3 (Y)

		public string Mesh;
		public Node node { get; set; }
	  
		public string position;

		protected float y = -0.25f;

		public bool firstMove = true;

		public GameController gameController;

		public Dictionary<string,Vector3> directions = new Dictionary<string, Vector3>();

		public List<Vector3> validDirections = new List<Vector3>();

		public Vector3 pos_vector { get; set; }
	
		//-1 black, 1 white
		public int team { get; set; }

		public virtual List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board) { throw new NotImplementedException(); }

		public virtual List<AvailableMove> CheckValidMovesVirt(Board board) { throw new NotImplementedException(); }

		public virtual void addVisuals() { throw new NotImplementedException(); }

		public void addToGame()
		{
			gameController.board.table.Add(pos_vector.ToString(), this);
		}

		public void ShowValidMoves(Board board)
		{
			TableController.showVisualizers(TableController.calculateVisualizers(CheckValidMovesVirt(board)), this);
		}

		public void DeleteVisualizers() {
			TableController.removeVisualizers();
		}


		public void removeMoves(List<AvailableMove> moves,Board board)
		{
			moves.RemoveAll(item => !board.block.Contains(item.move.ToString()));
		}


		public void SetMesh()
		{

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			setColor();
		}


		public void Move(Vector3 vector,AvailableMove move) {

			vector.Y = pos_vector.Y;

			this.pos_vector = TableController.reversePosition(vector);

			this.position = TableController.convertReverse(pos_vector);

			if (move.target != null && move.attack)
			{
				gameController.board.updateWithAttack(move);
				move.target.Delete(); 
			}
			else
			{
				gameController.board.updatekey(move);
			}

			if (this is Pawn)
			{
				Pawn pawn = (Pawn)this;
				pawn.promotePawn(gameController.board, move, true);
			}

			if (this is King && (move.kingSideCastling || move.queenSideCastling))

			{ move.target.Move(TableController.calculatePosition(move.rookNewPos), new AvailableMove(move.target, move.rookNewPos, false, pos_vector)); }


			if (firstMove) firstMove = false;

			TableController.removeVisualizers();

			this.node.CallDeferred("_bot_move2", vector);

			gameController.NextMove(team);

		}


		public void VirtualMove(Vector3 vector, AvailableMove move, Board board)
		{

			this.pos_vector = TableController.reversePosition(vector);

			this.position = TableController.convertReverse(pos_vector);

			if (move.attack && move.target != null)
			{
				board.updateWithAttack(move);
			}
			else
			{
				board.updatekey(move);
			}

			if (this is Pawn)
			{
				Pawn pawn = (Pawn)this;
				pawn.promotePawn(gameController.board, move, false);
			}

			if (this is King && (move.kingSideCastling || move.queenSideCastling))

			{ move.target.VirtualMove(TableController.calculatePosition(move.rookNewPos), new AvailableMove(move.target, move.rookNewPos, false, pos_vector), board); }

			if (firstMove) firstMove = false;

		}




		public void Delete()
		{
			this.node.QueueFree();
			DeleteVisualizers();

		}


		protected Piece(string position, int team, GameController game)
		{
			this.position = position;
			this.team = team;
			this.gameController = game;


			Vector3 p = TableController.convert(position);
			p.Y = y;
			pos_vector = p;

		}

		protected void setColor()
		{

			Dummy d = (Dummy) node;

			if(team == 1) {
				d.CallDeferred("setColorWhite");
			}
			else
			{
				d.CallDeferred("setColorBlack");
			}
			

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




		protected List<AvailableMove> pawnMoves(bool cover, bool kingProtection, Board board)
		{
			List<AvailableMove> results = new List<AvailableMove>();


			if (pos_vector.X == 1) { return results; };


			Vector3 vec = new Vector3(pos_vector.X + (1 * team), pos_vector.Y, pos_vector.Z);

			Board.target move = board.find(vec, this);

			if (!Board.checkboundries(vec) && move.num == 0 && !cover)
			{
				AvailableMove m = new AvailableMove(this, vec, false,pos_vector,firstMove);
				results.Add(m);
			}

			if (firstMove && !cover)
			{
				Vector3 vec2 = new Vector3(pos_vector.X + (2 * team), pos_vector.Y, pos_vector.Z);

				Board.target move2 = board.find(vec2, this);

				if (move2.num == 0 && move.num == 0)
				{
					AvailableMove m = new AvailableMove(this, vec2, false, pos_vector, firstMove);
					results.Add(m);

				}
			}

			foreach(KeyValuePair<string, Vector3> entry in directions)
				{	
					if(entry.Value.Z != 0)
						{
							vec = new Vector3(pos_vector.X + entry.Value.X, pos_vector.Y, pos_vector.Z + entry.Value.Z);
							move = board.find(vec,this);

							if (move.num == 2 || cover)
								{
									AvailableMove m = new AvailableMove(this, vec, true, move.p,pos_vector,firstMove);
									results.Add(m);
								}

						}
				
				}

				return results;
			}




		protected List<AvailableMove> horseMoves(bool cover, bool kingProtect, Board board)
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

						if (Board.checkboundries(newPosition)) { continue; }

						AvailableMove a = IsValidPosition(newPosition, cover, kingProtect, board);

						if (a != null) { results.Add(a); }

					}
				}
			}

			return results;
		}

		//HORSE
		private AvailableMove IsValidPosition(Vector3 position, bool cover, bool kingProtect, Board board)
		{
	
			Board.target move = board.find(position, this);


				if (move.num == 2 || cover)
				{
					return new AvailableMove(this, position, true, move.p, pos_vector);
					
				}
				else if (move.num == 0 || cover)
				{

					return new AvailableMove(this, position, false, pos_vector);
				}

			return null;

		}


		private AvailableMove checkWithCover(Vector3 ij,Board board)
		{

			var t = board.find(ij, this);

			if (t.num == 0)
			{
				return new AvailableMove(this, ij, false, pos_vector, firstMove);
			}
			else if (t.num == 2)
			{
				return new AvailableMove(this, ij, true, t.p, pos_vector, firstMove);
			}

			return new AvailableMove(this, ij, false, true, pos_vector, firstMove);
		}

		private AvailableMove check(Vector3 ij,Board board, bool kingProtect = false)
		{

			var t = board.find(ij, this);

				if (t.num == 0)
				{
					return new AvailableMove(this, ij, false, pos_vector, firstMove);
				}
				else if (t.num == 2)
				{
					return new AvailableMove(this, ij, true, t.p, pos_vector, firstMove);
				}

			return null;
		}


	private ans diagnolStraightmovesHelper(Vector3 ij, bool kingProtect, bool cover, bool one_square, Board board)
	{
	ans ans = new ans(null, false);

	if (ij.Z < 1 || ij.Z > 8 || ij.X < 1 || ij.X > 8) 
	{
		ans.stop = true;
		return ans;
	}

	AvailableMove a = null;

	if (cover) 
	{ 
		a = checkWithCover(ij, board);
	}
	else
	{
		a = check(ij, board, kingProtect);
	}


	if (a != null)
	{
		ans.move = a;
		if (a.attack || a.cover) 
		{
			if (!(cover && a.target is King)) 
			{ 
				ans.stop = true; 
			}
		}
	}
	else 
	{ 
			ans.stop = true;  
	}

	if (one_square) 
	{ 
		ans.stop = true; 
	}

	return ans;
	}

	private List<AvailableMove> generateMoves(List<Vector3> directions, bool one_square, bool cover, Board board, bool kingProtect = false)
{
	List<AvailableMove> results = new List<AvailableMove>();

	foreach (Vector3 direction in directions)
	{
		Vector3 ij = pos_vector;
		while (true)
		{
			ij.X += direction.X;
			ij.Z += direction.Z;

			ans ans = diagnolStraightmovesHelper(ij, kingProtect, cover, one_square, board);

			if (ans.move != null) 
			{ 
				results.Add(ans.move); 
			}

			if (ans.stop) 
			{ 
				break; 
			}
		}
	}

	return results;
}



	protected List<AvailableMove> diagnolMoves(bool one_square, bool cover, Board board, bool kingProtect = false)
	{
	
	List<Vector3> diagonalDirections = new List<Vector3>
	{
		new Vector3(1, 0, 1),
		new Vector3(-1, 0, -1),
		new Vector3(1, 0, -1),
		new Vector3(-1, 0, 1)
	};


	return generateMoves(diagonalDirections, one_square, cover, board, kingProtect);

	}




	protected List<AvailableMove> straightMoves(bool one_square, bool cover, Board board, bool kingProtect = false)
	{

	List<Vector3> straightDirections = new List<Vector3>
	{
		new Vector3(1, 0, 0),
		new Vector3(-1, 0, 0),
		new Vector3(0, 0, 1),
		new Vector3(0, 0, -1)
	};

	return generateMoves(straightDirections, one_square, cover, board, kingProtect);
	}


	protected List<AvailableMove> slidingMoves(bool one_square, bool cover, Board board,List<Vector3> directions, bool kingProtect = false)
	{
		return generateMoves(directions, one_square, cover, board, kingProtect);
	}



		protected List<AvailableMove> kingMoves(Board board)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			results.AddRange(straightMoves(true, false,board));
			results.AddRange(diagnolMoves(true, false,board));

			List<AvailableMove> ans = new List<AvailableMove>();

			foreach (var move in results)
			{
				if (!board.attackedSquares.Contains(TableController.convertReverse(move.move)))
				{
					ans.Add(move);
				}
			}

			return ans;
		}






	}
}
