using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private CharacterBattle characterPrefab;

    [Header("Positioning")]
    [SerializeField, Range(0, 10)] private float positionOffsetFromCentre = 3f;

    public static BattleHandler Instance { get; private set; }

    // Public references for UI / turn system
    public CharacterBattle PlayerCharacter { get; private set; }
    public CharacterBattle EnemyCharacter { get; private set; }

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


