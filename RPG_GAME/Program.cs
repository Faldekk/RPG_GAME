using RPG_GAME.App;
using RPG_GAME.App.Configuration;
using RPG_GAME.Model;
using RPG_GAME.UI;

const int requiredWidth = World.Width + 50;
const int requiredHeight = 36;

var config = ConfigLoader.Load(Path.Combine(AppContext.BaseDirectory, "gameconfig.json"));

ConsoleHost.Initialize(requiredWidth, requiredHeight);

try
{
    new Game(config).Run();
}
finally
{
    ConsoleHost.Shutdown();
}
