namespace RPG_GAME.Model
{
    public class Player
    {
        public PlayerStats Stats { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public Vec2 Pos { get; private set; }
        public bool IsAlive => Stats.IsAlive;

        public Player(Vec2 startPosition)
        {
            Pos = startPosition;
            Stats = new PlayerStats();
            Inventory = new PlayerInventory();
        }
        public void MoveTo(Vec2 newPos)
        {
            Pos = newPos;
        }
        public void SwapWeapons()
        {
            var temp = Inventory.LeftHand;

            if (Inventory.LeftHand != null)
                Inventory.UnequipItem(0);

            if (Inventory.RightHand != null)
            {
                var rightWeapon = Inventory.UnequipItem(1);
                if (rightWeapon != null)
                    Inventory.EquipItem(rightWeapon, 0);
            }

            if (temp != null)
                Inventory.EquipItem(temp, 1);
        }

        public void Heal(int amount)
        {
            Stats.Heal(amount);
        }

        public override string ToString()
        {
            return $"Player at {Pos} - HP: {Stats.Health}/{Stats.MaxHealth}, Coins: {Stats.Coins}";
        }
    }
}