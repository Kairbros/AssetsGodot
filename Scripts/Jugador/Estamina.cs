public partial class Nyla : Jugador
{
    bool TieneEstamina;
    bool RecargandoEstamina;
    private void EstaminaControl(double delta)
    {
        TextureProgressBar Estamina = GetNode<TextureProgressBar>("Info/BarraEstamina");
        Estamina.Value += GetDirMov() == Vector3.Zero ? (float)delta * 50 : RecargandoEstamina ? (float)delta * 35 : Input.IsActionPressed("shift") ? -(float)delta * 50 : (float)delta * 50;
        TieneEstamina = !RecargandoEstamina && Estamina.Value >= Estamina.MinValue; 
        RecargandoEstamina =  RecargandoEstamina == true ? Estamina.Value < Estamina.MaxValue && RecargandoEstamina : Estamina.Value <= Estamina.MinValue || RecargandoEstamina;
        Estamina.TintProgress = RecargandoEstamina ? new Color("ff0000") : new Color("00ffff");
        Estamina.Visible = Estamina.Value >= Estamina.MaxValue ? false : true;    
    }
}