using Chess;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;

namespace test.Pieces.Resources
{
	public class Board
	{
		public Dictionary<string,Piece> table = new Dictionary<string, Piece>();

		public King kingWhite;

		public King kingBlack;

		public int current;

		public bool kingIsInCheck = false;

		public bool kingIsInDoubleCheck = false;

		public HashSet<string> attackedSquares = new HashSet<string>();

		public HashSet<string> block = new HashSet<string>();

		public Board(Dictionary<string, Piece> table, List<Pawn> enPassant)
		{
			this.table = table;
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

			bool ans = table.TryGetValue(vector.ToString(), out Piece value);

			if(!ans) { return new target(0, null); }

			if(value.team == curr.team)
			{
				return new target(1, value);
			}
			else
			{
				return new target(2, value);
			}

	
		}


		public void updatekey(AvailableMove move)
		{
			table.Remove(move.oldPositon.ToString());
			table.Add(move.moving.pos_vector.ToString(), move.moving);

		}

		public void updateWithAttack(AvailableMove move)
		{
			table.Remove(move.oldPositon.ToString());
			table[move.target.pos_vector.ToString()] = move.moving;

		}


		public Piece find_piece(Vector3 vector)
		{
			return table.TryGetValue(vector.ToString(), out Piece value) ? value : null;
		}



		//return true if the target position is outside of the map
		public static bool checkboundries(Vector3 ij)
		{
			return ij.Z < 1 || ij.Z > 8 || ij.X < 1 || ij.X > 8;
		}


		public List<AvailableMove> checkAllMoves()
		{
			List<AvailableMove> moves = new List<AvailableMove> ();

			foreach (KeyValuePair<string,Piece> piece in table)
			{
				if(piece.Value.team != current) { continue; }
				moves.AddRange(piece.Value.CheckValidMovesVirt(this));
			}
			return moves;
		}



		public void takeBackMove(AvailableMove move)
		{
			if (move.firstMove) { move.moving.firstMove = true; }

			if (move.attack)
			{
				table[move.moving.pos_vector.ToString()] = move.target;
				table.Add(move.oldPositon.ToString(), move.moving);
				move.moving.pos_vector = move.oldPositon;
				return;
			}

			move.moving.pos_vector = move.oldPositon;
			table.Remove(move.move.ToString());
			table.Add(move.oldPositon.ToString(), move.moving);
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
