using UnityEngine;
using UnityEngine.UI;
using static CombatEnums;

public class RPSIntentDisplay : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    [Header("Sprites")]
    [SerializeField] private Sprite rockSprite;
    [SerializeField] private Sprite paperSprite;
    [SerializeField] private Sprite scissorsSprite;

    public void ShowAttack(RPSChoice choice)
    {
        iconImage.sprite = GetSprite(choice);
        //iconImage.color = Color.white;   // attack = normal
        //Debug.Log($"ICON SET: {choice} → {GetSprite(choice)}");
        iconImage.enabled = true;
    }

    public void ShowDefend(RPSChoice choice)
    {
        iconImage.sprite = GetSprite(choice);
        //iconImage.color = Color.cyan;    // defend = tinted (optional)
        //Debug.Log($"ICON SET: {choice} → {GetSprite(choice)}");
        iconImage.enabled = true;
    }

    public void Clear()
    {
        iconImage.enabled = false;
    }

    private Sprite GetSprite(RPSChoice choice)
    {
        return choice switch
        {
            RPSChoice.Rock => rockSprite,
            RPSChoice.Paper => paperSprite,
            RPSChoice.Scissors => scissorsSprite,
            _ => null
        };
    }
}
