using System;
using System.Collections.Generic;

public static class RPSOutcomeEffectMap
{
    private static readonly Dictionary<CombatEnums.RPSOutcome, CombatEnums.CombatEffect> Map =
        new Dictionary<CombatEnums.RPSOutcome, CombatEnums.CombatEffect>
        {
            { CombatEnums.RPSOutcome.Win,  CombatEnums.CombatEffect.DealDamage },
            { CombatEnums.RPSOutcome.Lose, CombatEnums.CombatEffect.Nothing },
            { CombatEnums.RPSOutcome.Draw, CombatEnums.CombatEffect.HealDefender }
        };

    public static CombatEnums.CombatEffect GetEffect(CombatEnums.RPSOutcome outcome)
    {
        if (!Map.TryGetValue(outcome, out CombatEnums.CombatEffect effect))
        {
            throw new ArgumentOutOfRangeException(
                nameof(outcome),
                $"No CombatEffect mapped for outcome {outcome}"
            );
        }

        return effect;
    }
}