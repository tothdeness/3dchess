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


		public static Vector2 convert(string coordinate)
		{
			Vector2 result = new Vector2();

			result.X = coordinate[0] - 'A' + 1;

			result.Y = int.Parse(coordinate[1].ToString());



			return result;
		}

		public static string convertReverse(Vector2 vector)
		{
			return $"{(char)vector.X}{vector.Y}";
		}


		public static bool find(Vector2 vector)
		{
			foreach(Piece piece in table)
			{
				if(piece.x == vector.X && piece.y == vector.Y)
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






		public static List<Vector3> calculateVisualizers(List<Vector2> list)
		{
			List<Vector3> results = new List<Vector3> ();


			foreach(Vector2 p in list) {

				results.Add(new Vector3( p.Y * 4 - 18f, -0.35f, p.X * 4 - 18f));
			}

			return results;
		}


		public static Vector3 calculatePosition(int x, int y)
		{
				return new Vector3(y * 4 - 18, 3, x * 4 - 18);
		}


		public static Vector2 reversePosition(Vector3 vec)
		{
			return new Vector2( (vec.Z + 18 ) / 4 , (vec.X + 18) / 4);
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


			GD.Print(piece.y + " " + piece.x);


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
