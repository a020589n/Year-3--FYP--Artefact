using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BattleHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private CharacterBattle characterPrefab;

    [Header("Positioning")]
    [SerializeField, Range(0, 10)] private float positionOffsetFromCentre = 3f;

    public static BattleHandler Instance { get; private set; }
    
    //TEMPORARY TESTING PURPOSES ONLY
    [SerializeField] private InputAction attack;

    // Public references for UI / turn system
    public CharacterBattle PlayerCharacter { get; private set; }
    public CharacterBattle EnemyCharacter { get; private set; }

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
        
        _state = State.WaitingForPlayer;
    }

    private void Update()
    {
        if (_state == State.WaitingForPlayer)
        {
            
            if (attack.triggered)
            {
                _state = State.Busy;
                
                PlayerCharacter.Attack(EnemyCharacter, () => { _state = State.WaitingForPlayer; });
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
}

