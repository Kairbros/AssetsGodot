using Godot;
public partial class ControladorGenerico3raPersona : Jugador
{
    float velocidadBase = 7;
    float FuerzaSaltoBase = 10;
    float FuerzaGravedadBase = 45;
    float FriccionBase = 10;
    
    bool SePuedeDobleSalto;
    bool DobleSalto;

    bool SePuedeDash;
    bool Dash;
    int DashMultiplicador = 5;

    Vector3 DirDash = new Vector3(0,0,1);

    float TemporizadorNado;
    bool Nadando;



    public override void _Ready()
    {
        base._Ready();
        SetVelocidad(velocidadBase);
        SetGravedad(FuerzaGravedadBase);
        SetFriccion(FriccionBase);
        SetFuerzaSalto(FuerzaSaltoBase);
        SetLugar("FueraAgua");
    }
    public override void _PhysicsProcess(double delta)
    {
        MovimientoControl(delta);
        SprintControl();
        SaltoControl();
        DashControl();
        FriccionGravedadControl();
        BajoAguaControl(delta);

        MoveAndSlide();

        CamaraControl();
        RotacionDireccionControl(delta);
        AnimacionControl();
        EmpujeControl();
        ColisionControl();
        base._PhysicsProcess(delta);
    }
    
    private void CamaraControl()
    {
        CamaraPos();
    }
    private void RotacionDireccionControl(double delta)
    {
        Vector3 DirMov = new Vector3(Input.GetAxis("a","d"),0,Input.GetAxis("w","s"));
        DirMov = DirMov.Rotated(Vector3.Up,GetCamara().Rotation.Y).Normalized();
        Rotacion(delta);
        SetDirMov(DirMov); 
    }
    private async void AnimacionControl()
    {
        AnimationTree Animacion = GetNode<AnimationTree>("Animacion");
        if (!Nadando)
        {
            if (!Dash)
            {
                if (IsOnFloor())
                {
                    if (GetDirMov() != Vector3.Zero)
                    {
                        if(GetVelocidad() > velocidadBase)
                        {
                            Animacion.Set("parameters/Transition/transition_request","Run");
                        }
                        else
                        {
                            Animacion.Set("parameters/Transition/transition_request","Walk");
                        }
                    }
                    else
                    {
                        Animacion.Set("parameters/Transition/transition_request","Idle");
                    }
                }
                else
                {
                    if (DobleSalto)
                    {
                        Animacion.Set("parameters/Transition/transition_request","Flip");
                        await ToSignal(Animacion,"animation_finished");
                        DobleSalto = false;
                    }
                    else
                    {
                        Animacion.Set("parameters/Transition/transition_request","Fall");
                    }
                }
            }        
            else
            {
                Animacion.Set("parameters/Transition/transition_request","Dash");
            }
        }
        else
        {
            Animacion.Set("parameters/Transition/transition_request","Dash");
        }

    }
    private void FriccionGravedadControl()
    {
        
        KinematicCollision3D Colision = GetLastSlideCollision(); 
        // Agarra la colision de lo que toca
        // Verifica que no sea Nulo
        SetGravedad(GetLugar() != "Agua" ? FuerzaGravedadBase : FuerzaGravedadBase * 0.05f);
        Node Colisionador = GetLugar() == "Agua" ? null : Colision != null ? Colision.GetCollider() as Node : null;
        SetFriccion( GetLugar() == "Agua"? FriccionBase * 0.3f 
        : Colisionador == null ? FriccionBase * 0.6f 
        : Colisionador.IsInGroup("Hielo") ? FriccionBase * 0.1f 
        : Colisionador.IsInGroup("Pasto") ? FriccionBase * 0.9f 
        : Colisionador.IsInGroup("Arena") ? FriccionBase * 1.1f 
        : FriccionBase);      
    }
      
