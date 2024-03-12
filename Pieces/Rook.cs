﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces
{
	internal class Rook : Piece
	{
		public Rook(string position, int team) : base(position, team)
		{

			PackedScene scene = GD.Load<PackedScene>("res://rook.tscn");
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(x, y));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

		}

		public override List<Vector2> CheckValidMoves()
		{
			List<Vector2> results = new List<Vector2>();

			results.AddRange(straightMoves());

			return results;
		}




	}
}
