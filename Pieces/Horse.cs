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

			setColor();

		}

		public override List<AvailableMove> CheckValidMoves()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves());

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithCover()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(horseMoves());

			return ans;
		}
	}
}
