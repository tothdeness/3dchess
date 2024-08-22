using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using test.Pieces.Resources;

namespace test.Pieces
{
    public class King : Piece
	{
		public bool castleAvailable;


		private List<Vector3> whiteKingSide = new List<Vector3>
			{
				new Vector3(1, 0, 6),
				new Vector3(1, 0, 7)
			};

		private List<Vector3>  blackKingSide = new List<Vector3>
			{
				new Vector3(8, 0, 6), 
				new Vector3(8, 0, 7)  
			};

		private List<Vector3> whiteQueenSide = new List<Vector3>
			{
				new Vector3(1, 0, 2), 
				new Vector3(1, 0, 3), 
				new Vector3(1, 0, 4)  
			};

		private List<Vector3> blackQueenSide = new List<Vector3>
			{
				new Vector3(8, 0, 2),
				new Vector3(8, 0, 3),
				new Vector3(8, 0, 4)
			};



		public King(string position, int team,GameController game) : base(position, team, game)
		{
			mesh = "res://TSCN/king.tscn";
			posVector = new Vector3(posVector.X, -0.25f, posVector.Z);
			castleAvailable = true;
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(KingMoves( board));


			if(!board.kingIsInCheck && firstMove)
			{
				AvailableMove k = KingSideCastle(board);
				AvailableMove q = QueenSideCastle(board);

				if(k != null) {  ans.Add(k);  }
				if(q != null) {  ans.Add(q);  }

			}


			return ans;
		}

		private AvailableMove KingSideCastle(Board board)
		{
			AvailableMove ans = null;

			if (board.current == 1)
			{

				if (!board.table.TryGetValue(new Vector3(1,0,8), out Piece rook) || !rook.firstMove) { return ans; }



				if (CanCastle(board, whiteKingSide)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z + 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z + 1), rook.posVector); }

			}
			else
			{
				if (!board.table.TryGetValue(new Vector3(8,0,8), out Piece rook) || !rook.firstMove) { return ans; }


				if (CanCastle(board, blackKingSide)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z + 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z + 1), rook.posVector); }
			}

			return ans;
		}

		private AvailableMove QueenSideCastle(Board board)
		{
			AvailableMove ans = null;

			if (board.current == 1)
			{

				if (!board.table.TryGetValue(new Vector3(1,0,1), out Piece rook) || !rook.firstMove) { return ans; }


				if (CanCastle(board, whiteQueenSide)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z - 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z - 1), rook.posVector); }

			}
			else
			{
				if (!board.table.TryGetValue(new Vector3(8,0,1), out Piece rook) || !rook.firstMove) { return ans; }


				if (CanCastle(board, blackQueenSide)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z - 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z - 1), rook.posVector); }
			}

			return ans;
		}

		private bool CanCastle(Board board,List<Vector3> moves)
		{
			foreach(var move in moves)
			{
				if(board.table.ContainsKey(move) || board.attackedSquares.Contains(move))
				{
					return false;
				}
			}

			return true;
		}


		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(StraightMoves(true, true, board));
			ans.AddRange(DiagnolMoves(true, true, board));
			return ans;
		}

	}
}
