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

			pos_vector = new Vector3(pos_vector.X, -0.25f, pos_vector.Z);
			setDirecitons();
		}


		public override void addVisuals()
		{

			Mesh = "res://TSCN/rook.tscn";

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();


			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			setColor();
		}

		private void setDirecitons()
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
				ans.AddRange(slidingMoves(false, false, board, validDirections));
			}
			else
			{
				ans.AddRange(straightMoves(false, false, board));
			}

			if (board.kingIsInCheck) { removeMoves(ans, board); }

			return ans;

		}

		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(straightMoves(false, true, board));
			return ans;
		}




	}
}
