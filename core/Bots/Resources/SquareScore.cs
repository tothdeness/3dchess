using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace test.core.Bots.Resources
{
    public static class SquareScore
    {
        private static float ReadSquare(Vector3 position, int team, float[,] map)
        {
			if (team == 1)
			{
				return map[(int)(8 - position.X), (int)position.Z - 1];
			}

			return map[(int)(position.X - 1), (int)position.Z - 1];
		}

        public static float ReadSquareScoreKnight(Vector3 position) { return ReadSquare(position, 0, knights); }
		public static float ReadSquareScorePawn(Vector3 position, int team) { return ReadSquare(position, team, pawns); }
		public static float ReadSquareScoreKing(Vector3 position) { return ReadSquare(position, 0, kingEarlyGame); }
		public static float ReadSquareScoreBishop(Vector3 position,int team) { return ReadSquare(position, team, bishops); }
		public static float ReadSquareScoreRooks(Vector3 position, int team) { return ReadSquare(position, team, rooks); }

		public static readonly float[,] knights = {

        {-0.50f, -0.40f, -0.30f, -0.30f, -0.30f, -0.30f, -0.40f, -0.50f},

        {-0.40f, -0.20f,  0.00f,  0.00f,  0.00f,  0.00f, -0.20f, -0.40f},

        {-0.30f,  0.00f,  0.10f,  0.15f,  0.15f,  0.10f,  0.00f, -0.30f},

        {-0.30f,  0.05f,  0.15f,  0.20f,  0.20f,  0.15f,  0.05f, -0.30f},

        {-0.30f,  0.00f,  0.15f,  0.20f,  0.20f,  0.15f,  0.00f, -0.30f},

        {-0.30f,  0.05f,  0.10f,  0.15f,  0.15f,  0.10f,  0.05f, -0.30f},

        {-0.40f, -0.20f,  0.00f,  0.05f,  0.05f,  0.00f, -0.20f, -0.40f},

        {-0.50f, -0.40f, -0.30f, -0.30f, -0.30f, -0.30f, -0.40f, -0.50f}

        };

        public static readonly float[,] pawns = {

        {0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f},

        {0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f},

        {0.1f,  0.1f,  0.2f,  0.3f,  0.3f,  0.2f,  0.1f,  0.1f},

        {0.05f, 0.05f, 0.1f,  0.4f, 0.4f, 0.1f,  0.05f, 0.05f},

        {0.0f,  0.0f,  0.0f,  0.25f,  0.25f,  0.0f,  0.0f,  0.0f},

        {0.05f, -0.05f,-0.1f,  0.0f,  0.0f,-0.1f, -0.05f, 0.05f},

        {0.05f, 0.1f,  0.1f, -0.2f, -0.2f, 0.1f,  0.1f,  0.05f},

        {0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f}
    };


        public static readonly float[,] kingEarlyGame = {

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f},

            {-0.30f, -0.40f, -0.40f, -0.50f, -0.50f, -0.40f, -0.40f, -0.30f}

        };

		public static readonly float[,] bishops = {

	        {-2.5f, -1.25f, -1.25f, -1.25f, -1.25f, -1.25f, -1.25f, -2.5f},

	        {-1.25f,  0f,    0f,    0f,    0f,    0f,    0f,   -1.25f},

	        {-1.25f,  0f,    0.625f, 1.25f,  1.25f,  0.625f, 0f,   -1.25f},

	        {-1.25f,  0.625f, 0.625f, 1.25f,  1.25f,  0.625f, 0.625f, -1.25f},

	        {-1.25f,  0f,    1.25f,  1.25f,  1.25f,  1.25f,  0f,   -1.25f},

	        {-1.25f,  1.25f,  1.25f,  1.25f,  1.25f,  1.25f,  1.25f, -1.25f},

	        {-1.25f,  0.625f, 0f,    0f,    0f,    0f,    0.625f, -1.25f},

	        {-2.5f,  -1.25f, -1.25f, -1.25f, -1.25f, -1.25f, -1.25f, -2.5f}

        };

		public static readonly float[,] rooks = {

	        {0f,    0f,    0f,    0f,    0f,    0f,    0f,    0f},

	        {0.833f, 1.667f, 1.667f, 1.667f, 1.667f, 1.667f, 1.667f, 0.833f},

	        {-0.833f, 0f,    0f,    0f,    0f,    0f,    0f,   -0.833f},

	        {-0.833f, 0f,    0f,    0f,    0f,    0f,    0f,   -0.833f},

	        {-0.833f, 0f,    0f,    0f,    0f,    0f,    0f,   -0.833f},

	        {-0.833f, 0f,    0f,    0f,    0f,    0f,    0f,   -0.833f},

	        {-0.833f, 0f,    0f,    0f,    0f,    0f,    0f,   -0.833f},

	        {0f,    0f,    0f,    0.833f, 0.833f,  0f,    0f,    0f}

         };




	}
}
