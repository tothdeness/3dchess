using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces.Resources
{
    public class Board
    {
        public List<Piece> table = new List<Piece>();

        public King kingWhite;

        public King kingBlack;

        public int current;

        public bool kingIsInCheck = false;

        public bool kingIsInDoubleCheck = false;

        public HashSet<string> attackedSquares = new HashSet<string>();

        public HashSet<string> block = new HashSet<string>();

        public Board(List<Piece> table, List<Pawn> enPassant)
        {
            this.table = table;
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


        public bool find(Vector3 vector)
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



        public Pawn findPawn(Vector3 vector, int team)
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

        public target find(Vector3 vector, Piece curr)
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


		public Piece find_piece(Vector3 vector)
		{
			foreach (Piece piece in table)
			{
				if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z)
				{
					return piece;

				}

			}

			return null;
		}




		public Piece find(Node node)
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

        //return true if the target position is outside of the map
        public static bool checkboundries(Vector3 ij)
        {
            return ij.Z < 1 || ij.Z > 8 || ij.X < 1 || ij.X > 8;
		}


        public List<AvailableMove> checkAllMoves()
        {
            List<AvailableMove> moves = new List<AvailableMove> ();

            foreach (Piece piece in table)
            {
                if(piece.team != current) { continue; }
                moves.AddRange(piece.CheckValidMovesVirt(this));
            }
            return moves;
        }


        public struct gameState
        {
            public string name;
            public int id;
            public int winner;
			public gameState(string name, int id,int winner)
			{
				this.name = name;
				this.id = id;
                this.winner = winner;
			}
		}


        // 0 = game is not over 1 = game is over (current player won) 2 = stalemate
        public gameState checkGameState(List<AvailableMove> moves)
        {
            if(kingIsInCheck && moves.Count == 0)
            {
                return new gameState("Game is over! Winner is " + (current == 1 ? "Black" : "White"),1,current * -1);

            }else if(!kingIsInCheck && moves.Count == 0)
            {
                return new gameState("Game is ended in a stalemate!",2,0);
            }

            return new gameState("Game is not over.",0,0);
        }


    }
}
