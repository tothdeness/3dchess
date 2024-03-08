using Godot;
using System;

public partial class validMove : MeshInstance3D
{

	public static MeshInstance3D current = null;

	// Called when the node enters the scene tree for the first time.
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
		GD.Print(this.Position);
	}


	private void _on_static_body_3d_mouse_exited()
	{
		current = null;
		GD.Print("RESET");
	}




}

