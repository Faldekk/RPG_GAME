using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class CombatModeHandler : IGameModeHandler
    {
        private const string CombatHelpMessage = "Combat mode: use 1/2/3.";

        public bool Handle(InputCommand command, World world, GameState state)
        {
            switch (command)
            {
                case InputCommand.CombatNormalAttack:
                    ExecuteCombatRound(world, state, "normal");
                    return true;

                case InputCommand.CombatStealthAttack:
                    ExecuteCombatRound(world, state, "stealth");
                    return true;

                case InputCommand.CombatMagicalAttack:
                    ExecuteCombatRound(world, state, "magical");
                    return true;

                case InputCommand.Quit:
                    world.Stop();
                    return false;

                default:
                    world.AddMessage(CombatHelpMessage);
                    return false;
            }
        }

        private void ExecuteCombatRound(World world, GameState state, string attackType)
        {
            world.TryCombatRound(attackType);

            if (!world.Player.IsAlive)
            {
                state.CurrentMode = GameMode.Death;
                return;
            }

            if (!world.IsCombatActive)
            {
                state.CurrentMode = GameMode.Normal;
                world.AddMessage("Combat ended.");
            }
        }
    }
}
