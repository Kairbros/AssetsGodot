	using Godot;
using System;

public partial class Agua : Area3D
{ 
    private void OnAguaEnter(Node3D body)
	{
		if (body is Jugador)
		{
			body.Call("SetLugar", "Agua");
		}
		if (body is RigidBody3D)
		{
			body.Set("gravity_scale", 1* 0.03);
		}
	}
	private void OnAguaExit(Node3D body)
	{
		if (body is Jugador)
		{	
			body.Call("SetLugar", "FueraAgua");
		}
			if (body is RigidBody3D)
		{
			body.Set("gravity_scale", 1);
		}
	}
}
