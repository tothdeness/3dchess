using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Pieces
{
	public class Board
	{
		public  List<Piece> table = new List<Piece>();

		public	List<Pawn> enPassant = new List<Pawn>();

		public Board(List<Piece> table, List<Pawn> enPassant)
		{
			this.table = table;
			this.enPassant = enPassant;
		}


		public Piece findPieceID(int id)
		{
			foreach (var piece in table)
			{
				if (piece.ID == id)
				{
					return piece;
				}

			}
			return null;
		}


		//PAWN
		public  void emptyEnpassant()
		{

			foreach (Pawn pawn in enPassant)
			{
				pawn.enPassant = false;
				pawn.enPassantLeft = null;
				pawn.enPassantRight = null;
			}

			enPassant.Clear();

		}


		public  bool find(Vector3 vector)
		{
			foreach (Piece piece in table)
			{
				if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z)
				{
					return true;
				}

			}

			return false;
		}



		public  Pawn findPawn(Vector3 vector, int team)
		{
			foreach (Piece piece in table)
			{
				if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z && piece.team != team && piece is Pawn)
				{
					return (Pawn)piece;
				}

			}

			return null;
		}



		public struct target
		{

			public int num;
			public Piece p;

			public target(int num, Piece p)
			{
				this.num = num;
				this.p = p;
			}

		}

		public  target find(Vector3 vector, Piece curr)
		{
			foreach (Piece piece in table)
			{
				if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z && piece.team == curr.team)
				{
					return new target(1, piece);

				}
				else if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z && piece.team != curr.team)
				{
					return new target(2, piece);
				}

			}

			return new target(0, null);
		}




		public  Piece find(Node node)
		{

			foreach (Piece piece in table)
			{
				if (piece.node.Equals(node))
				{
					return piece;

				}

			}

			return null;
		}




	}
}
