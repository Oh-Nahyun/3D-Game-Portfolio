using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    TextMeshPro text; // 3D 글자

    protected virtual void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>(true);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 cameraToEnemy = transform.position - Camera.main.transform.position;
            float angle = Vector3.Angle(transform.forward, cameraToEnemy);
            if (angle > 90.0f) // 사이각이 90도보다 크면 카메라가 적 앞에 있다.
            {
                text.transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            }
            else
            {
                text.transform.rotation = transform.rotation; // 적의 회전 그대로 적용
            }

            text.gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(TextAppear());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            text.gameObject.SetActive(false);
        }
    }

    IEnumerator TextAppear()
    {
        yield return new WaitForSeconds(3.0f);
        text.gameObject.SetActive(false);
    }
}
