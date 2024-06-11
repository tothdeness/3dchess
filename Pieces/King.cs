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
	public class King : Piece
	{
		public King(string position, int team,GameController game) : base(position, team, game)
		{

			Mesh = "res://TSCN/king.tscn";

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();

			pos_vector = new Vector3(pos_vector.X, -0.25f, pos_vector.Z);

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			Random rand = new Random();
			this.ID = rand.Next(000000000, 999999999);

			setColor();

		}


		public King(string position, int team, Board board, bool firstMove, int ID, GameController game) : base(position, team, board, game)
		{
			this.firstMove = firstMove;
			this.ID = ID;
		}


		public override List<AvailableMove> CheckValidMoves(bool s)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(kingMoves());

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesWithCover()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(diagnolMoves(true,true));
			ans.AddRange(straightMoves(true, true));


			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithKingProtection()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(kingMoves(true));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(kingMoves(board));


			return ans;
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(kingMoves( board));

			return ans;
		}


	}
}
