namespace TownOfSushi.Modules.ColorsMod
{
    public class ColorBehaviour : MonoBehaviour
    {
        public Renderer Renderer;
        public int Id;

        public void AddRend(Renderer rend, int id)
        {
            Renderer = rend;
            Id = id;
        }

        public void Update()
        {
            if (Renderer == null) return;

            if (ColorUtils.IsRainbow(Id))
                ColorUtils.SetRainbow(Renderer);

            else if (ColorUtils.IsMonochrome(Id))
                ColorUtils.SetMonochrome(Renderer);
                
            else if (ColorUtils.IsGalaxy(Id))
                ColorUtils.SetGalaxy(Renderer);
        }

        public ColorBehaviour(IntPtr ptr) : base(ptr) { }
    }
}
