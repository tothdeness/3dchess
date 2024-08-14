using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using test.Pieces;
using test.Pieces.Resources;

namespace test.Mode
{
	public class SetupBaseGame
	{




		public static void AddPiecesStandardGame(GameController game, Board board)
		{
			// Create a list to store all pieces
			List<Piece> pieces = new List<Piece>();

			// WHITE
			pieces.Add(new Rook("A1", 1, game));
			pieces.Add(new Rook("H1", 1, game));
			pieces.Add(new Horse("B1", 1, game));
			pieces.Add(new Horse("G1", 1, game));
			pieces.Add(new Bishop("F1", 1, game));
			pieces.Add(new Bishop("C1", 1, game));

			pieces.Add(new Pawn("A2", 1, game));
			pieces.Add(new Pawn("B2", 1, game));
			pieces.Add(new Pawn("C2", 1, game));
			pieces.Add(new Pawn("D2", 1, game));
			pieces.Add(new Pawn("E2", 1, game));
			pieces.Add(new Pawn("F2", 1, game));
			pieces.Add(new Pawn("G2", 1, game));
			pieces.Add(new Pawn("H2", 1, game));

			pieces.Add(new Queen("D1", 1, game));

			pieces.Add(new King("E1", 1, game));

			// BLACK
			pieces.Add(new Rook("A8", -1, game));
			//pieces.Add(new Rook("H8", -1, game));
			//pieces.Add(new Horse("B8", -1, game));
			//pieces.Add(new Horse("G8", -1, game));
			pieces.Add(new Bishop("F8", -1, game));
			pieces.Add(new Bishop("C8", -1, game));


			pieces.Add(new Pawn("A7", -1, game));
			pieces.Add(new Pawn("B7", -1, game));
			pieces.Add(new Pawn("C7", -1, game));
			pieces.Add(new Pawn("D7", -1, game));
			pieces.Add(new Pawn("E7", -1, game));
			pieces.Add(new Pawn("F7", -1, game));
			pieces.Add(new Pawn("G7", -1, game));
			pieces.Add(new Pawn("H7", -1, game));

			//pieces.Add(new Queen("D8", -1, game));

			pieces.Add(new King("E8", -1, game));

			// Add visuals and add to game


			// Assign kings to the board
			board.kingBlack = pieces.OfType<King>().FirstOrDefault(p => p.team == -1);
			board.kingWhite = pieces.OfType<King>().FirstOrDefault(p => p.team == 1);

			// Call addToGame on each piece
			foreach (Piece piece in pieces)
			{
				piece.addToGame();
				piece.addVisuals();

			}
		}


	}
}
