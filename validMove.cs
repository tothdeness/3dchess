using Godot;
using System;
using test.Pieces.Resources;

public partial class validMove : MeshInstance3D
{



	public static MeshInstance3D current = null;

	public static AvailableMove static_target;

	public AvailableMove target;




	// Called  when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	private void _on_static_body_3d_mouse_entered()
	{
		current = this;

		static_target = target;
	}


	private void _on_static_body_3d_mouse_exited()
	{
		current = null;

		static_target = null;

	}




}

