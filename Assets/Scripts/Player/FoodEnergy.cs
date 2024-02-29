using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodEnergy : MonoBehaviour, IInteracable
{
    TextMeshPro text; // 3D 글자 (UI 아님)
    public HealthBar healthBar;

    /// <summary>
    /// 사용 가능 여부
    /// </summary>
    public bool CanUse = false;

    /// <summary>
    /// 식량의 존재 유무를 알리는 델리게이트
    /// </summary>
    public Action<bool> IsDisappear;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>(true);
        gameObject.SetActive(true);
    }

    public virtual void Use()
    {
        if (CanUse) // 사용 가능할 때만 사용
        {
            StopAllCoroutines();
            StartCoroutine(GrapFood()); ///// 다가가면 자동으로 획득

            IsDisappear?.Invoke(CanUse); // 식량이 사라졌다는 신호 보내기
            Debug.Log($"CanUse : {CanUse}");
            Debug.Log("플레이어는 식량을 얻었다.");
        }
    }

    IEnumerator GrapFood()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Camera.main.
            Vector3 cameraToFood = transform.position - Camera.main.transform.position; // 카메라에서 식량으로 향하는 방향 벡터
            float angle = Vector3.Angle(transform.forward, cameraToFood);
            text.transform.rotation = transform.rotation * Quaternion.Euler(0, angle, 0);

            text.gameObject.SetActive(true);
            CanUse = true;

            // 식량을 얻으면 그 즉시 체력 회복
            healthBar.UpdateHealthBar(100, 100);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            text.gameObject.SetActive(false);
            CanUse = false;
        }
    }
}
