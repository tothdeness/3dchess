using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using test.Pieces.Resources;



namespace test.Pieces
{

    public class Pawn : Piece
	{


		public Pawn(string position, int team, GameController game) : base(position, team, game)
		{
				pos_vector = new Vector3(pos_vector.X, -0.25f , pos_vector.Z);
				setDirections();
		}

		private void setDirections()
		{
			if(team == 1)
			{
				directions = new Dictionary<string, Vector3>
				{
					{ "(1, 0, 1)", new Vector3(1, 0, 1) },
					{ "(1, 0, -1)", new Vector3(1, 0, -1) },
					{ "(1, 0, 0)", new Vector3(1, 0, 0) },
				};
			}
			else
			{
				directions = new Dictionary<string, Vector3>
				{
					{ "(-1, 0, -1)", new Vector3(-1, 0, -1) },
					{ "(-1, 0, 1)", new Vector3(-1, 0, 1) },
					{ "(-1, 0, 0)", new Vector3(1, 0, 0) },
				};
			}
		}

		public override void addVisuals()
		{

			Mesh = "res://TSCN/pawn.tscn";

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			setColor();
		}



		public void promotePawn(Board board,AvailableMove move, bool visual)
		{
			if (pos_vector.X == 8 || pos_vector.X == 1)
			{

				var addQueen = new Queen(position, team, gameController);

				board.table[pos_vector.ToString()] = addQueen;

				if (visual)
				{
					Delete();
					addQueen.addVisuals();
				}

				move.promoted = true;
			}

		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			if (board.kingIsInDoubleCheck) { return ans; }

			bool locked = validDirections.Count > 0;

			string direc = "";

			if (locked) { direc = validDirections[0].ToString(); }

			if (locked && !directions.ContainsKey(direc)) { return ans; }

			ans.AddRange(pawnMoves(false, false, board));

			if (locked)
			{
				if (firstMove && (direc == "(1, 0, 0)" || direc == "(-1, 0, 0)") )
				{
					ans.RemoveAll(item => item.move.Z != pos_vector.Z);

				}
				else
				{
					ans.RemoveAll(item => item.move != validDirections[0] + pos_vector);
				}

			}

			if (board.kingIsInCheck) { removeMoves(ans, board); }

			return ans;

		}

		public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();
			ans.AddRange(pawnMoves(true, false, board));
			return ans;
		}



	}
}
