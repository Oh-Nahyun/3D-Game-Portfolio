using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    public Action<IInteracable> onEnemy;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        Transform target = other.transform;
        IInteracable obj = null;

        do
        {
            obj = target.GetComponent<IInteracable>();
            target = target.parent;
        } while (obj == null && target != null); // obj를 찾았거나 더 이상 부모가 없으면 루프 종료

        if (obj != null)
        {
            onEnemy?.Invoke(obj); // IInteracable이 있는 오브젝트를 사용했다고 알림
        }
    }
}
