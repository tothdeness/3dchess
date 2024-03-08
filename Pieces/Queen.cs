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

			inst.Set("position", tableController.calculatePosition(x,y));


		

			tableController.tableGraphics.AddChild(inst);

			node = inst;
			
		}

		public override List<Vector2> CheckValidMoves()
		{
			List<Vector2> results = new List<Vector2>();

			results.AddRange(diagnolMoves());

			return results;
		}

		public override void Move(int x, int y)
		{
			throw new NotImplementedException();
		}

		public override void ShowValidMoves()
		{
			tableController.showVisualizers(tableController.calculateVisualizers(CheckValidMoves()));
		}



	}
}
