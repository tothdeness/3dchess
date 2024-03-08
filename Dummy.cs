using Godot;
using System;

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

			if(mouse.ButtonIndex == MouseButton.Left)
			{
				figureLocked = false;


				piece = GetNode<MeshInstance3D>("../"+Name);

				standardMaterial.AlbedoColor = new Color(1.0f, 1.0f, 1.0f);
				piece.MaterialOverlay = standardMaterial;




				GD.Print("Figura unlocked!");
				
			

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



				standardMaterial.AlbedoColor = new Color(0.0f, 1.0f, 0.0f);
				piece.MaterialOverlay = standardMaterial;

				
				


				figureLocked = true;

				GD.Print("Figura locked!");
			}
			

		}




	}


}



