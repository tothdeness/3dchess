using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using test.Controllers;
using test.Pieces;

public partial class main : Node3D
{

	private Vector2 draggingPosition;
	private Vector2 currentPos;
	private bool rightMouseButtonIsPressed = false;
	private Node3D cameraHelper;

	public override void _Ready()
	{
		GD.Print("Udvozlet a 3D sakk jatekban!");

		List<Piece> table = new List<Piece>();

		TableController.tableGraphics = this;

		TableController.table = table;



		Queen queen2 = new Queen("D5", 0);

		Queen queen = new Queen("D1",1);

		//Queen queen2 = new Queen("C2", 1);

		//Queen queen3 = new Queen("G3", 1);

		//Queen queen4 = new Queen("E5", 1);

		//Queen queen5 = new Queen("G6", 1);


		//Rook rook = new Rook("F4", 0);

		Bishop bishop = new Bishop("D4", 0);

		King king = new King("E1", 1);

		Horse horse = new Horse("A2", 1);



		Pawn pawn = new Pawn("E3", 1);

	}

	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent _event)
	{

	
		base._Input(_event);

		if (_event is InputEventMouseButton)
		{
			InputEventMouseButton mouse = (InputEventMouseButton)_event;


			if (mouse.ButtonIndex == MouseButton.Right)
			{
				draggingPosition = mouse.Position;
				rightMouseButtonIsPressed = !rightMouseButtonIsPressed;

				if (rightMouseButtonIsPressed)
				{
					GD.Print("Eger locked! " + draggingPosition);
				}
				else
				{
					GD.Print("Eger unlocked!");
				}


			}
		

		} else if (_event is InputEventMouseMotion && rightMouseButtonIsPressed) {

			InputEventMouseMotion mouse = (InputEventMouseMotion)_event;


			cameraHelper = GetNode<Node3D>("camera");

			//GD.Print("Eger mozgatas! " + mouse.Position);


			currentPos = mouse.Position;


			Vector2 distance = draggingPosition - currentPos;



			if (Math.Abs(distance.X) > 2)
			{
				cameraHelper.RotateY(0.02f * (distance.X > 0 ? 1 : -1));
				//cameraHelper.RotateObjectLocal(new Vector3(0, 1, 0), 0.02f * (distance.X > 0 ? 1 : -1));
				draggingPosition = currentPos;
			}




			if (Math.Abs(distance.Y) > 2)
			{



				cameraHelper.RotateObjectLocal(new Vector3(0, 0, 1), 0.02f * (distance.Y > 0 ? 1 : -1));
				//cameraHelper.RotateX(0.02f * (distance.Y > 0 ? 1 : -1));
				draggingPosition = currentPos;
			}

		



		}


	}

}


