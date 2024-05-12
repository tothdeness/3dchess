using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using test.Controllers;



namespace test.Pieces
{

	public class Pawn : Piece
	{


		public Pawn(string position, int team) : base(position, team)
		{
	
				PackedScene scene = GD.Load<PackedScene>("res://TSCN/pawn.tscn");
				Node inst = scene.Instantiate();

				y = 1;

				inst.Set("position", TableController.calculatePosition(new Vector3(pos_vector.X, y, pos_vector.Z)));

				TableController.tableGraphics.AddChild(inst);

				node = inst;

				Random rand = new Random();
				this.ID = rand.Next(000000000, 999999999);

				setColor();
	
		}


		public Pawn(string position, int team, Board board, bool firstMove, int ID) : base(position, team, board) {
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

				Queen q = new Queen(position, team);

			}

		}

		public void promotePawn(Board board)
		{
			if (pos_vector.X == 8 || pos_vector.X == 1)
			{
				Queen q = new Queen(position, team, board, false, this.ID);

				board.table.Remove(board.findPieceID(this.ID));
			}

		}




		public override Pawn Clone(Piece p)
		{
			return new Pawn(p.position,p.team);
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

			ans.AddRange(pawnMoves(false, true, board));

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
