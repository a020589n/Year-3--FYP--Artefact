using UnityEngine;
using UnityEngine.UI;
using static CombatEnums;

public class RPSPlayerUI : MonoBehaviour
{
    [SerializeField] private Button rockButton;
    [SerializeField] private Button paperButton;
    [SerializeField] private Button scissorsButton;

    private RPSChoice? _attackChoice;

    private void Awake()
    {
        rockButton.onClick.AddListener(() => OnChoice(RPSChoice.Rock));
        paperButton.onClick.AddListener(() => OnChoice(RPSChoice.Paper));
        scissorsButton.onClick.AddListener(() => OnChoice(RPSChoice.Scissors));
    }

    public void Show()
    {
        _attackChoice = null;
        EnableAllButtons();
        gameObject.SetActive(true);
    }

    private void OnChoice(RPSChoice choice)
    {
        if (_attackChoice == null)
        {
            // First choice = attack
            _attackChoice = choice;
            DisableButton(choice);
        }
        else
        {
            // Second choice = defence
            CombatIntent intent = new CombatIntent(_attackChoice.Value, choice);
            gameObject.SetActive(false);

            BattleHandler.Instance.OnPlayerIntentChosen(intent);
        }
    }

    private void DisableButton(RPSChoice choice)
    {
        GetButton(choice).interactable = false;
    }

    private void EnableAllButtons()
    {
        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorsButton.interactable = true;
    }

    private Button GetButton(RPSChoice choice)
    {
        return choice switch
        {
            RPSChoice.Rock => rockButton,
            RPSChoice.Paper => paperButton,
            RPSChoice.Scissors => scissorsButton,
            _ => null
        };
    }
}