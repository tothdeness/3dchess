﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;

namespace test.Bots.Resources
{
	public static class SquareScore
	{




		public static float ReadSquareScoreKnight(Vector3 positon)
		{

			return knights[(int)(positon.X - 1),(int)positon.Z - 1];
		}

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





		public static float ReadSquareScorePawn(Vector3 positon, int team)
		{
			if(team == 1)
			{
				return pawns[(int)(8 - positon.X  ), (int)positon.Z -  1 ];
			}
			return pawns[ (int)(positon.X - 1), (int)positon.Z - 1 ];
		}




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






	}
}