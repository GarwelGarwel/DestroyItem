using Verse;

namespace DestroyItem
{
    public class MapComponent_DestroyItem : MapComponent
    {
        public MapComponent_DestroyItem(Map map)
            : base(map)
        { }

        public override void MapComponentOnGUI() => Utility.RegisterDesignator<Designator_DestroyItem>();
    }
}
