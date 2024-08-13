using Godot;

public partial class CamaraTerceraPersona : Node3D
{
    float Sensibilidad = 0.2f;
    Camera3D CamaraView;
    
    SpringArm3D Brazo;
    SphereShape3D ColisionSphere;
    CollisionShape3D Colision;

    int CamaraModo;

    public override void _Ready() 
    {
        Inicializacion();
    }

    public override void _Input(InputEvent @event)
    {
            if (@event is InputEventMouseMotion MousePos)
            {
                RotationDegrees = RotationDegrees.Clamp(new Vector3(-90,RotationDegrees.Y,0),new Vector3(30,RotationDegrees.Y,0));
                RotationDegrees -= new Vector3(MousePos.Relative.Y * Sensibilidad , MousePos.Relative.X * Sensibilidad, 0);
            }
            if (@event is InputEventMouseButton Wheel)
            {
                Brazo.SpringLength = Mathf.Clamp(Brazo.SpringLength,3,15);
                if (Wheel.ButtonIndex == MouseButton.WheelUp)
                {
                    Brazo.SpringLength -= 0.5f;
                }
                if (Wheel.ButtonIndex == MouseButton.WheelDown)
                {
                    Brazo.SpringLength += 0.5f;
                }
            }
    }
    public Camera3D GetCamara()
    {
        return CamaraView;
    }
    public Vector3 GetCamaraRayDir()
    {
        Vector2 MousePos = GetViewport().GetMousePosition();
        Vector3 RayOrigin = CamaraView.ProjectRayOrigin(MousePos);
        Vector3 RayNormal = CamaraView.ProjectRayNormal(MousePos) * 50;
        Vector3 MouseDir = (RayNormal - RayOrigin).Normalized();
        return MouseDir;
    }

    private void Inicializacion()
    {
        CamaraModo = 0;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        TopLevel = true;
        CamaraView = new Camera3D();
        ColisionSphere = new SphereShape3D()
        {
            Radius = 0.5f
        };
        Brazo = new SpringArm3D()
        {
            Position = new Vector3(0,0.5f,0),
            Shape = ColisionSphere,
            SpringLength = 10,
            CollisionMask = 4
        };
        Colision = new CollisionShape3D()
        {
            Shape = ColisionSphere
        };
        AddChild(CamaraView);
        AddChild(Brazo);
        CamaraView.Reparent(Brazo);
    }
}
