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

		public Piece moving;

		public Vector3 oldPositon;

		public Vector3 move;

		public Vector3 rookNewPos;

		public bool attack;

		public Piece target;

		public bool cover;

		public bool enPassant = false;

		public Piece covered;

		public Pawn canEnPassantRight;

		public Pawn canEnPassantLeft;

		public bool kingSideCastling;

		public bool queenSideCastling;

		public bool firstMove;


		public AvailableMove(Piece moving,Vector3 move, bool attack, Vector3 oldPositon)
		{
			this.moving = moving;
			this.move = move;
			this.attack = attack;
			this.oldPositon = oldPositon;

		}

		public AvailableMove(Piece moving,Vector3 move,bool attack,Vector3 oldPositon,bool firstMove) : this(moving, move, attack, oldPositon)
		{
			this.firstMove = firstMove;
		}


		public AvailableMove(Piece moving,Vector3 move, bool attack, Piece target, Vector3 oldPositon) : this(moving, move, attack, oldPositon)
		{
			this.target = target;
		}

		public AvailableMove(Piece moving, Vector3 move, bool attack, Piece target, Vector3 oldPositon, bool firstMove) : this(moving, move, attack, oldPositon)
		{
			this.target = target;
			this.firstMove=firstMove;
		}


		public AvailableMove(Piece moving, Vector3 move, bool attack, bool cover, Vector3 oldPositon) : this(moving,move, attack, oldPositon)
		{
			this.cover = cover;
		}

		public AvailableMove(Vector3 move, Piece covered)
		{
			this.move = move;
			this.covered = covered;
		}


		public AvailableMove(Piece moving,Vector3 move, bool attack, Piece target, bool cover, bool enPassant, Pawn canEnPassantRight, Pawn canEnPassantLeft, Vector3 oldPositon) : this(moving, move, attack, target, oldPositon)
		{
			this.cover = cover;
			this.enPassant = enPassant;
			this.canEnPassantRight = canEnPassantRight;
			this.canEnPassantLeft = canEnPassantLeft;
		}


		public void toString()
		{
			GD.Print("CSapat: "+ this.moving.team  + " Mozgo babu: " + this.moving + " Jelenlegi pozi:  " + this.moving.position + " Uj pozi:  " + TableController.convertReverse(this.move) + (this.attack ? " TAMAD " : ""));
		}


		public string to_String()
		{
			return "CSapat: " + this.moving.team + " Mozgo babu: " + this.moving + " Jelenlegi pozi:  " + this.moving.position + " Uj pozi:  " + TableController.convertReverse(this.move) + (this.attack ? " TAMAD " : "");
		}

		public string hit_String()
		{
			return TableController.convertReverse(this.move);
		}



	}
}
