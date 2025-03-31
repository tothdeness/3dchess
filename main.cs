using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using test.core.Controllers;
using test.core.Logging;
using test.core.Mode;
using test.core.Network;
using test.core.Pieces;
using static Godot.Control;

public partial class main : Node3D
{
	private Vector2 draggingPosition;
	private Vector2 currentPos;
	private bool rightMouseButtonIsPressed = false;
	private Node3D cameraHelper;
	private GameController game;

	private Button elfogad;
	private Button elutasit;

	private VBoxContainer container;

	private Label response;

	private bool pressed;


	[Signal]
	public delegate void FullyInitializedEventHandler();
	private bool _isFullyInitialized = false;

	public override void _Ready()
	{
		GD.Print("main _Ready: Start");
		cameraHelper = GetNode<Node3D>("camera");


		elfogad = GetNode<Button>("UI/VBoxContainer/Elfogad");
		elutasit = GetNode<Button>("UI/VBoxContainer/Elutasit");
		container = GetNode<VBoxContainer>("UI/VBoxContainer");
		response = GetNode<Label>("UI/response");



		elfogad.Pressed += accepted;
		elutasit.Pressed += declined;

		container.Hide();


		GD.Print("main _Ready: Finished setup. Emitting FullyInitialized.");
		_isFullyInitialized = true;
		EmitSignal(SignalName.FullyInitialized);

	}

	public bool IsFullyInitialized() => _isFullyInitialized;

	public void ShowTakeBackPopup()
	{
		container.Show();
	}

	public async void ResponseAccept()
	{
		response.Text = "Elfogadva!";
		response.Visible = true;
		await ToSignal(GetTree().CreateTimer(3f), "timeout");  // Wait 3 seconds
		response.Visible = false;

	}

	public async void ResponseDeclined()
	{
		response.Text = "Elutasitva!";
		response.Visible = true;
		await ToSignal(GetTree().CreateTimer(3f), "timeout");  // Wait 3 seconds
		response.Visible = false;

	}



	private void accepted() { OnPopupItemSelected(0); }

	private void declined() { OnPopupItemSelected(1); }

	// Handle popup selection
	private void OnPopupItemSelected(long id)
	{
		container.Hide(); // Hide the popup after selection
		if (id == 0) // Accept
		{
			GD.Print("Elfogadva");
			game.AcceptTakeBack();
		}
		else if (id == 1) // Decline
		{
			GD.Print("Elutasitva");
			game.DeclineTakeBack();
		}
	}



	public void StartNewBotGame(int depth, int team)
	{
		TableController.table.Clear();
		string playerTeam = team == -1 ? "Black" : "White"; // Map team to string
		var gameID = CreateGameFile.CreateNewGameFile("Bot", playerTeam, depth);
		game = GameController.CreateAndStartGame(true, depth, this, team, null, 1,gameID);
				
	}

	public void LoadGame(string gameID)
	{
		TableController.table.Clear();
		var data = GameFileReader.GetGameMetadata(gameID);

		GD.Print(data["GameType"]);

		game = GameController.LoadAndStartGame(data["GameType"] == "Bot", int.Parse(data["BotDepth"]), this, data["PlayerTeam"] == "White" ? 1 : -1, null, data["GameType"] == "Bot" ? 1 : 0, gameID);


	}


	public void StartNewPvpGame()
	{
		TableController.table.Clear();
		var gameID = CreateGameFile.CreateNewGameFile("PvP");
		game = GameController.CreateAndStartGame(false, 0, this, 1, null, 0, gameID);
		
	}

	private void OnConnectionEstablished(TcpConnect client)
	{
		GD.Print("Creating new game after connection...");
		var gameID = CreateGameFile.CreateNewGameFile("PvP");
		game = GameController.CreateAndStartGame(false, 0, this, 1, client, 2, gameID);
	}

	public async void CreateNewLanGame(string port)
	{
		GD.Print(port);
		TableController.table.Clear();
		TcpConnect server = new TcpConnect("0",int.Parse(port));
		server.OnConnectionEstablished += OnConnectionEstablished;
		await server.CreateTcpListener();
	}

	public void ConnectLanGame(string ip,string port)
	{
		TableController.table.Clear();
		TcpConnect server = new TcpConnect(ip, int.Parse(port));
		var gameID = CreateGameFile.CreateNewGameFile("PvP");
		game = GameController.CreateAndStartGame(false, 0, this, -1, server, 2, gameID);
		server.ConnectToPeer();
	}



	public void SetMesh()
	{

		TableController.tableGraphics = this;

		foreach(var item in TableController.table)
		{
			item.Value.AddVisuals();
		}

	}


	//1 white
	//-1 black
	public void SetCameraPosition(int position)
	{

		if (position == -1)
		{
			cameraHelper.RotateY((float)3.14159);
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
			InputEventMouseMotion mouse = (InputEventMouseMotion) _event;

			float rotatingSpeed = 0.022f;

			currentPos = mouse.Position;

			Vector2 distance = draggingPosition - currentPos;

			if (Math.Abs(distance.X) > 2)
			{
				cameraHelper.RotateY(rotatingSpeed * (distance.X > 0 ? 1 : -1));
				draggingPosition = currentPos;
			}

			if (Math.Abs(distance.Y) > 2)
			{
				cameraHelper.RotateObjectLocal(new Vector3(0, 0, 1), rotatingSpeed * (distance.Y > 0 ? 1 : -1));
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
			LoadNewScene("res://TSCN/GUI/menu.tscn");
		}

		if (_event is InputEventKey keyEvent2 && keyEvent2.Pressed && keyEvent2.Keycode == Key.B)
		{
			if(game.gameMode == 2 )

			{

				if (!game.takeBackPending)
				{
					game.RequestTakeBack();
				}
				
			
			
			}
			
			
			else
			{



				game.MoveBack();
			}
		}

	}

	private void LoadNewScene(string scenePath)
	{
		//TableController.table.Clear();
		var sceneSwitcher = GetNode("/root/SceneSwitcher");
		sceneSwitcher.Call("switch_scene", scenePath);

	}

}
