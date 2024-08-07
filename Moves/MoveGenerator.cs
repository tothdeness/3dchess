using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using test.Pieces;
using test.Pieces.Resources;
using static Godot.TextServer;

namespace test.Moves
{
	public static class MoveGenerator
	{

		private static List<Vector3> directions = new List<Vector3>
		{
		new Vector3(1, 0, 1),
		new Vector3(-1, 0, -1),
		new Vector3(1, 0, -1),
		new Vector3(-1, 0, 1),

		new Vector3(1, 0, 0),
		new Vector3(-1, 0, 0),
		new Vector3(0, 0, 1),
		new Vector3(0, 0, -1)

		};



		public static void checkValidMoves(Board board)
		{
			reset(board);

			board.attackedSquares =	hitMap(board);

			if (!board.kingIsInDoubleCheck)
			{
				checkPins(board);
			}



		}

		private static King currentKingPosition(Board board)
		{
			King a = (board.current == 1) ? board.kingWhite : board.kingBlack;
			return a;
		}

		private static void checkPins(Board board)
		{
			King king = currentKingPosition(board);

			Vector3 safeKingPosition = king.pos_vector;

			Piece last_piece = null;

			foreach (Vector3 direction in directions)
			{
				Vector3 ij = safeKingPosition;

				last_piece = null;

				while (true)
				{
					ij += direction;

					if( Board.checkboundries(ij) ) { break; }

					Piece piece = board.find_piece(ij); 

					if( piece == null ) { continue; }

					//todo store checked positions

					if(last_piece != null)
					{
							bool contains = piece.directions.ContainsKey(direction.ToString());

						if (last_piece.team == piece.team || !contains) { 
							break; 
						} else { 
							GD.Print(last_piece.position + " Valid direction:  " + direction);
							last_piece.validDirections.Add(direction);
							break;
						}
						
					}


					if(piece.team != board.current && piece.directions.ContainsKey(direction.ToString()) )
					{
						GD.Print(board.current + " KING IS IN CHECK BY A SLIDING PIECE!");
						board.block = validPositionsToBlockCheck(direction,ij,safeKingPosition);
						return;
					} else if (piece.team != board.current)
					{
						break;
					}

					last_piece = piece;

				}

			}

		}

		struct hitMapReturn
		{
			public HashSet<string> attackMap;
			public bool twoCheck;
			public hitMapReturn(HashSet<string> attackMap, bool twoCheck)
			{
				this.attackMap = attackMap;
				this.twoCheck = twoCheck;
			}
		}


		private static HashSet<string> hitMap(Board board)
		{

			HashSet<string> map = new HashSet<string>();

			int kingCheckCounter = 0;

			foreach(Piece hit in board.table)
			{
				if(hit.team != board.current)
				{
					foreach(AvailableMove move in hit.CheckValidCoveredMovesOnVirtualBoard(board))
							{
								map.Add(TableController.convertReverse(move.move));
								
								if(move.target is King && move.moving.team != move.target.team)
									{
										kingCheckCounter++;

										GD.Print(move.moving);

										board.block.Add(move.moving.pos_vector.ToString());

										board.kingIsInCheck = true;
										
										if(kingCheckCounter > 1) {

										GD.Print("ONLY KING CAN MOVE!");		
										
										board.kingIsInDoubleCheck = true;

										}

									}
							}
						{

					}
				}
			}

			return map;
		}


		private static HashSet<string> validPositionsToBlockCheck(Vector3 direction,Vector3 attacker,Vector3 kingPosition)
		{

			HashSet<string> ans = new HashSet<string>();
  		
			while(kingPosition != attacker)
			{
				kingPosition += direction;
				ans.Add(kingPosition.ToString());
				GD.Print(TableController.convertReverse(kingPosition));
			}

			return ans;

		}



		private static void reset(Board board)
		{

			foreach(Piece pic in board.table)
			{
				pic.validDirections.Clear();
			}

			board.block.Clear();
			board.attackedSquares.Clear();
			board.kingIsInCheck = false;
			board.kingIsInDoubleCheck = false;

		}

		private static void printOutHitMap(HashSet<string> map)
		{
			foreach(string hit in map)
			{
				GD.Print(hit);
			}

			GD.Print();

		}	




	}
}
