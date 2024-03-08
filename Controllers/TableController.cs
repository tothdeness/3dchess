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
	internal class TableController
	{
		private int tableSize;

		public List<Piece> table = new List<Piece>();
		public Node3D tableGraphics;

		private List<Node> visualizers = new List<Node>();

		public TableController(int tableSize, List<Piece> table, Node3D tableGraphics)
		{
			this.tableSize = tableSize;
			this.table = table;
			this.tableGraphics = tableGraphics;
		}

		public static Vector2 convert(string coordinate)
		{
			Vector2 result = new Vector2();

			result.X = coordinate[0] - 'A' + 1;

			result.Y = int.Parse(coordinate[1].ToString());



			return result;
		}

		public static string convertReverse(int[] cord)
		{
			return $"{(char)cord[0]}{cord[1]}";
		}


		public bool find(int x, int y)
		{
			foreach(Piece piece in table)
			{
				if(piece.x == x && piece.y == y)
				{
					return true;
				}

			}

			return false;
		}

		public List<Vector3> calculateVisualizers(List<Vector2> list)
		{
			List<Vector3> results = new List<Vector3> ();


			foreach(Vector2 p in list) {

				results.Add(new Vector3( (p.X) * 4 - 16.5f, 1.5f, (p.Y) * 4 - 16.5f));

			}

			return results;
		}


		public Vector3 calculatePosition(int x, int y)
		{
				return new Vector3(y * 4 - 18f, 3f, x * 4 - 18f);
		}




		public void showVisualizers(List<Vector3> list)
		{

			foreach (Vector3 p in list) {

				PackedScene scene = GD.Load<PackedScene>("res://validMove.tscn");
				Node inst = scene.Instantiate();
				tableGraphics.AddChild(inst);


			}


		}


		public void removeVisualizers()
		{

			foreach (Node p in visualizers) {
				p.QueueFree();
			}

		}





	}
}
