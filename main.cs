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
	private GameController game;
	private bool pressed;

	public override void _Ready()
	{
		GD.Print("Udvozlet a 3D sakk jatekban!");
	}

	public void StartNewBotGame(int depth,int team)
	{
		TableController.table.Clear();
		GameController new_game = new GameController(true, depth, this, team);
	}

	public void StartNewPvpGame()
	{
		TableController.table.Clear();
		GameController new_game = new GameController(false,0,this,1);
	}



	public void SetMesh()
	{

		TableController.tableGraphics = this;

		foreach(KeyValuePair<string,Piece> item in TableController.table)
		{
			item.Value.SetMesh();
		}

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
		}
		else if (_event is InputEventMouseMotion && rightMouseButtonIsPressed)
		{
			InputEventMouseMotion mouse = (InputEventMouseMotion)_event;

			cameraHelper = GetNode<Node3D>("camera");

			// GD.Print("Eger mozgatas! " + mouse.Position);

			currentPos = mouse.Position;

			Vector2 distance = draggingPosition - currentPos;

			if (Math.Abs(distance.X) > 2)
			{
				cameraHelper.RotateY(0.02f * (distance.X > 0 ? 1 : -1));
				// cameraHelper.RotateObjectLocal(new Vector3(0, 1, 0), 0.02f * (distance.X > 0 ? 1 : -1));
				draggingPosition = currentPos;
			}

			if (Math.Abs(distance.Y) > 2)
			{
				cameraHelper.RotateObjectLocal(new Vector3(0, 0, 1), 0.02f * (distance.Y > 0 ? 1 : -1));
				// cameraHelper.RotateX(0.02f * (distance.Y > 0 ? 1 : -1));
				draggingPosition = currentPos;
			}
		}
		if (_event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
		{
			pressed = true;
		}

		if(pressed && _event is InputEventKey key && key.IsReleased())
		{
			pressed = false;
			LoadNewScene("res://TSCN/menu.tscn");
		}

	}

	private void LoadNewScene(string scenePath)
	{
		//TableController.table.Clear();
		var sceneSwitcher = GetNode("/root/SceneSwitcher");
		sceneSwitcher.Call("switch_scene", scenePath);

	}

}
