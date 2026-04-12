using System.Collections.Generic;
using System.Linq;
using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class BuildContext
    {
        private readonly List<BuildInstruction> _instructions = new();     
        private readonly HashSet<string> _features = new(); 

        public List<RectRoom> Rooms { get; } = new();  
        public RectRoom? CentralRoom { get; set; }  

        public IReadOnlyList<BuildInstruction> Instructions => _instructions;  
        public IReadOnlyCollection<string> Features => _features;  
        public void AddFeature(string feature)
        {
            _features.Add(feature);
        }
        public bool HasFeature(string feature)
        {
            return _features.Contains(feature);
        }

        public void AddInstruction(string key, string description)
        {
            _instructions.Add(new BuildInstruction(key, description));
        }
<<<<<<< HEAD
=======

>>>>>>> c96ec9f57eb768118669ed627b482fd00ba86918
        
        public void AddInstruction(BuildInstruction instruction)
        {
            _instructions.Add(instruction);
        }
    }
}