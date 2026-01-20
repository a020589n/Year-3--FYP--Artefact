using System;

public static class RPSRulesResolver
{
    public static CombatEnums.RPSOutcome ResolveOutcome(
        CombatEnums.RPSChoice attackChoice,
        CombatEnums.RPSChoice defendChoice)
    {
        if (attackChoice == defendChoice)
        {
            return CombatEnums.RPSOutcome.Draw;
        }

        if (Beats(attackChoice, defendChoice))
        {
            return CombatEnums.RPSOutcome.Win;
        }

        return CombatEnums.RPSOutcome.Lose;
    }

    private static bool Beats(
        CombatEnums.RPSChoice attacker,
        CombatEnums.RPSChoice defender)
    {
        return (attacker == CombatEnums.RPSChoice.Rock     && defender == CombatEnums.RPSChoice.Scissors)
               || (attacker == CombatEnums.RPSChoice.Scissors && defender == CombatEnums.RPSChoice.Paper)
               || (attacker == CombatEnums.RPSChoice.Paper    && defender == CombatEnums.RPSChoice.Rock);
    }
}

