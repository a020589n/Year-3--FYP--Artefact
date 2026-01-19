using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform _healthBar;
    private HealthSystem _boundHealthSystem;
    
    private void Start()
    {
        _healthBar = transform.Find("HB_Bar");
    }

    private void SetSize(float sizeNormalised)
    {
        _healthBar.localScale = new Vector3(sizeNormalised, 1f);
    }

    public void Bind(HealthSystem healthSystem)
    {
        _boundHealthSystem = healthSystem;
        _boundHealthSystem.OnHealthChanged += SetSize;
    }

    private void OnDestroy()
    {
        if (_boundHealthSystem != null)
        {_boundHealthSystem.OnHealthChanged -= SetSize;}
    }

}
