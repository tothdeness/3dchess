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


		public AvailableMove(Vector3 move, bool attack)
		{
			this.move = move;
			this.attack = attack;
		}


		public AvailableMove(Vector3 move, bool attack, Piece target) : this(move, attack)
		{
			this.target = target;
		}


	}
}
