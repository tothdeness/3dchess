using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;
using test.Pieces.Resources;


namespace test.Controllers
{
    public static class TableController
	{
	
		public static Dictionary<string,Piece> table = new Dictionary<string, Piece>();
		public static Node3D tableGraphics;
		public static Piece current;
		private static List<Node> visualizers = new List<Node>();

		public static Vector3 Convert(string coordinate)
		{
			Vector3 result = new Vector3();

			result.Z = coordinate[0] - 'A' + 1;

			result.Y = -0.25f;

			result.X = int.Parse(coordinate[1].ToString());

			return result;
		}

		public static string ConvertReverse(Vector3 vector)
		{
			int x = (int)vector.Z + 64;
			int z = (int)vector.X + 48;

			char charX = (char)x;
			char charZ = (char)z;

			return $"{charX}{charZ}";
		}


		public struct Target{

			public int num;
			public Piece p;

			public Target(int num, Piece p)
			{
				this.num = num;
				this.p = p;
			}

		}


		public static Piece Find(Node node)
		{

			foreach(KeyValuePair<string,Piece> piece in table)
			{
				if (piece.Value.node.Equals(node))
				{
					return piece.Value;

				}

			}

			return null;
		}



		public static List<AvailableMove> CalculateVisualizers(List<AvailableMove> list)
		{

			foreach (AvailableMove p in list) {

			 p.move = new Vector3(p.move.X * 4 - 18f, -0.35f, p.move.Z * 4 - 18f);

			}

			return list;
		}


		public static Vector3 CalculatePosition(Vector3 v)
		{
				return new Vector3(v.X * 4 - 18, v.Y, v.Z * 4 - 18);
		}


		public static Vector3 ReversePosition(Vector3 vec)
		{
			return new Vector3((vec.X + 18) / 4, vec.Y, (vec.Z + 18) / 4);
		}



		public static void ShowVisualizers(List<AvailableMove> list, Piece piece)
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

				validMove a = (validMove) inst; a.target = p;
				//if (p.attack) {validMove a = (validMove)inst; a.target = p;}

			}

			//GD.Print(piece.y + " " + piece.x);


		}

		public static void RemoveVisualizers()
		{

			foreach (Node p in visualizers) {
				p.QueueFree();

			}

	
			 visualizers.Clear();


		}



	}
}
