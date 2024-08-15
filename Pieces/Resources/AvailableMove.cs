using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;
using test.Pieces;
using test.Controllers;

namespace test.Pieces.Resources
{
    public class AvailableMove
    {

        public Piece moving;

        public Vector3 oldPositon;

        public Vector3 move;

        public Vector3 rookNewPos;

        public bool attack;

        public Piece target;

        public bool cover;

        public bool enPassant = false;

        public Piece covered;

        public Pawn canEnPassantRight;

        public Pawn canEnPassantLeft;

        public bool kingSideCastling;

        public bool queenSideCastling;

        public bool firstMove;

        public bool promoted;

        public AvailableMove(Piece moving, Vector3 move, bool attack, Vector3 oldPositon)
        {
            this.moving = moving;
            this.move = move;
            this.attack = attack;
            this.oldPositon = oldPositon;

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
            GD.Print("CSapat: " + moving.team + " Mozgo babu: " + moving + " Jelenlegi pozi:  " + moving.position + " Uj pozi:  " + TableController.convertReverse(move) + (attack ? " TAMAD " : ""));
        }


        public string to_String()
        {
            return "CSapat: " + moving.team + " Mozgo babu: " + moving + " Jelenlegi pozi:  " + moving.position + " Uj pozi:  " + TableController.convertReverse(move) + (attack ? " TAMAD " : "");
        }

        public string hit_String()
        {
            return TableController.convertReverse(move);
        }



    }
}
