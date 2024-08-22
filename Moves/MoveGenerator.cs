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



		public static void CheckValidMoves(Board board)
		{
			ResetBoard(board);

			HitMap(board);

			if (!board.kingIsInDoubleCheck)
			{
				CheckPins(board);
			}

		}

		private static King CurrentKingPosition(Board board)
		{
			King a = (board.current == 1) ? board.kingWhite : board.kingBlack;
			return a;
		}

		private static void CheckPins(Board board)
		{
			King king = CurrentKingPosition(board);

			Vector3 safeKingPosition = king.posVector;

			Piece last_piece = null;

			foreach (Vector3 direction in directions)
			{
				Vector3 ij = safeKingPosition;

				last_piece = null;

				while (true)
				{
					ij += direction;

					if( Board.CheckBoundaries(ij) ) { break; }

					Piece piece = board.FindPiece(ij); 

					if( piece == null ) { continue; }

					//todo store checked positions

					if(last_piece != null)
					{
						bool contains = piece.directions.Contains(direction);

						if (last_piece.team == piece.team || !contains) { 
							break; 
						} else {
							last_piece.validDirections.Add(direction);
							break;
						}
						
					}

					if(piece.team != board.current && piece.directions.Contains(direction) && piece is not Pawn )
					{
						ValidPositionsToBlockCheck(direction,ij,safeKingPosition,board);
						return;
					} else if (piece.team != board.current)
					{
						break;
					}

					last_piece = piece;

				}

			}

		}


		//concurr

		private static void HitMap(Board board)
		{

			int kingCheckCounter = 0;

			foreach(var piece in board.table)
			{
				if(piece.Value.team != board.current)
				{
					foreach(AvailableMove move in piece.Value.CheckValidCoveredMovesOnVirtualBoard(board))
							{

								board.attackedSquares.Add(move.move);
										
								if(move.target is King && move.moving.team != move.target.team)
									{
										kingCheckCounter++;

										board.block.Add(move.moving.posVector);

										board.kingIsInCheck = true;
										
										if(kingCheckCounter > 1) {
									
										board.kingIsInDoubleCheck = true;


									}

								}
					}				
				}
			}

		}


		private static void ValidPositionsToBlockCheck(Vector3 direction,Vector3 attacker,Vector3 kingPosition, Board board)
		{		
			while(kingPosition != attacker)
			{
				kingPosition += direction;
				board.block.Add(kingPosition);

			}
	
		}



		private static void ResetBoard(Board board)
		{

			foreach(var pic in board.table)
			{
				pic.Value.validDirections.Clear();
			}

			board.block.Clear();
			board.attackedSquares.Clear();

			board.kingIsInCheck = false;
			board.kingIsInDoubleCheck = false;

		}



	}
}
