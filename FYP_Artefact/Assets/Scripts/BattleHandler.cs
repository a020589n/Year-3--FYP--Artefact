using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class BattleHandler : MonoBehaviour
{
    [SerializeField] private Transform characterInBattle;
    
    [Tooltip("This sets the offset for the player character. " +
             "The position of the enemy is the negative of this player offset.")]
    [SerializeField] [Range(0, 10)] private float positionOffsetFromCentre;

    private void Start()
    {
        //Spawn Player
        SpawnCharacter(true);
        
        //Spawn Enemy
        SpawnCharacter(false);
    }

    private void SpawnCharacter(bool isPlayerTeam)
    {
        Vector3 position;
        
        //Debug.Log(isPlayerTeam);

        // If isPlayerTeam = true, position is set to right, else position is set to left.
        position = isPlayerTeam ? new Vector3(positionOffsetFromCentre, 0) : new Vector3(-positionOffsetFromCentre, 0);
        
        Instantiate(characterInBattle, position, Quaternion.identity);
    }
}
