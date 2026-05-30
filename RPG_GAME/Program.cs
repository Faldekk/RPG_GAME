using RPG_GAME.App;
using RPG_GAME.App.Configuration;
using RPG_GAME.App.Startup;
using RPG_GAME.Model;
using RPG_GAME.UI;

const int requiredWidth = World.Width + 50;
const int requiredHeight = 36;

var config = ConfigLoader.Load(Path.Combine(AppContext.BaseDirectory, "gameconfig.json"));
var options = StartupParser.Parse(args);

ConsoleHost.Initialize(requiredWidth, requiredHeight);

try
{
    if (options.IsServer || options.IsClient)
    {
        await StartupRunner.RunAsync(options, config);
    }
    else
    {
        new Game(config).Run();
    }
}
finally
{
    ConsoleHost.Shutdown();
}
