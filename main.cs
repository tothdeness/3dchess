using Godot;
using System;
using System.IO;
using System.Runtime.CompilerServices;

public partial class main : Node3D
{

	private Vector2 draggingPosition;
	private Vector2 currentPos;
	private bool rightMouseButtonIsPressed = false;
	private Node3D cameraHelper;

	public override void _Ready()
	{
		GD.Print("Udvozlet a 3D sakk jatekban!");

		PackedScene scene = GD.Load<PackedScene>("res://mesh.tscn");
		Node inst = scene.Instantiate();
		AddChild(inst);


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


			cameraHelper = GetNode<Node3D>("Node3D");

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



				cameraHelper.RotateObjectLocal(new Vector3(1, 0, 0), 0.02f * (distance.Y > 0 ? 1 : -1));
				//cameraHelper.RotateX(0.02f * (distance.Y > 0 ? 1 : -1));
				draggingPosition = currentPos;
			}

		



		}


	}

}


