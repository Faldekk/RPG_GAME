namespace RPG_GAME.Model.Instructions
{
    public class InstructionEntry
    {
        public string KeyHint { get; }
        public string Description { get; }

        public InstructionEntry(string keyHint, string description)
        {
            KeyHint = keyHint;
            Description = description;
        }
    }
}