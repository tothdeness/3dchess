﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;

namespace test.Controllers
{
	public class AvailableMove
	{

		public Piece moving;

		public Vector3 move;

		public bool attack;

		public Piece target;

		public bool cover;

		public bool enPassant = false;

		public Piece covered;

		public Pawn canEnPassantRight;

		public Pawn canEnPassantLeft;

		bool shortCastling;

		bool longCastling;

		public AvailableMove(Piece moving,Vector3 move, bool attack)
		{
			this.move = move;
			this.attack = attack;
		}


		public AvailableMove(Piece moving,Vector3 move, bool attack, Piece target) : this(moving, move, attack)
		{
			this.target = target;
		}

		public AvailableMove(Piece moving, Vector3 move, bool attack, bool cover) : this(moving,move, attack)
		{
			this.cover = cover;
		}

		public AvailableMove(Vector3 move, Piece covered)
		{
			this.move = move;
			this.covered = covered;
		}


		public AvailableMove(Piece moving,Vector3 move, bool attack, Piece target, bool cover, bool enPassant, Pawn canEnPassantRight, Pawn canEnPassantLeft) : this(moving, move, attack, target)
		{
			this.cover = cover;
			this.enPassant = enPassant;
			this.canEnPassantRight = canEnPassantRight;
			this.canEnPassantLeft = canEnPassantLeft;
		}
	}
}
