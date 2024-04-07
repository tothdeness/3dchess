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

		static List<Char>  abc = new List<Char>();
		

		public static void add()
		{
			for(int i = 65; i < 73; i++)
			{
				abc.Add((char)i);
			}
		}


		public static void AddPiecesStandardGame()
		{
			add();
			//white pawns
			int i = 2;
			foreach (Char c in abc)
			{
				Pawn pawn = new Pawn($"{c}{i}", 1);
			}
			//black pawns
			i = 7;
			foreach (Char c in abc)
			{
				Pawn pawn = new Pawn($"{c}{i}", -1);
			}

			int s = 8;

			for (int f = -1; f < 2;f++)
			{
				Rook brook = new Rook("A"+s, f);
				Rook brook2 = new Rook("H"+s, f);
				Horse bhorse = new Horse("B"+s, f);
				Horse bhorse2 = new Horse("G"+s, f);
				Bishop bbishop = new Bishop("F"+s, f);
				Bishop bishop2 = new Bishop("C"+s, f);
				Queen bqueen = new Queen("D"+s, f);
				King bking = new King("E"+s, f);

				f++;
				s = 1;
			}




		}


	}
}
