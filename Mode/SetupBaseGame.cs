using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;

namespace test.Mode
{
	public class SetupBaseGame
	{




		public static void AddPiecesStandardGame()
		{

			// WHITE

			Rook white_rook = new Rook("A1", 1);
			Rook white_rook2 = new Rook("H1", 1);
			Horse white_horse = new Horse("B1", 1);
			Horse white_horse2 = new Horse("G1", 1);
			Bishop white_bishop = new Bishop("F1", 1);
			Bishop white_bishop2 = new Bishop("C1", 1);
			King white_king = new King("E1", 1);

			Pawn white_pawn1 = new Pawn("A2", 1);
			Pawn white_pawn2 = new Pawn("B2", 1);
			Pawn white_pawn3 = new Pawn("C2", 1);
			Pawn white_pawn4 = new Pawn("D2", 1);
			Pawn white_pawn5 = new Pawn("E2", 1);
			Pawn white_pawn6 = new Pawn("F2", 1);
			Pawn white_pawn7 = new Pawn("G2", 1);
			Pawn white_pawn8 = new Pawn("H2", 1);

			// BLACK

			Rook black_rook = new Rook("A8", -1);
			Rook black_rook2 = new Rook("H8", -1);
			Horse black_horse = new Horse("B8", -1);
			Horse black_horse2 = new Horse("G8", -1);
			Bishop black_bishop = new Bishop("F8", -1);
			Bishop black_bishop2 = new Bishop("C8", -1);
			King black_king = new King("E8", -1);


			Pawn black_pawn1 = new Pawn("A7", -1);
			Pawn black_pawn2 = new Pawn("B7", -1);
			Pawn black_pawn3 = new Pawn("C7", -1);
			Pawn black_pawn4 = new Pawn("D7", -1);
			Pawn black_pawn5 = new Pawn("E7", -1);
			Pawn black_pawn6 = new Pawn("F7", -1);
			Pawn black_pawn7 = new Pawn("G7", -1);
			Pawn black_pawn8 = new Pawn("H7", -1);

			Queen black_queen = new Queen("D8", -1);
			Queen white_queen = new Queen("D1", 1);

		}

	}
}
