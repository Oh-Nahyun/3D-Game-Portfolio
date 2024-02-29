using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthbarSprite;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        float max = 1 / maxHealth;
        healthbarSprite.fillAmount = currentHealth * max;
    }
}