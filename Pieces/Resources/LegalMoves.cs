using Godot;
using System.Collections.Generic;
using test.Controllers;


namespace test.Pieces.Resources
{
    public class LegalMoves
    {

        public int team;

        public int enemy;

        public Dictionary<Vector3, Vector3> legalSquares;


        
        public static bool CheckStalemate(Board board, int team)
        {
            
            foreach(Piece piece in board.table)
            {
                if(piece.team == team && piece.CheckValidMovesWithKingProtection().Count > 0)
                {
                    return false;
                }
            } 

            return true;
        }


		public static bool GameOver(Board board,int team) {

			return (CheckStalemate(board, team) && KingCheck(board, team));
		}



		public static bool KingCheck(Board board, int team)
		{
			foreach (Piece piec in board.table)
			{
				if (team != piec.team)
				{
					foreach (AvailableMove move in piec.CheckValidMovesVirt(board))
					{
						if (move.target is King)
						{
							return true;
						}
					}
				}

			}
			return false;
		}




	}
}
