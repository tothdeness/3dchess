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

		public string mesh;
		public Node node { get; set; }
	  
		public string position;

		protected float y = -0.25f;

		public bool firstMove = true;

		public GameController gameController;

		public Dictionary<string,Vector3> directions = new Dictionary<string, Vector3>();

		public List<Vector3> validDirections = new List<Vector3>();

		public Vector3 posVector { get; set; }
	
		//-1 black, 1 white
		public int team { get; set; }

		public virtual List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board) { throw new NotImplementedException(); }

		public virtual List<AvailableMove> CheckValidMovesVirt(Board board) { throw new NotImplementedException(); }

		public void AddToGame()
		{
			gameController.board.table.Add(posVector.ToString(), this);
		}

		public void ShowValidMoves(Board board)
		{
			TableController.ShowVisualizers(TableController.CalculateVisualizers(CheckValidMovesVirt(board)), this);
		}

		public void DeleteVisualizers() {
			TableController.RemoveVisualizers();
		}


		public void RemoveMoves(List<AvailableMove> moves,Board board)
		{
			moves.RemoveAll(item => !board.block.Contains(item.move.ToString()));	
		}

		public void AddVisuals()
		{
			PackedScene scene = GD.Load<PackedScene>(mesh);
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.CalculatePosition(posVector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			SetColor();

		}






		public void SetMesh()
		{

			PackedScene scene = GD.Load<PackedScene>(mesh);
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.CalculatePosition(posVector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			SetColor();
		}


		public void MovePieceWithVisualUpdate(Vector3 vector,AvailableMove move) {

			vector.Y = posVector.Y;

			this.posVector = TableController.ReversePosition(vector);

			this.position = TableController.ConvertReverse(posVector);

			if (move.target != null && move.attack)
			{
				gameController.board.UpdateWithAttack(move);
				move.target.Delete(); 
			}
			else
			{
				gameController.board.Updatekey(move);
			}

			if (this is Pawn)
			{
				Pawn pawn = (Pawn)this;
				pawn.PromotePawn(gameController.board, move, true);
			}


			if (firstMove) firstMove = false;

			TableController.RemoveVisualizers();

			this.node.CallDeferred("_bot_move2", vector);


			if (this is King && move.castle)
			{ 
				move.rook.MovePieceWithVisualUpdate(TableController.CalculatePosition(move.rookNewPos), new AvailableMove(move.rook, move.rookNewPos, false, move.rookOldPos, true));
				return;
			}


			gameController.NextMove(team);

		}


		public void VirtualMove(Vector3 vector, AvailableMove move, Board board)
		{

			this.posVector = TableController.ReversePosition(vector);

			this.position = TableController.ConvertReverse(posVector);

			if (move.attack && move.target != null)
			{
				board.UpdateWithAttack(move);
			}
			else
			{
				board.Updatekey(move);
			}

			if (this is Pawn)
			{
				Pawn pawn = (Pawn)this;
				pawn.PromotePawn(gameController.board, move, false);
			}

			if (firstMove) firstMove = false;

			if (this is King && move.castle)
			{
				move.rook.VirtualMove(TableController.CalculatePosition(move.rookNewPos), new AvailableMove(move.rook, move.rookNewPos, false, move.rookOldPos, true),board);
			}

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


			Vector3 p = TableController.Convert(position);
			p.Y = y;
			posVector = p;

		}

		protected void SetColor()
		{

			Dummy d = (Dummy) node;

			if(team == 1) {
				d.CallDeferred("SetColorWhite");
			}
			else
			{
				d.CallDeferred("SetColorBlack");
			}
			

		}


		
		struct Move
		{
			public AvailableMove move;
			public bool stop;

			public Move(AvailableMove move, bool stop)
			{
				this.move = move;
				this.stop = stop;
			}
		}




		protected List<AvailableMove> pawnMoves(bool cover, bool kingProtection, Board board)
		{
			List<AvailableMove> results = new List<AvailableMove>();


			if (posVector.X == 1) { return results; };


			Vector3 vec = new Vector3(posVector.X + (1 * team), posVector.Y, posVector.Z);

			Board.Target move = board.Find(vec, this);

			if (!Board.CheckBoundaries(vec) && move.num == 0 && !cover)
			{
				AvailableMove m = new AvailableMove(this, vec, false,posVector,firstMove);
				results.Add(m);
			}

			if (firstMove && !cover)
			{
				Vector3 vec2 = new Vector3(posVector.X + (2 * team), posVector.Y, posVector.Z);

				Board.Target move2 = board.Find(vec2, this);

				if (move2.num == 0 && move.num == 0)
				{
					AvailableMove m = new AvailableMove(this, vec2, false, posVector, firstMove);
					results.Add(m);

				}
			}

			foreach(KeyValuePair<string, Vector3> entry in directions)
				{	
					if(entry.Value.Z != 0)
						{
							vec = new Vector3(posVector.X + entry.Value.X, posVector.Y, posVector.Z + entry.Value.Z);
							move = board.Find(vec,this);

							if (move.num == 2 || cover)
								{
									AvailableMove m = new AvailableMove(this, vec, true, move.p,posVector,firstMove);
									results.Add(m);
								}

						}
				
				}

				return results;
			}




		protected List<AvailableMove> HorseMoves(bool cover, bool kingProtect, Board board)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			int[] offsets = { 1, 2, -1, -2 };

			foreach (int x in offsets)
			{
				foreach (int z in offsets)
				{
					if (Math.Abs(x) != Math.Abs(z)) // Ensure L-shaped moves
					{

						Vector3 newPosition = new Vector3(posVector.X + x, posVector.Y, posVector.Z + z);

						if (Board.CheckBoundaries(newPosition)) { continue; }

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
	
			Board.Target move = board.Find(position, this);


				if (move.num == 2 || cover)
				{
					return new AvailableMove(this, position, true, move.p, posVector);
					
				}
				else if (move.num == 0 || cover)
				{

					return new AvailableMove(this, position, false, posVector);
				}

			return null;

		}


		private AvailableMove CheckWithCover(Vector3 ij,Board board)
		{

			var t = board.Find(ij, this);

			if (t.num == 0)
			{
				return new AvailableMove(this, ij, false, posVector, firstMove);
			}
			else if (t.num == 2)
			{
				return new AvailableMove(this, ij, true, t.p, posVector, firstMove);
			}

			return new AvailableMove(this, ij, false, true, posVector, firstMove);
		}

		private AvailableMove Check(Vector3 ij,Board board, bool kingProtect = false)
		{

			var t = board.Find(ij, this);

				if (t.num == 0)
				{
					return new AvailableMove(this, ij, false, posVector, firstMove);
				}
				else if (t.num == 2)
				{
					return new AvailableMove(this, ij, true, t.p, posVector, firstMove);
				}

			return null;
		}


	private Move DiagnolStraightmovesHelper(Vector3 ij, bool kingProtect, bool cover, bool one_square, Board board)
	{
	Move ans = new Move(null, false);

	if (Board.CheckBoundaries(ij)) 
	{
		ans.stop = true;
		return ans;
	}

	AvailableMove a = null;

	if (cover) 
	{ 
		a = CheckWithCover(ij, board);
	}
	else
	{
		a = Check(ij, board, kingProtect);
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

	private List<AvailableMove> GenerateSlidingMoves(List<Vector3> directions, bool one_square, bool cover, Board board, bool kingProtect = false)
{
	List<AvailableMove> results = new List<AvailableMove>();

	foreach (Vector3 direction in directions)
	{
		Vector3 ij = posVector;
		while (true)
		{
			ij.X += direction.X;
			ij.Z += direction.Z;

			Move ans = DiagnolStraightmovesHelper(ij, kingProtect, cover, one_square, board);

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



	protected List<AvailableMove> DiagnolMoves(bool one_square, bool cover, Board board, bool kingProtect = false)
	{
	
	List<Vector3> diagonalDirections = new List<Vector3>
	{
		new Vector3(1, 0, 1),
		new Vector3(-1, 0, -1),
		new Vector3(1, 0, -1),
		new Vector3(-1, 0, 1)
	};


	return GenerateSlidingMoves(diagonalDirections, one_square, cover, board, kingProtect);

	}




	protected List<AvailableMove> StraightMoves(bool one_square, bool cover, Board board, bool kingProtect = false)
	{

	List<Vector3> straightDirections = new List<Vector3>
	{
		new Vector3(1, 0, 0),
		new Vector3(-1, 0, 0),
		new Vector3(0, 0, 1),
		new Vector3(0, 0, -1)
	};

	return GenerateSlidingMoves(straightDirections, one_square, cover, board, kingProtect);
	}


	protected List<AvailableMove> SlidingMoves(bool one_square, bool cover, Board board,List<Vector3> directions, bool kingProtect = false)
	{
		return GenerateSlidingMoves(directions, one_square, cover, board, kingProtect);
	}



		protected List<AvailableMove> KingMoves(Board board)
		{
			List<AvailableMove> results = new List<AvailableMove>();

			results.AddRange(StraightMoves(true, false,board));
			results.AddRange(DiagnolMoves(true, false,board));

			List<AvailableMove> ans = new List<AvailableMove>();

			foreach (var move in results)
			{
				if (!board.attackedSquares.Contains(TableController.ConvertReverse(move.move)))
				{
					ans.Add(move);
				}
			}

			return ans;
		}






	}
}
