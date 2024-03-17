﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces
{
	internal class King : Piece
	{
		public King(string position, int team) : base(position, team)
		{
			PackedScene scene = GD.Load<PackedScene>("res://TSCN/king.tscn");
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

		}

		public override List<Vector3> CheckValidMoves()
		{
			List<Vector3> results = new List<Vector3>();

			results.AddRange(kingMoves());

			return results;
		}



	}
}
