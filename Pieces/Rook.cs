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

			Random rand = new Random();
			this.ID = rand.Next(000000000, 999999999);

			setColor();

		}


		public Rook(string position, int team, Board board, bool firstMove, int ID) : base(position, team, board)
		{
			this.firstMove = firstMove;
			this.ID = ID;
		}

		public override List<AvailableMove> CheckValidMoves(bool s)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(straightMoves(false, false));

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithCover()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(straightMoves(false, true));

			return ans;
		}

		public override List<AvailableMove> CheckValidMovesWithKingProtection()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(straightMoves(false, false, true));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(straightMoves(false, false, board));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(straightMoves(false, false, board));

			return ans;

		}

	}
}
