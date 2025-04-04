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

	private Vector2 rotationInput = Vector2.Zero;



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

	private void CleanupPreviousGame()
	{
		GD.Print("Checking for existing game controller to clean up...");
		if (game != null)
		{
			GD.Print("Found existing game controller. Cleaning up...");
			game.Cleanup();
			game = null; // Release the reference
		}
		else
		{
			GD.Print("No existing game controller found.");
		}

		// Optional: Add any other scene-specific cleanup needed here
		// e.g., removing leftover visual nodes not tied to pieces, resetting UI elements.
		// Since TableController.table.Clear() is called anyway, maybe not much else needed here.
	}

	public void StartNewBotGame(int depth, int team)
	{
		CleanupPreviousGame();
		TableController.table.Clear();
		string playerTeam = team == -1 ? "Black" : "White"; // Map team to string
		var gameID = CreateGameFile.CreateNewGameFile("Bot", playerTeam, depth);
		game = GameController.CreateAndStartGame(true, depth, this, team, null, 1,gameID);
				
	}

	public void LoadGame(string gameID)
	{
		CleanupPreviousGame();
		TableController.table.Clear();
		var data = GameFileReader.GetGameMetadata(gameID);

		GD.Print(data["GameType"]);

		game = GameController.LoadAndStartGame(data["GameType"] == "Bot", data["GameType"] == "Bot" ? int.Parse(data["BotDepth"]) : 0, this, data["PlayerTeam"] == "White" ? 1 : -1, null, data["GameType"] == "Bot" ? 1 : 0, gameID);


	}


	public void StartNewPvpGame()
	{
		CleanupPreviousGame();
		TableController.table.Clear();
		var gameID = CreateGameFile.CreateNewGameFile("PvP");
		game = GameController.CreateAndStartGame(false, 0, this, 1, null, 0, gameID);
		
	}

	private void OnConnectionEstablished(TcpConnect client)
	{
		CleanupPreviousGame();
		GD.Print("Creating new game after connection...");
		var gameID = CreateGameFile.CreateNewGameFile("LAN","White");
		game = GameController.CreateAndStartGame(false, 0, this, 1, client, 2, gameID);
	}

	public async void CreateNewLanGame(string port)
	{
		CleanupPreviousGame();
		GD.Print(port);
		TableController.table.Clear();
		TcpConnect server = new TcpConnect("0",int.Parse(port));
		server.OnConnectionEstablished += OnConnectionEstablished;
		await server.CreateTcpListener();
	}

	public async void LoadNewLanGame(string port, string gameID)
	{
		CleanupPreviousGame();
		TableController.table.Clear();
		TcpConnect server = new TcpConnect("0", int.Parse(port));

		var data = GameFileReader.GetGameMetadata(gameID);

		server.OnConnectionEstablished += (client) => {
			GD.Print("Load game after connection...");
			game = GameController.LoadAndStartGame(data["GameType"] == "Bot", data["GameType"] == "Bot" ? int.Parse(data["BotDepth"]) : 0, this, data["PlayerTeam"] == "White" ? 1 : -1, server, 2, gameID);
		}; 


		await server.CreateTcpListener();

	}

	public void ConnectLoadLanGame(string ip,string port, string gameID)
	{


		CleanupPreviousGame();
		TableController.table.Clear();
		TcpConnect server = new TcpConnect(ip, int.Parse(port));
		var data = GameFileReader.GetGameMetadata(gameID);
		game = GameController.LoadAndStartGame(data["GameType"] == "Bot", data["GameType"] == "Bot" ? int.Parse(data["BotDepth"]) : 0, this, data["PlayerTeam"] == "White" ? 1 : -1, server, 2, gameID);
		server.ConnectToPeer();

	}


	public void ConnectLanGame(string ip,string port)
	{
		CleanupPreviousGame();
		TableController.table.Clear();
		TcpConnect server = new TcpConnect(ip, int.Parse(port));
		var gameID = CreateGameFile.CreateNewGameFile("LAN","Black");
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
		if (rightMouseButtonIsPressed && rotationInput != Vector2.Zero)
		{
			float sensitivity = 0.005f;
			float yaw = -rotationInput.X * sensitivity;
			float pitch = -rotationInput.Y * sensitivity; // Negative Y for up/down

			// --- YAW ---
			// Rotates the helper around the WORLD'S Y axis (Up)
			cameraHelper.RotateY(yaw);

			// --- PITCH ---
			// Rotates the helper around ITS OWN LOCAL X axis (Right)
			// This should make the camera attached to it tilt up/down.
			cameraHelper.RotateObjectLocal(new Vector3(0,0,1), pitch); // Use this!

			// --- Optional Pitch Clamping ---
			Vector3 currentRotation = cameraHelper.Rotation;
			currentRotation.X = Mathf.Clamp(currentRotation.X, Mathf.DegToRad(-85.0f), Mathf.DegToRad(85.0f));
			cameraHelper.Rotation = currentRotation;

			rotationInput = Vector2.Zero;
		}
	}

	public void add_child(Dummy child)
	{
		AddChild(child);
	}

	public override void _Input(InputEvent _event)
	{
		base._Input(_event);
		if (_event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Right)
			{
				rightMouseButtonIsPressed = mouseButtonEvent.Pressed;
				// Optional: Reset rotation input when button is released to prevent
				// lingering movement if _Process runs slightly after release.
				if (!rightMouseButtonIsPressed)
				{
					rotationInput = Vector2.Zero;
				}
				// Optional: Capture mouse mode to hide cursor and keep it centered
				Input.MouseMode = rightMouseButtonIsPressed ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
			}
		}
		// Accumulate Mouse Motion if Right Button is Held
		else if (_event is InputEventMouseMotion mouseMotionEvent && rightMouseButtonIsPressed)
		{
			// Accumulate relative movement. This is the key change.
			rotationInput += mouseMotionEvent.Relative;
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
		if(game != null && game.gameMode == 2)
		{
			TableController.table.Clear();
			CleanupPreviousGame();
		}
		var sceneSwitcher = GetNode("/root/SceneSwitcher");
		sceneSwitcher.Call("switch_scene", scenePath);

	}

}
