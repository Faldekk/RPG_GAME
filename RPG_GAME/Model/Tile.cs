using RPG_GAME.Model;

public class Tile
{
    public bool IsWall { get; set; }
    public Items? Item { get; set; }

    public bool HasItem => Item != null;
    public Tile(bool iswall)
    {
        IsWall = iswall;            
    }
}
