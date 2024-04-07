using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;


namespace test.Controllers
{
	public static class TableController
	{



		private static int tableSize;
		public static List<Piece> table = new List<Piece>();
		public static Node3D tableGraphics;
		public static Piece current;
		private static List<Node> visualizers = new List<Node>();
		public struct enPassantStruct
		{

			public Piece target;
			public Piece attacker;

			public enPassantStruct(Piece target, Piece attacker)
			{
				this.target = target;
				this.attacker = attacker;
			}
		}


		public static List<enPassantStruct> enPassantpieces = new List<enPassantStruct>();



		public static Vector3 convert(string coordinate)
		{
			Vector3 result = new Vector3();

			result.Z = coordinate[0] - 'A' + 1;

			result.X = int.Parse(coordinate[1].ToString());



			return result;
		}

		public static string convertReverse(Vector3 vector)
		{
			int x = (int)vector.Z + 64;
			int z = (int)vector.X + 48;

			char charX = (char)x;
			char charZ = (char)z;

			return $"{charX}{charZ}";
		}


		public static bool find(Vector3 vector)
		{
			foreach(Piece piece in table)
			{
				if(piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z)
				{
					return true;
				}

			}

			return false;
		}



		public static Pawn findPawn(Vector3 vector,int team)
		{
			foreach (Piece piece in table)
			{
				if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z && piece.team != team && piece is Pawn)
				{
					return (Pawn) piece;
				}

			}

			return null;
		}



		public struct target{

			public int num;
			public Piece p;

			public target(int num, Piece p)
			{
				this.num = num;
				this.p = p;
			}

		}

		public static target find(Vector3 vector,Piece curr)
		{
			foreach (Piece piece in table)
			{
				if (piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z && piece.team == curr.team)
				{
					return new target(1,piece);

				} else if(piece.pos_vector.X == vector.X && piece.pos_vector.Z == vector.Z && piece.team != curr.team)
				{
					return new target(2,piece);
				}

			}

			return new target(0,null);
		}




		public static Piece find(Node node)
		{

			foreach(Piece piece in table)
			{
				if (piece.node.Equals(node))
				{
					return piece;

				}

			}

			return null;
		}






		public static List<AvailableMove> calculateVisualizers(List<AvailableMove> list)
		{

			foreach (AvailableMove p in list) {

			 p.move = new Vector3(p.move.X * 4 - 18f, -0.35f, p.move.Z * 4 - 18f);

			}

			return list;
		}


		public static Vector3 calculatePosition(Vector3 v)
		{
				return new Vector3(v.X * 4 - 18, v.Y, v.Z * 4 - 18);
		}


		public static Vector3 reversePosition(Vector3 vec)
		{
			return new Vector3((vec.X + 18) / 4, 3, (vec.Z + 18) / 4);
		}



		public static void showVisualizers(List<AvailableMove> list, Piece piece)
		{

			string at = "attackMove.tscn";
			string mo = "validMove.tscn";

			current = piece;

			foreach (AvailableMove p in list) {

				PackedScene scene = GD.Load<PackedScene>("res://TSCN/" + (p.attack ? at : mo));
				Node inst = scene.Instantiate();
				inst.Set("position",p.move);
				visualizers.Add(inst);
				tableGraphics.AddChild(inst);

				if (p.attack) {validMove a = (validMove)inst; a.target = p;}

			}


			//GD.Print(piece.y + " " + piece.x);


		}


		public static void removeVisualizers()
		{

			foreach (Node p in visualizers) {
				p.QueueFree();

			}

		
			 visualizers.Clear();


		}





	}
}
