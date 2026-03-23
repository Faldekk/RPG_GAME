namespace RPG_GAME.Model.DungeonBuilding
{
  
    public interface IDungeonStrategy
    {
        BuildContext Build(Tile[,] tiles, int width, int height);
    }
}