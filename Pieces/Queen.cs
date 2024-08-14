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
using Chess;

namespace test.Pieces
{
	public class Queen : Piece
	{


		public Queen(string position, int team, GameController game) : base(position,team, game)
		{
			pos_vector = new Vector3(pos_vector.X, -0.25f, pos_vector.Z);
			setDirections();
		}

		public override void addVisuals()
		{
			Mesh = "res://TSCN/mesh.tscn";

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();

			node = inst;
			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.CallDeferred("add_child", inst);

			setColor();
		}


		
		private void setDirections()
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
				ans.AddRange(slidingMoves(false, false, board, validDirections));
			}
			else
			{
				ans.AddRange(diagnolMoves(false, false, board));
				ans.AddRange(straightMoves(false, false, board));
			}

			if (board.kingIsInCheck) { removeMoves(ans, board); }

			return ans;
		}


		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(diagnolMoves(false, true, board));
			ans.AddRange(straightMoves(false, true, board));
			return ans;
		}




	}
}
