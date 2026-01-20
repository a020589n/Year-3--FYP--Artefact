using UnityEngine;

public static class CombatEnums
{
    public enum RPSChoice
    {
        Rock,
        Paper,
        Scissors
    }

    public enum RPSOutcome
    {
        Win,    // Attacker beats defender
        Lose,   // Attacker loses to defender
        Draw    // Same choice
    }

    public enum CombatEffect
    {
        DealDamage,
        HealDefender,
        Nothing
    }

}
