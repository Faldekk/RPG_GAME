namespace RPG_GAME.Model
{
    public static class WorldAccessor
    {
        public static Tile[,] Tiles => World.TilesForServices;
    }
}
