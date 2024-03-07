using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces
{
	internal abstract  class Piece
	{

		Node node { get; set; }

		private string position;

		private int logicCoordinate {  get; set; }

		private bool real { get; set; }

		int x { get; set; }
		
		int y { get; set; }

		//0 black, 1 white
		private int team { get; set; }

		public static TableController tableController;

		public abstract List<Vector3> CheckValidMoves();

		public abstract void Move(int x, int y);




	}
}
