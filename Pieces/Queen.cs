using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using test.Controllers;
using static Godot.HttpRequest;
using test.Pieces.Resources;

namespace test.Pieces
{
    public class Queen : Piece
	{


		public Queen(string position, int team, GameController game) : base(position,team, game)
		{
			mesh = "res://TSCN/mesh.tscn";
			posVector = new Vector3(posVector.X, -0.25f, posVector.Z);
			SetDirections();
		}


		
		private void SetDirections()
		{
			directions = new Dictionary<string, Vector3>
			{
				{ "(1, 0, 0)", new Vector3(1, 0, 0) },
				{ "(-1, 0, 0)", new Vector3(-1, 0, 0) },
				{ "(0, 0, 1)", new Vector3(0, 0, 1) },
				{ "(0, 0, -1)", new Vector3(0, 0, -1) },
				{ "(1, 0, 1)", new Vector3(1, 0, 1) },
				{ "(-1, 0, -1)", new Vector3(-1, 0, -1) },
				{ "(1, 0, -1)", new Vector3(1, 0, -1) },
				{ "(-1, 0, 1)", new Vector3(-1, 0, 1) }
			};
		}



		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			if (board.kingIsInDoubleCheck) { return ans; }

			if (this.validDirections.Count > 0)
			{
				ans.AddRange(SlidingMoves(false, false, board, validDirections));
			}
			else
			{
				ans.AddRange(DiagnolMoves(false, false, board));
				ans.AddRange(StraightMoves(false, false, board));
			}

			if (board.kingIsInCheck) { RemoveMoves(ans, board); }

			return ans;
		}


		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(DiagnolMoves(false, true, board));
			ans.AddRange(StraightMoves(false, true, board));
			return ans;
		}




	}
}
