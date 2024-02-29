using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodCheck : MonoBehaviour
{
    public FoodEnergy foodEnergy;

    public Image foreGround;

    /// <summary>
    /// 식량이 사라졌는지 아닌지 확인
    /// </summary>
    bool grapFood = false;

    private void Start()
    {
        if (foodEnergy != null)
        {
            foodEnergy.IsDisappear += GrapFood;
            Debug.Log("IsDisappear 델리게이트 갱신");
        }
    }

    private void Update()
    {
        OnPrint();
    }

    /// <summary>
    /// 식량 보관 여부 출력
    /// </summary>
    private void OnPrint()
    {
        if (grapFood)
        {
            foreGround.gameObject.SetActive(true);
        }
        else
        {
            foreGround.gameObject.SetActive(false);
        }
    }

    private void GrapFood(bool canUse)
    {
        grapFood = canUse;
        Debug.Log($"grapFood : {grapFood}");
    }
}
