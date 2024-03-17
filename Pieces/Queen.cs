﻿using Godot;
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
	public class Queen : Piece
	{

		public Queen(string position, int team) : base(position,team)
		{
			PackedScene scene = GD.Load<PackedScene>("res://TSCN/mesh.tscn");
			Node inst = scene.Instantiate();

			inst.Set("position", TableController.calculatePosition(pos_vector));

			TableController.tableGraphics.AddChild(inst);

			node = inst;

			setColor();

		}

		public override List<AvailableMove> CheckValidMoves()
		{
			throw new NotImplementedException();
		}



	}
}
