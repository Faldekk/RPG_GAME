using RPG_GAME.App;
using RPG_GAME.Model;
using RPG_GAME.UI;

const int requiredWidth = World.Width + 50;
const int requiredHeight = 36;

ConsoleHost.Initialize(requiredWidth, requiredHeight);

try
{
    new Game().Run();
}
finally
{
    ConsoleHost.Shutdown();
}
