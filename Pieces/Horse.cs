using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces
{
	public class Horse : Piece
	{
		public Horse(string position, int team) : base(position, team)
		{
			PackedScene scene = GD.Load<PackedScene>("res://TSCN/horse.tscn");
			Node inst = scene.Instantiate();

			y = 1;

			inst.Set("position", TableController.calculatePosition(new Vector3(pos_vector.X, y, pos_vector.Z)));

			TableController.tableGraphics.AddChild(inst);

			node = inst;


			Random rand = new Random();
			this.ID = rand.Next(000000000, 999999999);


			setColor();

		}


		public Horse(string position, int team, Board board, bool firstMove, int ID) : base(position, team, board)
		{
			this.firstMove = firstMove;
			this.ID = ID;
		}


		public override List<AvailableMove> CheckValidMoves(bool s)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves(false));

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithCover()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves(true));

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithKingProtection()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves(false,true));

			return ans;
		}





		public override List<AvailableMove> CheckValidMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves(false, true, board));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves(false, false, board));

			return ans;

		}


	}
}
