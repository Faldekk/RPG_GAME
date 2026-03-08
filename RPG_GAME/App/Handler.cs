using RPG_GAME.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.App
{
    public abstract class CommandHandler
    {
        protected CommandHandler next;

        public void SetNext(CommandHandler nextHandler)
        {
            next = nextHandler;
        }

        public void Handle(InputCommand cmd, World world, Game game)
        {
            if (!Process(cmd, world, game))
                next?.Handle(cmd, world, game);
        }

        protected abstract bool Process(InputCommand cmd, World world, Game game);
    }

    public class MoveUpHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Up) return false;

            world.TryMovePlayer(0, -1);
            return true;
        }
    }

    public class MoveDownHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Down) return false;

            world.TryMovePlayer(0, 1);
            return true;
        }
    }

    public class MoveLeftHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Left) return false;

            world.TryMovePlayer(-1, 0);
            return true;
        }
    }

    public class MoveRightHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Right) return false;

            world.TryMovePlayer(1, 0);
            return true;
        }
    }

    public class PickupHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Pickup) return false;

            world.TryPickUpItem();
            return true;
        }
    }

    public class DropHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Drop) return false;

            if (!world.TryDropItem(0))
                world.TryDropItem(1);

            return true;
        }
    }

    public class SwapWeaponsHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.SwapWeapons) return false;

            world.SwapPlayerWeapons();
            return true;
        }
    }

    public class DropLeftHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.DropLeftHand) return false;

            world.TryDropSpecificHand(0);
            return true;
        }
    }

    public class DropRightHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.DropRightHand) return false;

            world.TryDropSpecificHand(1);
            return true;
        }
    }

    public class QuitHandler : CommandHandler
    {
        protected override bool Process(InputCommand cmd, World world, Game game)
        {
            if (cmd != InputCommand.Quit) return false;

            game.Stop();
            return true;
        }
    }
}






