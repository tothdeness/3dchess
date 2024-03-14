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

namespace test.Pieces
{
	internal class Queen : Piece
	{

		public Queen(string position, int team) : base(position,team)
		{
			PackedScene scene = GD.Load<PackedScene>("res://mesh.tscn");
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;
			
		}

		public override List<Vector3> CheckValidMoves()
		{
			List<Vector3> results = new List<Vector3>();

			results.AddRange(diagnolMoves());
			results.AddRange(straightMoves());

			return results;
		}



	}
}
