using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTurtle : MonoBehaviour
{
    Rigidbody rigid;
    Animator animator;
    TextMeshPro text; // 3D 글자

    Player player;

    /// <summary>
    /// 적의 이동 방향
    /// </summary>
    Vector3 moveDirection;

    /// <summary>
    /// 적의 이동 속도
    /// </summary>
    public float moveSpeed = 1.0f;

    /// <summary>
    /// 플레이어와 적과의 거리
    /// </summary>
    float distanceToPlayer;

    /// <summary>
    /// 공격 파워
    /// </summary>
    public float pushPower = 0.1f;

    /// <summary>
    /// Enemy의 HP
    /// </summary>
    public int turtleHp = 100;

    public int TurtleHP
    {
        get => turtleHp;
        private set
        {
            if (turtleHp != value)
            {
                turtleHp = Math.Min(value, 100); // 최대 체력 100
            }

            if (turtleHp <= 0) // HP가 0 이하가 되면 죽는다.
            {
                turtleHp = 0;
                OnDie();
                _healthBar.UpdateHealthBar(_maxHealth, 0);
            }
        }
    }

    /// <summary>
    /// 최대 체력
    /// </summary>
    [SerializeField]
    private float _maxHealth = 100;

    /// <summary>
    /// 현재 체력
    /// </summary>
    private float _currentHealth;

    /// <summary>
    /// HealthBar UI
    /// </summary>
    [SerializeField]
    private HealthBar _healthBar;

    /// <summary>
    /// 적의 죽음을 알리는 델리게이트
    /// </summary>
    public Action<int> onTurtleDie;

    bool isTurtleDie = false;

    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int BattleModeHash = Animator.StringToHash("BattleMode");
    readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    readonly int IsDieHash = Animator.StringToHash("IsDie");

    private void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshPro>(true);

        player = GameManager.Instance.Player; // 플레이어 찾기
    }

    private void FixedUpdate()
    {
        // 플레이어와의 거리 계산
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        moveDirection = (player.transform.position - transform.position).normalized;

        if (isTurtleDie)
        {
            text.gameObject.SetActive(false);
            animator.SetBool(IsMoveHash, false);
        }

        else if (!isTurtleDie)
        {
            // 일정 거리 이하일 때만 플레이어 공격
            if (distanceToPlayer < 4.0f)
            {
                OnAttack();
            }

            // 일정 거리 이하일 때만 플레이어를 향해 이동
            else if (distanceToPlayer < 7.0f) /////&& distanceToPlayer > 2.0f
            {
                animator.SetBool(BattleModeHash, false);
                OnMove();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
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
            animator.SetBool(BattleModeHash, true);
            //Debug.Log("Enemy_BattleMode_Start");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            text.gameObject.SetActive(false);
            animator.SetBool(BattleModeHash, false);
            animator.SetBool(IsMoveHash, false);
            //Debug.Log("Enemy_BattleMode_End");
        }
    }

    private void OnMove()
    {
        text.gameObject.SetActive(false);
        animator.SetBool(IsMoveHash, true);

        // 플레이어를 향하도록 방향 조절
        transform.LookAt(player.transform);

        // 플레이어를 향해 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        /////transform.Rotate(moveDirection);
    }

    private void OnAttack()
    {
        text.gameObject.SetActive(false);
        animator.SetTrigger(IsAttackHash);
        rigid.AddForce(pushPower * moveDirection, ForceMode.Impulse);

        //Debug.Log("Enemy_Attack");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon") | collision.gameObject.CompareTag("Player"))
        {
            _currentHealth -= 35;
            TurtleHP -= 35; // 35만큼 체력 감소

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDie();
            }
            else
            {
                _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
            }

            Debug.Log($"Turtle의 EnemyHP : {TurtleHP}"); // EnemyHP 값 출력
        }
    }

    /// <summary>
    /// 사망 처리용 함수
    /// </summary>
    protected virtual void OnDie()
    {
        isTurtleDie = true;
        animator.SetTrigger(IsDieHash);
        onTurtleDie?.Invoke(TurtleHP); // 죽었다는 신호 보내기

        StopAllCoroutines();
        StartCoroutine(TakeTime());

        Debug.Log("EnemyTurtle_Die");
    }

    IEnumerator TakeTime()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
