using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;



    /// <summary>
    /// 이동 방향 (1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// 회전 방향 (1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// 플레이어의 HP
    /// </summary>
    public int hp = 100;
    // int hpMax = 100;

    public int HP
    {
        get => hp;
        private set
        {
            if (hp != value)
            {
                hp = Math.Min(value, 100); // 최대 체력 100
                onHPChange?.Invoke(hp); // 이 델리게이트에 함수를 등록한 모든 대상에게 변경된 체력을 알림
            }

            if (hp <= 0) // HP가 0 이하가 되면 죽는다.
            {
                hp = 0;
                OnDie();
                SceneManager.LoadScene("GameEnd");
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
    /// 체력이 변경되었을 때 알리는 델리게이트 (파라메터 : 변경된 체력)
    /// </summary>
    public Action<int> onHPChange;

    /// <summary>
    /// 플레이어의 죽음을 알리는 델리게이트
    /// </summary>
    public Action<int> onDie;

    /// <summary>
    /// 애니메이터용 해시값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int UseHash = Animator.StringToHash("Use");
    readonly int AttackHash = Animator.StringToHash("Attack");
    readonly int AttackModeHash = Animator.StringToHash("AttackMode");
    readonly int BattleModeHash = Animator.StringToHash("BattleMode");
    readonly int DieHash = Animator.StringToHash("Die");

    private void Start()
    {
        _currentHealth = _maxHealth;
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions(); // 인풋 액션 생성
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        ItemUseChecker checker = GetComponentInChildren<ItemUseChecker>();
        checker.onItemUse += (interacable) => interacable.Use(); ///// 일회용이기에 람다식 사용
    }

    private void FixedUpdate() ///// Update를 사용하면 캐릭터가 떨리는 현상이 나타나므로, FixedUpdate 사용
    {
        Move();
        Rotate();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable(); // 활성화될 때 Player 액션맵을 활성화
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Attack.performed += OnAttackInput;
        inputActions.Player.Use.performed += OnUseInput;
    }

    private void OnDisable()
    {
        inputActions.Player.Use.performed -= OnUseInput;
        inputActions.Player.Attack.performed -= OnAttackInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable(); // Player 액션맵을 비활성화
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetMoveInput(context.ReadValue<Vector2>(), !context.canceled);
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void OnUseInput(InputAction.CallbackContext context)
    {
        animator.SetTrigger(UseHash);

        StopAllCoroutines();
        StartCoroutine(TakeTime());

        //Debug.Log("Player_Use");
    }

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="IsMove">이동 중이면 true, 이동 중이 아니면 false</param>
    void SetMoveInput(Vector2 input, bool IsMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;

        animator.SetBool(IsMoveHash, IsMove);
    }

    /// <summary>
    /// 실제 이동 처리 함수 (FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// 실제 회전 처리 함수 (FixedUpdate에서 사용)
    /// </summary>
    void Rotate()
    {
        // 이번 FixedUpdate에서 추가로 회전할 회전 (delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);
        rigid.MoveRotation(rigid.rotation * rotate); // 현재 회전에서 rotate만큼 추가로 회전
    }

    /// <summary>
    /// 실제 공격 처리를 하는 함수
    /// </summary>
    void Attack()
    {
        animator.SetTrigger(AttackModeHash);

        int randAttack = UnityEngine.Random.Range(0, 3) + 1;
        animator.SetInteger(AttackHash, randAttack);

        StopAllCoroutines();
        StartCoroutine(TakeTime());

        //Debug.Log("Player_Attack");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // 플레이어를 중심으로 적이 일정 거리 안에 들어오면, 공격 모드로 변경
        {
            animator.SetBool(BattleModeHash, true);

            //Debug.Log("Player_BattleMode_Start");
        }

        //if (other.CompareTag("Door"))
        //{
        //    SceneManager.LoadScene("GameClear");
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            animator.SetBool(BattleModeHash, false);

            //Debug.Log("Player_BattleMode_End");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // 플레이어가 적을 만나면 죽기
        {
            _currentHealth -= 20;
            HP -= 20; // 20만큼 체력 감소 // 하트 5개 배정 예정

            if (_currentHealth < 0)
            {
                _currentHealth = 0;
                OnDie();
            }
            else
            {
                _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
            }

            Debug.Log($"Player의 HP : {HP}"); // HP 값 출력
        }
    }

    /// <summary>
    /// 플레이어가 죽는 함수
    /// </summary>
    public void OnDie()
    {
        animator.SetTrigger(DieHash);
        inputActions.Player.Disable(); // Player 액션맵을 비활성화
        onDie?.Invoke(HP); // 사망했음을 알림
        Debug.Log("Player_Die");
    }

    IEnumerator TakeTime()
    {
        inputActions.Player.Disable(); // Player 액션맵을 비활성화
        yield return new WaitForSeconds(0.7f);
        inputActions.Player.Enable(); // Player 액션맵을 활성화
    }

    /*
    /// <summary>
    /// 점프력
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// 점프 중인지 아닌지 나타내는 변수
    /// </summary>
    bool isJumping = false;

    /// <summary>
    /// 점프 쿨타임
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float jumpCoolRemains = -1.0f;

    /// <summary>
    /// 점프가 가능한지 확인하는 프로퍼티 (점프중이 아니고 쿨타임이 다 지났다.)
    /// </summary>
    bool IsJumpAvailable => !isJumping && (jumpCoolRemains < 0.0f);

    private void Update()
    {
        jumpCoolRemains -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    /// <summary>
    /// 실제 점프 처리를 하는 함수
    /// </summary>
    void Jump()
    {
        if (IsJumpAvailable) // 점프가 가능할 때만 점프
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse); // 위쪽으로 jumpPower만큼 힘을 더하기
            jumpCoolRemains = jumpCoolTime; // 쿨타임 초기화
            isJumping = true; // 점프했다고 표시
        }
    }
    */
}
