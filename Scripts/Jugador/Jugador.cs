using System;
using Godot;


public partial class Jugador : CharacterBody3D
{
    float Velocidad;
    float FuerzaSalto;
    float Friccion;
    float FuerzaGravedad;
    String Lugar;

    Vector3 DirMov;

    [Export] public PackedScene Camara;

    public override void _PhysicsProcess(double delta)
    {
        LimitarVariables();   
    }

    private void LimitarVariables()
    {
        Velocity = new Vector3(Velocity.X,Mathf.Clamp(Velocity.Y,-FuerzaGravedad*2,FuerzaGravedad*10),Velocity.Z);
        //Limites de toda la velocidad a la que pueda llegar el personaje si no esta en el piso
    }

    public void SetFriccion(float newFriccion)
    {
        Friccion = newFriccion;
    }   
    public void SetVelocidad(float newVelocidad)
    {
        Velocidad = newVelocidad;
    }
    public void SetFuerzaSalto(float newFuerzaSalto)
    {
        FuerzaSalto = newFuerzaSalto;
    }
    public void SetGravedad(float newFuerzaGravedad)
    {
        FuerzaGravedad = newFuerzaGravedad;
    }
    public void SetDirMov(Vector3 newDirMov)
    {
        DirMov = newDirMov;
    }
    public void SetLugar(String newLugar)
    {
        Lugar = newLugar;
    }

    public float GetFriccion()
    {
        return Friccion;
    }
    public String GetLugar()
    {
        return Lugar;
    }
    public float GetFuerzaGravedad()
    {
        return FuerzaGravedad;
    }
    public float GetVelocidad()
    {
        return Velocidad;
    }
    public Vector3 GetDirMov()
    {
        return DirMov;
    } 
    public Node3D GetCamara()
    {
        return Camara;
    }
    public float GetFuerzaSalto()
    {
        return FuerzaSalto;
    }

    public void Desplazamiento(Vector3 DirDash)
    {
        // Da un breve impulso en el eje horizontal
        Velocity = new Vector3(Velocidad * DirDash.X , Velocity.Y, Velocidad * DirDash.Z);
    }
    public void Salto()
    {
        // Da un breve impulso en el eje verical
        Velocity = new Vector3(Velocity.X,FuerzaSalto,Velocity.Z);
    }
    public void CamaraPos()
    {
        Camara.Position = Position;
    }
    public void Movimientos(double delta)
    {
        Velocity = new Vector3(Mathf.Lerp(Velocity.X, Velocidad * DirMov.X, Friccion * (float)delta), Velocity.Y, Mathf.Lerp(Velocity.Z, Velocidad * DirMov.Z, Friccion * (float)delta));
        // Esta velocidad funciona en medida de la interpolacion de un Float hacia el numero que se le asigne en medida de un tercer parametro
        // Se mueve y se frena dependiendo si "DirMov" es != (0,0,0) o == (0,0,0)
        // Dato: Velocity de esta manera nunca llega a (0,0,0) debido a que la interpolacion no es precisa, quedando en ocaciones por ejemplo:
        // (-45E-45,0,-45E-45) Este numero equivaliendo a −0.000000000000000000000000000000000000000000045 Aproximadamente, no siendo capaz de mover el personaje pero si conservar la rotacion

        if (!IsOnFloor())
        {
            Velocity -= new Vector3(0, FuerzaGravedad * (float)delta, 0);
            // Gravedad si no esta en el piso
        }  
    }
    public void Rotacion(double delta)
    {  
        // La rotacion es controlada directamente por el parametro de velocidad "Velocity" 
        // este crea un angulo entre la velocidad en el eje X y Z siempre y cuando estos no sean 0 para posteriormente interpolar el angulo entre el eje de rotacion Y hacia el angulo de el movimiento de velocidad X y Z
        if (Velocity.X != 0 || Velocity.Z != 0)
        {
            Vector2 AngleMov = new Vector2(Velocity.Z,Velocity.X);
            Rotation = new Vector3(0,Mathf.LerpAngle(Rotation.Y,AngleMov.Angle(),15*(float)delta),0);  
        }
    }
}
