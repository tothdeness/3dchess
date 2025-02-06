using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;
using test.core.Pieces;
using test.core.Controllers;
using System.Text.Json.Serialization;
using static test.core.Network.Serializer;

namespace test.core.Pieces.Resources
{
    public class AvailableMove
    {
		public Piece moving { get; set; }

		[JsonConverter(typeof(GodotVector3JsonConverter))]
		public Vector3 oldPositon { get; set; }

		[JsonConverter(typeof(GodotVector3JsonConverter))]
		public Vector3 move { get; set; }

		public Piece rook { get; set; }

		[JsonConverter(typeof(GodotVector3JsonConverter))]
		public Vector3 rookNewPos { get; set; }

		[JsonConverter(typeof(GodotVector3JsonConverter))]
		public Vector3 rookOldPos { get; set; }
		public bool attack { get; set; }
		public Piece target { get; set; }
		public bool cover { get; set; }
		public bool castle { get; set; }
		public bool firstMove { get; set; }
		public bool promoted { get; set; }

        public AvailableMove() { }

		public AvailableMove(Piece moving, Vector3 move, bool attack, Vector3 oldPositon)
        {
            this.moving = moving;
            this.move = move;
            this.attack = attack;
            this.oldPositon = oldPositon;

        }

        public AvailableMove(Piece moving, Vector3 move, bool castle, Piece rook, bool firstMove, Vector3 oldPositon, Vector3 rookNewPos, Vector3 rookOldPos)
        {
            this.moving = moving;
            this.move = move;
            this.castle = castle;
            this.rook = rook;
            this.firstMove = firstMove;
            this.oldPositon = oldPositon;
            this.rookNewPos = rookNewPos;
            this.rookOldPos = rookOldPos;
        }


        public AvailableMove(Piece moving, Vector3 move, bool attack, Vector3 oldPositon, bool firstMove) : this(moving, move, attack, oldPositon)
        {
            this.firstMove = firstMove;
        }


        public AvailableMove(Piece moving, Vector3 move, bool attack, Piece target, Vector3 oldPositon) : this(moving, move, attack, oldPositon)
        {
            this.target = target;
        }

        public AvailableMove(Piece moving, Vector3 move, bool attack, Piece target, Vector3 oldPositon, bool firstMove) : this(moving, move, attack, oldPositon)
        {
            this.target = target;
            this.firstMove = firstMove;
        }


        public AvailableMove(Piece moving, Vector3 move, bool attack, bool cover, Vector3 oldPositon, bool firstMove) : this(moving, move, attack, oldPositon)
        {
            this.cover = cover;
            this.firstMove = firstMove;
        }


        public void toString()
        {
            GD.Print("CSapat: " + moving.team + " Mozgo babu: " + moving + " Jelenlegi pozi:  " + moving.position + " Uj pozi:  " + TableController.ConvertReverse(move) + (attack ? " TAMAD " : ""));
        }


        public string to_String()
        {
            return "CSapat: " + moving.team + " Mozgo babu: " + moving + " Jelenlegi pozi:  " + moving.position + " Uj pozi:  " + TableController.ConvertReverse(move) + (attack ? " TAMAD " : "");
        }

        public string hit_String()
        {
            return TableController.ConvertReverse(move);
        }



    }
}
