using System;

public sealed class CombatIntent
{
    public CombatEnums.RPSChoice AttackChoice { get; }
    public CombatEnums.RPSChoice DefendChoice { get; }

    public CombatIntent(
        CombatEnums.RPSChoice attackChoice,
        CombatEnums.RPSChoice defendChoice)
    {
        if (attackChoice == defendChoice)
        {
            throw new ArgumentException(
                "Attack choice and defend choice must be different."
            );
        }

        AttackChoice = attackChoice;
        DefendChoice = defendChoice;
    }
}