    private void MovimientoControl(double delta)
    {
        Movimientos(delta);
    }
    private void SprintControl()
    {
        SetVelocidad(!Nadando ? 
        (!Dash ? 
        (Input.IsActionPressed("shift") ? velocidadBase * 1.5f : velocidadBase) 
        : velocidadBase * 0.5f) 
        : velocidadBase * 0.9f);
    }
    private void SaltoControl() 
    {
        DobleSalto = IsOnFloor() && DobleSalto ? false : DobleSalto;
        if (!Nadando)
        {    
            if (IsOnFloor())
            {
                SePuedeDobleSalto = Input.IsActionJustPressed("space");
                if (SePuedeDobleSalto)
                {
                    SetFuerzaSalto(FuerzaSaltoBase);
                    Salto();
                }
            }
            else if (SePuedeDobleSalto && GetLugar() != "Agua" && Input.IsActionJustPressed("space"))
            {
                SePuedeDobleSalto = false;
                DobleSalto = true;
                SetFuerzaSalto(FuerzaSaltoBase * 1.5f);
                Salto();
            }
        }
    }
    private void BajoAguaControl(double delta)
    {
        RayCast3D DetectorTerreno = GetNode<RayCast3D>("DetectorTerreno");

        TemporizadorNado = (GetLugar() == "Agua" && !IsOnFloor() && !DetectorTerreno.IsColliding()) ? Mathf.Max(0, TemporizadorNado - (float)delta)  : 0.4f;
        Nadando = TemporizadorNado <= 0 ? true : false;
        DetectorTerreno.Enabled = GetLugar() == "Agua" ? true : false;

        DobleSalto = Nadando ? false : DobleSalto;
        Dash = Nadando ? false : Dash;
        SePuedeDash = Nadando ? false : SePuedeDash;
        SePuedeDobleSalto = Nadando ? false : SePuedeDobleSalto;
        
        if (Nadando)
        {
            SetFuerzaSalto(Input.IsActionPressed("space") ? FuerzaSaltoBase * 0.5f : Input.IsActionPressed("shift") ? FuerzaSaltoBase * -0.9f : FuerzaSaltoBase);
            if (Input.IsActionPressed("space") || Input.IsActionPressed("shift"))
            {
                Salto();
            }
            if (Input.IsActionJustPressed("c"))
            {
                SetVelocidad(GetVelocidad() * DashMultiplicador);
                Desplazamiento(DirDash);
            }

        }
        
    }
    private void ColisionControl()
    {
        CollisionShape3D Colision = GetNode<CollisionShape3D>("Colision");
        Colision.RotationDegrees = Nadando ? new Vector3(90, 0, 0) : (Dash ? new Vector3(90,0,0) : new Vector3(0,0,0));
    }
    private void DashControl()
    {
        DirDash = GetDirMov() != Vector3.Zero ? GetDirMov() : DirDash;
        if (!Nadando)
        {
            Dash = Dash && Input.IsActionJustPressed("space") ? false : Dash;
            if (!Dash && IsOnFloor())
            {
                SePuedeDash = true;
            }
            if (SePuedeDash && Input.IsActionJustPressed("c"))
            {           
                SetVelocidad(GetVelocidad() * DashMultiplicador);
                Desplazamiento(DirDash);
                SePuedeDash = false;
                SePuedeDobleSalto = true;
                Dash = true;
            }
        }
    }
    private void EmpujeControl()
    {

        float FuerzaEmpuje;
        FuerzaEmpuje = !Dash ? 
        (GetDirMov() != Vector3.Zero ? 
        (GetVelocidad() > velocidadBase ? GetVelocidad() * 1.5f : GetVelocidad()) 
        : 0) 
        : (IsOnFloor() ? GetVelocidad() : velocidadBase * 500);

        if (GetSlideCollisionCount() > 0)
        {
            for (int i = 0; i < GetSlideCollisionCount(); i++) 
            {
                var Colisionador = GetSlideCollision(i);
                if (Colisionador.GetCollider() as RigidBody3D is RigidBody3D)
                {
                    Vector3 DirPush = ((Colisionador.GetCollider() as RigidBody3D).GlobalPosition - GlobalPosition ).Normalized();
                    (Colisionador.GetCollider() as RigidBody3D).ApplyCentralForce(DirPush * FuerzaEmpuje);
                }
            }    
        }
    }
}

