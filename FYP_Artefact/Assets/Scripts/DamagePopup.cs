using System;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float regularHitFontSize = 2f;
    [SerializeField] private string regularHitFontColour = "#FFA500";
    [SerializeField] private float criticalHitFontSize = 5f;
    [SerializeField] private string criticalHitFontColour = "#FF0000";
    [SerializeField] private string healingFontColour = "#00FF00";
    
    private TMP_Text _textMesh;
    private Color _textColor;
    private float _disappearTimer;

    // Create a DamagePopup
    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit, bool isHealing)
    {
        Transform damagePopupTransform = Instantiate(BattleHandler.Instance.damagePopupPrefab, 
            position, Quaternion.identity);
        DamagePopup damagePopupScript = damagePopupTransform.GetComponent<DamagePopup>();
   
        damagePopupScript.DamagePopupSetup(damageAmount, isCriticalHit, isHealing);
        
        return damagePopupScript;
    }

    private void DamagePopupSetup(int damageAmount, bool isCriticalHit, bool isHealing)
    {
        _textMesh.SetText(damageAmount.ToString());

        if (isHealing)
        {
            _textMesh.fontSize = criticalHitFontSize;
        
            _textColor = ColourFromString(healingFontColour);
        }
        
        if (!isCriticalHit  && !isHealing)
        {
            //Is not a Critical hit and Not Healing
            
            _textMesh.fontSize = regularHitFontSize;

            _textColor = ColourFromString(regularHitFontColour);
        }
        
        if (isCriticalHit && !isHealing)
        {
            //Is a Critical Hit and Not Healing
            
            _textMesh.fontSize = criticalHitFontSize;
        
            _textColor = ColourFromString(criticalHitFontColour);
        }
        
        _textMesh.color = _textColor;
        _disappearTimer = 1f;
    }
    
    private void Awake()
    {
        _textMesh = gameObject.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        
        _disappearTimer -= Time.deltaTime;
        if (_disappearTimer <= 0)
        {
            float disappearSpeed = 3f;
            
            _textColor.a -= disappearSpeed * Time.deltaTime;
            
            _textMesh.color = _textColor;
        }

        if (_textColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    private static Color ColourFromString(string colourString)
    {
        Color colour;
        if (ColorUtility.TryParseHtmlString(colourString, out colour))
        {
            return colour;
        }
    
        Debug.LogWarning($"Invalid color string: {colourString}, defaulting to white.");
        return Color.white;
    }
}
