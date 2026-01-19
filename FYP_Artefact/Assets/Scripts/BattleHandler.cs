using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BattleHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private CharacterBattle characterPrefab;
    public Transform damagePopupPrefab;

    [Header("Positioning")]
    [SerializeField, Range(0, 10)] private float positionOffsetFromCentre = 3f;

    public static BattleHandler Instance { get; private set; }
    
    //TEMPORARY TESTING PURPOSES ONLY
    [SerializeField] private InputAction attack;

    // Public references for UI / turn system
    public CharacterBattle PlayerCharacter { get; private set; }
    public CharacterBattle EnemyCharacter { get; private set; }
    
    public CharacterBattle ActiveCharacter { get; private set; }

    private State _state;
    private enum State
    {
        WaitingForPlayer,
        Busy,
    }

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
        
        SetActiveCharacter(PlayerCharacter);
        
        _state = State.WaitingForPlayer;
    }

    private void Update()
    {
        if (_state == State.WaitingForPlayer)
        {
            
            if (attack.triggered)
            {
                _state = State.Busy;
                
                PlayerCharacter.Attack(EnemyCharacter, () => { ChooseNextActiveCharacter(); });
            }
        }
        
    }
    
    private void OnEnable()
    {
        attack.Enable();
    }
    
    private void OnDisable()
    {
        attack.Disable();
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

    private void SetActiveCharacter(CharacterBattle character)
    {
        ActiveCharacter = character;
    }

    private void ChooseNextActiveCharacter()
    {
        if (IsBattleOver()) { return; }
        
        if (ActiveCharacter == PlayerCharacter)
        {
            SetActiveCharacter(EnemyCharacter);
            
            _state = State.Busy;
            
            EnemyCharacter.Attack(PlayerCharacter, () => { ChooseNextActiveCharacter(); });
        }
        else
        {
            SetActiveCharacter(PlayerCharacter);
            _state = State.WaitingForPlayer;
        }
    }

    private bool IsBattleOver()
    {
        if (PlayerCharacter.IsDead())
        {
            return true;
        }
        if (EnemyCharacter.IsDead())
        {
            return true;
        }
        
        return false;
    }
}

