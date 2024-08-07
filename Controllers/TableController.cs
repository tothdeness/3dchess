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



	
		public static List<Piece> table = new List<Piece>();
		public static Node3D tableGraphics;
		public static Piece current;
		private static List<Node> visualizers = new List<Node>();

		public static Vector3 convert(string coordinate)
		{
			Vector3 result = new Vector3();

			result.Z = coordinate[0] - 'A' + 1;

			result.X = int.Parse(coordinate[1].ToString());



			return result;
		}

		public static Piece findPieceID(int id)
		{
			foreach(var piece in table)
			{
				if(piece.ID == id)
				{
					return piece;
				}
				
			}
			return null;
		}


		public static string convertReverse(Vector3 vector)
		{
			int x = (int)vector.Z + 64;
			int z = (int)vector.X + 48;

			char charX = (char)x;
			char charZ = (char)z;

			return $"{charX}{charZ}";
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
			return new Vector3((vec.X + 18) / 4, vec.Y, (vec.Z + 18) / 4);
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

				validMove a = (validMove)inst; a.target = p;
				//if (p.attack) {validMove a = (validMove)inst; a.target = p;}

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
