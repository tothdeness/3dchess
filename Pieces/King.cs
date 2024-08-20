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

				if (!board.table.TryGetValue("(1, -0.25, 8)", out Piece rook) || !rook.firstMove) { return ans; }

				List<string> white = new List<string> { "F1", "G1" };


				if (CanCastle(board, white)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z + 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z + 1), rook.posVector); }

			}
			else
			{
				if (!board.table.TryGetValue("(8, -0.25, 8)", out Piece rook) || !rook.firstMove) { return ans; }

				List<string> black = new List<string> { "G8", "F8" };


				if (CanCastle(board, black)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z + 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z + 1), rook.posVector); }
			}

			return ans;
		}

		private AvailableMove QueenSideCastle(Board board)
		{
			AvailableMove ans = null;

			if (board.current == 1)
			{

				if (!board.table.TryGetValue("(1, -0.25, 1)", out Piece rook) || !rook.firstMove) { return ans; }

				List<string> white = new List<string> { "B1", "C1", "D1" };

				if (CanCastle(board, white)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z - 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z - 1), rook.posVector); }

			}
			else
			{
				if (!board.table.TryGetValue("(8, -0.25, 1)", out Piece rook) || !rook.firstMove) { return ans; }

				List<string> black = new List<string> { "B8", "C8", "D8" };

				if (CanCastle(board, black)) { ans = new AvailableMove(this, new Vector3(posVector.X, -0.25f, posVector.Z - 2), true, rook, true, posVector, new Vector3(posVector.X, -0.25f, posVector.Z - 1), rook.posVector); }
			}

			return ans;
		}

		private bool CanCastle(Board board,List<string> moves)
		{
			foreach(string move in moves)
			{
				if(board.table.ContainsKey(TableController.Convert(move).ToString()) || board.attackedSquares.Contains(move))
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
