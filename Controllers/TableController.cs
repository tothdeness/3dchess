using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;


namespace test.Controllers
{
	internal static class TableController
	{
		private static int tableSize;

		public static List<Piece> table = new List<Piece>();
		public static Node3D tableGraphics;
		public static Piece current;

		private static List<Node> visualizers = new List<Node>();


		public static Vector3 convert(string coordinate)
		{
			Vector3 result = new Vector3();

			result.X = coordinate[0] - 'A' + 1;

			result.Z = int.Parse(coordinate[1].ToString());



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






		public static List<Vector3> calculateVisualizers(List<Vector3> list)
		{
			List<Vector3> results = new List<Vector3> ();


			foreach(Vector3 p in list) {

				results.Add(new Vector3( p.X * 4 - 18f, -0.35f, p.Z * 4 - 18f));
			}

			return results;
		}


		public static Vector3 calculatePosition(Vector3 v)
		{
				return new Vector3(v.X * 4 - 18, 3, v.Z * 4 - 18);
		}


		public static Vector3 reversePosition(Vector3 vec)
		{
			return new Vector3((vec.X + 18) / 4, 3, (vec.Z + 18) / 4);
		}



		public static void showVisualizers(List<Vector3> list, Piece piece)
		{

			foreach (Vector3 p in list) {

				PackedScene scene = GD.Load<PackedScene>("res://validMove.tscn");
				Node inst = scene.Instantiate();
				inst.Set("position",p);
				visualizers.Add(inst);
				tableGraphics.AddChild(inst);
				current = piece;

			}


			//GD.Print(piece.y + " " + piece.x);


		}


		public static void removeVisualizers()
		{

			foreach (Node p in visualizers) {
				p.QueueFree();
			}
			visualizers.Clear();
			current = null;

		}





	}
}
