using System.Collections.Generic;
using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class BuildContext
    {
        public List<RectRoom> Rooms { get; } = new();
        public RectRoom? CentralRoom { get; set; }
    }
}