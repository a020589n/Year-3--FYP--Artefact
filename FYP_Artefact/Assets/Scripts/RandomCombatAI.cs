// using UnityEngine;
//
// /// <summary>
// /// Temporary, plug-and-play AI for testing.
// /// Randomly picks defend first, then attack.
// /// Ensure attack choice != defend choice.
// /// </summary>
// public class RandomCombatAI : MonoBehaviour, ICombatAI
// {
//     public CombatIntent GenerateIntent(CharacterBattle self, CharacterBattle opponent)
//     {
//         // Pick defend choice first
//         CombatEnums.RPSChoice defendChoice = (CombatEnums.RPSChoice)Random.Range(0, 3);
//
//         // Pick attack choice, must be different from defend
//         CombatEnums.RPSChoice attackChoice;
//         do
//         {
//             attackChoice = (CombatEnums.RPSChoice)Random.Range(0, 3);
//         } while (attackChoice == defendChoice);
//
//         // Return as CombatIntent
//         return new CombatIntent(attackChoice, defendChoice);
//     }
// }

