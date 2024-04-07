using System;
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
		public Vector3 move;

		public bool attack;

		public Piece target;

		public bool cover;

		public bool enPassant = false;

		public Pawn canEnPassantRight;

		public Pawn canEnPassantLeft;

		public AvailableMove(Vector3 move, bool attack)
		{
			this.move = move;
			this.attack = attack;
		}


		public AvailableMove(Vector3 move, bool attack, Piece target) : this(move, attack)
		{
			this.target = target;
		}

		public AvailableMove(Vector3 move, bool attack, bool cover) : this(move, attack)
		{
			this.cover = cover;
		}

		public AvailableMove(Vector3 move, bool attack, Piece target, bool cover, bool enPassant, Pawn canEnPassantRight, Pawn canEnPassantLeft) : this(move, attack, target)
		{
			this.cover = cover;
			this.enPassant = enPassant;
			this.canEnPassantRight = canEnPassantRight;
			this.canEnPassantLeft = canEnPassantLeft;
		}
	}
}
