using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private CharacterBattle characterPrefab;
    public Transform damagePopupPrefab;
    
    [Header("UI")]
    [SerializeField] private RPSPlayerUI playerUI;

    [Header("Enemy AI")]
    [SerializeField] private CombatEnums.EnemyAIType enemyAIType;
    [SerializeField] private float adaptiveWithCheatPercentage = 0.2f;
    [SerializeField] private bool intelligent = true;

    [Header("Positioning")]
    [SerializeField, Range(0, 10)] private float positionOffsetFromCentre = 3f;

    public static BattleHandler Instance { get; private set; }
    
    private CombatIntent _playerIntent;
    private CombatIntent _enemyIntent;
    
    //private readonly List<CombatIntent> _playerHistory = new();
    
    private readonly Dictionary<CombatEnums.RPSChoice, int> playerAttackCounts = new();
    private readonly Dictionary<CombatEnums.RPSChoice, int> playerDefendCounts = new();
    
    private CharacterBattle PlayerCharacter { get; set; }
    private CharacterBattle EnemyCharacter { get; set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PlayerCharacter = SpawnCharacter(true);
        EnemyCharacter  = SpawnCharacter(false);
        
        foreach (CombatEnums.RPSChoice choice in Enum.GetValues(typeof(CombatEnums.RPSChoice)))
        {
            playerAttackCounts[choice] = 0;
            playerDefendCounts[choice] = 0;
        }
        
        StartPlayerTurn();
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam)
    {
        Vector3 position = isPlayerTeam
            ? new Vector3(positionOffsetFromCentre, 0, 0)
            : new Vector3(-positionOffsetFromCentre, 0, 0);

        CharacterBattle character =
            Instantiate(characterPrefab, position, Quaternion.identity);

        character.Setup(isPlayerTeam);
        return character;
    }


    #region ---------- PLAYER TURN ----------
    
    
    private void StartPlayerTurn()
    {
        if (playerUI == null)
        {
            Debug.LogError("Player UI not assigned on BattleHandler");
            return;
        }

        playerUI.Show();
    }

    public void OnPlayerIntentChosen(CombatIntent intent)
    {
        playerAttackCounts[intent.AttackChoice]++;
        playerDefendCounts[intent.DefendChoice]++;
        
        _playerIntent = intent;
        //_enemyIntent = GenerateEnemyIntent(false);
        
        PlayerCharacter.ShowAttackIcon(intent.AttackChoice);
        EnemyCharacter.ShowDefendIcon(intent.DefendChoice);

        ExecutePlayerTurn();
    }

    private void ExecutePlayerTurn()
    {
        // Enemy prepares intent WITHOUT knowing the player's choices (no cheating)
        _enemyIntent = GenerateEnemyIntent(allowCheatPeek: false);

        PlayerCharacter.ExecuteTurn(
            EnemyCharacter,
            _playerIntent,
            _enemyIntent,
            OnPlayerTurnComplete
        );
    }

    private void OnPlayerTurnComplete()
    {
        if (CheckBattleOver()) {return;}
        
        PlayerCharacter.ClearIntentIcon();
        EnemyCharacter.ClearIntentIcon();
        
        ExecuteEnemyTurn();
    }
    
    #endregion

    #region ---------- ENEMY TURN ----------
    
    private void ExecuteEnemyTurn()
    {
        bool allowCheat =
            enemyAIType == CombatEnums.EnemyAIType.AdaptiveWithCheat
            && Random.value < adaptiveWithCheatPercentage;

        _enemyIntent = GenerateEnemyIntent(allowCheat);
        
        EnemyCharacter.ShowAttackIcon(_enemyIntent.AttackChoice);
        PlayerCharacter.ShowDefendIcon(_playerIntent.DefendChoice);

        EnemyCharacter.ExecuteTurn(
            PlayerCharacter,
            _enemyIntent,
            _playerIntent,
            OnEnemyTurnComplete
        );
    }

    private void OnEnemyTurnComplete()
    {
        if (CheckBattleOver()) {return;}
        
        PlayerCharacter.ClearIntentIcon();
        EnemyCharacter.ClearIntentIcon();
        
        StartPlayerTurn();
    }
    
    #endregion

    #region ---------- AI LOGIC ----------

    private CombatIntent GenerateEnemyIntent(bool allowCheatPeek)
    {
        return enemyAIType switch
        {
            CombatEnums.EnemyAIType.Random =>
                RandomIntent(),

            CombatEnums.EnemyAIType.Adaptive =>
                AdaptiveIntent(),

            CombatEnums.EnemyAIType.AdaptiveWithCheat =>
                allowCheatPeek
                    ? CheatingAdaptiveIntent(_playerIntent)
                    : AdaptiveIntent(),

            _ => RandomIntent()
        };
    }

    private CombatIntent RandomIntent()
    {
        CombatEnums.RPSChoice attack = (CombatEnums.RPSChoice)Random.Range(0, 3);
        CombatEnums.RPSChoice defend;
        do
        {
            defend = (CombatEnums.RPSChoice)Random.Range(0, 3);
        } while (defend == attack);

        return new CombatIntent(attack, defend);
    }

    private CombatIntent AdaptiveIntent()
    {
        // Fallback if no data yet
        if (playerAttackCounts.Values.All(v => v == 0))
            return RandomIntent();

        // ----- ATTACK SELECTION -----
        // Prefer attacks that beat commonly-used player defences
        CombatEnums.RPSChoice attack = WeightedAttackChoice();

        // ----- DEFENCE SELECTION -----
        // Prefer healing or blocking depending on intelligence
        CombatEnums.RPSChoice defend = WeightedDefenceChoice(attack);

        return new CombatIntent(attack, defend);
    }
    
    private CombatIntent CheatingAdaptiveIntent(CombatIntent playerIntent)
    {
        CombatEnums.RPSChoice attack = Counter(playerIntent.DefendChoice);

        CombatEnums.RPSChoice defend = intelligent
            ? playerIntent.AttackChoice           // heal
            : Counter(playerIntent.AttackChoice); // block

        return new CombatIntent(attack, defend);
    }
    
    private CombatEnums.RPSChoice WeightedAttackChoice()
    {
        Dictionary<CombatEnums.RPSChoice, float> weights = new();

        foreach (CombatEnums.RPSChoice choice in Enum.GetValues(typeof(CombatEnums.RPSChoice)))
        {
            // want to attack with something that beats common defences
            CombatEnums.RPSChoice beats = Counter(choice);
            if (!weights.ContainsKey(beats))
                weights[beats] = 0;

            weights[beats] += playerDefendCounts[choice] + 1;
        }

        return PickWeighted(weights);
    }
    
    private CombatEnums.RPSChoice WeightedDefenceChoice(CombatEnums.RPSChoice myAttack)
    {
        Dictionary<CombatEnums.RPSChoice, float> weights = new();

        foreach (CombatEnums.RPSChoice choice in Enum.GetValues(typeof(CombatEnums.RPSChoice)))
        {
            if (choice == myAttack) continue;

            // Healing: same as player's common attack
            if (intelligent && playerAttackCounts[choice] > 0)
            {
                weights[choice] = playerAttackCounts[choice] * 2f;
            }
            else
            {
                // Blocking: loses to player's common attack
                weights[choice] = playerAttackCounts[Counter(choice)];
            }
        }

        return PickWeighted(weights);
    }
    
    private CombatEnums.RPSChoice PickWeighted(Dictionary<CombatEnums.RPSChoice, float> weights)
    {
        float total = weights.Values.Sum();
        float roll = Random.value * total;

        foreach (var pair in weights)
        {
            roll -= pair.Value;
            if (roll <= 0)
                return pair.Key;
        }

        return weights.Keys.First();
    }
    
    private CombatEnums.RPSChoice Counter(CombatEnums.RPSChoice choice)
    {
        return choice switch
        {
            CombatEnums.RPSChoice.Rock => CombatEnums.RPSChoice.Paper,
            CombatEnums.RPSChoice.Paper => CombatEnums.RPSChoice.Scissors,
            CombatEnums.RPSChoice.Scissors => CombatEnums.RPSChoice.Rock,
            _ => CombatEnums.RPSChoice.Rock
        };
    }
    
    #endregion

    private bool CheckBattleOver()
    {
        if (PlayerCharacter.IsDead())
        {
            BattleOverWindow.ShowBattleOverWindow("DEFEAT");
            return true;
        }
        if (EnemyCharacter.IsDead())
        {
            BattleOverWindow.ShowBattleOverWindow("VICTORY");
            return true;
        }
        
        return false;
    }
}