using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using test.Controllers;
using test.Mode;
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

		GameController new_game = new GameController(true, 5 , this, -1);

	}

	public override void _Process(double delta)
	{
	}


	public void add_child(Dummy child)
	{
		AddChild(child);
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


