using Godot;
using System;
using test.Controllers;
using test.Pieces;

public partial class Dummy : MeshInstance3D
{


	private bool figureLocked = false;
	private MeshInstance3D piece;
	private StandardMaterial3D standardMaterial = new StandardMaterial3D();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("LOADED" + Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_static_body_3d_mouse_entered()
	{
		//GD.Print("You have entered the ccube!!!");
	}




	public override void _Input(InputEvent _event)
	{
		if(_event is InputEventMouseButton && figureLocked)
		{
			InputEventMouseButton mouse = (InputEventMouseButton)_event;




			if (mouse.ButtonIndex == MouseButton.Left)
			{
				figureLocked = false;


				var c = validMove.current;

				AvailableMove target = validMove.static_target;


				if (c != null && IsInstanceValid(c) && !c.IsQueuedForDeletion())
				{

					TableController.current.Move(c.Position, target);


					if (target != null && target.attack) { target.target.Delete(); };


				}




				Piece p = TableController.find(this);

				if (p == null) { return; }

					if (p.team == 1)
					{
						setColorWhite();
					}
					else
					{
						setColorBlack();
					}

					p.DeleteVisualizers();

	


				//QueueFree();
			}

		}

	}




	private void _on_static_body_3d_input_event(Node camera, InputEvent _event, Vector3 position, Vector3 normal, long shape_idx)
	{
		

		if(_event is InputEventMouseButton)
		{

			InputEventMouseButton mouse = (InputEventMouseButton)_event;


			if ( mouse.ButtonIndex == MouseButton.Left && !mouse.IsReleased() )
			{
				piece = GetNode<MeshInstance3D>("../"+Name);


				Piece p_del = TableController.find(this);

				p_del.DeleteVisualizers();


				standardMaterial.AlbedoColor = new Color(0.0f, 1.0f, 0.0f);
				piece.MaterialOverlay = standardMaterial;


				Piece p = TableController.find(this);

				p.ShowValidMoves();



				figureLocked = true;


			}
			

		}




	}


	public void setColorWhite()
	{

		var piece = GetNode<MeshInstance3D>("../" + Name);

		StandardMaterial3D material = new StandardMaterial3D();

		material.AlbedoColor = new Color(236/255f, 241/255f, 230/255f, 0.8f);
		piece.MaterialOverlay = material;


	}

	public void setColorBlack()
	{

		var piece = GetNode<MeshInstance3D>("../" + Name);

		StandardMaterial3D material = new StandardMaterial3D();

		material.AlbedoColor = new Color(86/255f, 80/255f, 81/255f,0.45f);
		material.Metallic = 0.1f;
		material.MetallicSpecular = 1.0f;

		piece.MaterialOverlay = material;


	}







}






