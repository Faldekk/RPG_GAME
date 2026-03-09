using System;
using RPG_GAME.App;

const int requiredWidth = 80;
const int requiredHeight = 30;

Console.CursorVisible = false;

try
{
    if (Console.BufferWidth < requiredWidth) Console.BufferWidth = requiredWidth;
    if (Console.BufferHeight < requiredHeight) Console.BufferHeight = requiredHeight;
    if (Console.WindowWidth < requiredWidth) Console.WindowWidth = requiredWidth;
    if (Console.WindowHeight < requiredHeight) Console.WindowHeight = requiredHeight;
    Console.SetWindowSize(requiredWidth, requiredHeight);
}
catch { }

if (Console.WindowWidth < requiredWidth ||
    Console.WindowHeight < requiredHeight ||
    Console.BufferWidth < requiredWidth ||
    Console.BufferHeight < requiredHeight)
{
    Console.WriteLine($"Console too small. Required: {requiredWidth} x {requiredHeight}");
    Console.CursorVisible = true;
    return;
}

Console.Clear();
new Game().Run();
Console.CursorVisible = true;
