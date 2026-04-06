using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class CombatModeHandler : IGameModeHandler
    {
        private const string CombatHelpMessage = "Combat mode: use 1/2/3.";

        public void Handle(InputCommand command, World world, GameState state)
        {
            switch (command)
            {
                case InputCommand.CombatNormalAttack:
                    ExecuteCombatRound(world, state, "normal");
                    break;

                case InputCommand.CombatStealthAttack:
                    ExecuteCombatRound(world, state, "stealth");
                    break;

                case InputCommand.CombatMagicalAttack:
                    ExecuteCombatRound(world, state, "magical");
                    break;

                case InputCommand.Quit:
                    world.Stop();
                    break;

                default:
                    world.AddMessage(CombatHelpMessage);
                    break;
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
