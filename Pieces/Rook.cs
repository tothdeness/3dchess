﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces
{
	public class Rook : Piece
	{
		public Rook(string position, int team) : base(position, team)
		{

			PackedScene scene = GD.Load<PackedScene>("res://TSCN/rook.tscn");
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
