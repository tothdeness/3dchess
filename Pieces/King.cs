using Chess;
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
			pos_vector = new Vector3(pos_vector.X, -0.25f, pos_vector.Z);
			castleAvailable = true;
		}

		public override void addVisuals()
		{

			Mesh = "res://TSCN/king.tscn";

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			setColor();
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(kingMoves( board));


			if(!board.kingIsInCheck && firstMove)
			{
				AvailableMove k = KingSideCastle(board);
				AvailableMove q = QueenSideCastle(board);

				if(k != null) { ans.Add(k); }
				if(q != null) { ans.Add(q); }
			}


			return ans;
		}

		private AvailableMove KingSideCastle(Board board)
		{
			AvailableMove ans = null;
		
			if(board.current == 1)
			{

				if (!board.table.TryGetValue("(1, -0,25, 8)", out Piece whiteRook) || !whiteRook.firstMove) { return ans; }

				List<string> white = new List<string> { "F1", "G1" };

				if (CanCastle(board, white)) { GD.Print("White can king side castle!"); }

			}
			else
			{
				if (!board.table.TryGetValue("(8, -0,25, 8)", out Piece blackRook) || !blackRook.firstMove) { return ans; }

				List<string> black = new List<string> { "G8", "F8" };

				if (CanCastle(board, black)) { GD.Print("Black can king side castle!"); }
			}


			return ans;
		}

		private AvailableMove QueenSideCastle(Board board)
		{
			AvailableMove ans = null;


			return ans;
		}

		private bool CanCastle(Board board,List<string> moves)
		{
			foreach(string move in moves)
			{
				if(board.table.ContainsKey(TableController.convert(move).ToString()) || board.attackedSquares.Contains(move))
				{
					return false;
				}
			}

			return true;
		}




		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(straightMoves(true, true, board));
			ans.AddRange(diagnolMoves(true, true, board));
			return ans;
		}

	}
}
