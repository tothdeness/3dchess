using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;
using test.Pieces.Resources;



namespace test.Pieces
{

	public class Pawn : Piece
	{


		public Pawn(string position, int team, GameController game) : base(position, team, game)
		{

				Mesh = "res://TSCN/pawn.tscn";

				PackedScene scene = GD.Load<PackedScene>(Mesh);
				Node inst = scene.Instantiate();


				pos_vector = new Vector3(pos_vector.X, -0.25f , pos_vector.Z);

				inst.Set("position", TableController.calculatePosition(pos_vector));

				TableController.tableGraphics.AddChild(inst);

				node = inst;

				Random rand = new Random();
				this.ID = rand.Next(000000000, 999999999);

				setColor();
	
		}


		public Pawn(string position, int team, Board board, bool firstMove, int ID, GameController game) : base(position, team, board, game) {
			this.firstMove = firstMove;
			this.ID = ID;
		}


		public override List<AvailableMove> CheckValidMoves(bool s)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(pawnMoves(false,false,s));

			return ans;

		}

		public void promotePawn()
		{
			if(pos_vector.X == 8 || pos_vector.X == 1)
			{
				Delete();

				Queen q = new Queen(position, team, this.gameController);

			}

		}

		public void promotePawn(Board board)
		{
			if (pos_vector.X == 8 || pos_vector.X == 1)
			{
				Queen q = new Queen(position, team, board, false, this.ID, this.gameController);

				board.table.Remove(board.findPieceID(this.ID));
			}

		}




		public override Pawn Clone(Piece p)
		{
			return new Pawn(p.position,p.team,this.gameController);
		}



		public override List<AvailableMove> CheckValidMovesWithCover()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(pawnMoves(true,false,false));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesWithKingProtection()
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(pawnMoves(false, true, true));

			return ans;
		}



		public override List<AvailableMove> CheckValidMovesOnVirtualBoard(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(pawnMoves(false, false, board));

			return ans;
		}


		public override List<AvailableMove> CheckValidMovesVirt(Board board)
		{
			List<AvailableMove> ans = new List<AvailableMove>();

			ans.AddRange(pawnMoves(false, false, board));

			return ans;

		}




	}
}
