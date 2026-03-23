namespace RPG_GAME.Model.DungeonBuilding
{
    public sealed class BuildInstruction
    {
       
        public string Key { get; }  
        public string Description { get; }  

        public BuildInstruction(string key, string description)
        {
            Key = key;
            Description = description;
        }
        public string ToDisplayText()
        {
            return $"[{Key}] {Description}";
        }
    }
}
