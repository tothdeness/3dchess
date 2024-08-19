using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using test.Pieces.Resources;

namespace test.Pieces
{
    public class Rook : Piece
	{

		public Rook(string position, int team, GameController game) : base(position, team, game)
		{
			mesh = "res://TSCN/rook.tscn";
			posVector = new Vector3(posVector.X, -0.25f, posVector.Z);
			SetDirecitons();
		}


		private void SetDirecitons()
		{

			directions = new Dictionary<string, Vector3>
			{
				{ "(1, 0, 0)", new Vector3(1, 0, 0) },
				{ "(-1, 0, 0)", new Vector3(-1, 0, 0) },
				{ "(0, 0, 1)", new Vector3(0, 0, 1) },
				{ "(0, 0, -1)", new Vector3(0, 0, -1) }
			};


		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			if (board.kingIsInDoubleCheck) { return ans; }

			if (this.validDirections.Count > 0)
			{
				if (!directions.ContainsKey(validDirections[0].ToString())) { return ans; }
				ans.AddRange(SlidingMoves(false, false, board, validDirections));
			}
			else
			{
				ans.AddRange(StraightMoves(false, false, board));
			}

			if (board.kingIsInCheck) { RemoveMoves(ans, board); }

			return ans;

		}

		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(StraightMoves(false, true, board));
			return ans;
		}




	}
}
