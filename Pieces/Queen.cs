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

			Mesh = "res://TSCN/mesh.tscn";

			PackedScene scene = GD.Load<PackedScene>(Mesh);
			Node inst = scene.Instantiate();


			pos_vector = new Vector3(pos_vector.X, -0.25f, pos_vector.Z);

			inst.Set("position", TableController.calculatePosition(pos_vector));


			node = inst;

			TableController.tableGraphics.CallDeferred("add_child", inst);


			Random rand = new Random();
			this.ID = rand.Next(000000000, 999999999);

			setColor();

		}


		public Queen(string position, int team, Board board, bool firstMove, int ID, GameController game) : base(position, team, board, game)
		{
			this.firstMove = firstMove;
			this.ID = ID;
		}

		public override List<AvailableMove> CheckValidMoves(bool s)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(diagnolMoves(false, false));
			ans.AddRange(straightMoves(false, false));

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithCover()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(diagnolMoves(false, true));
			ans.AddRange(straightMoves(false, true));

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithKingProtection()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(diagnolMoves(false, false, true));
			ans.AddRange(straightMoves(false, false, true));

			return ans;
		}




		public override List<AvailableMove> CheckValidMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(diagnolMoves(false, false, board));
			ans.AddRange(straightMoves(false, false, board));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(diagnolMoves(false, false, board));
			ans.AddRange(straightMoves(false, false, board));

			return ans;
		}



		}
}
