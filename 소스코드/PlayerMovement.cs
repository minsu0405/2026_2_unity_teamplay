using UnityEngine;

// 플레이어의 기본 이동과 2단 점프를 담당하는 스크립트
public class PlayerMovement : MonoBehaviour
{
    // 플레이어의 이동 속도
    public float moveSpeed = 5f;

    // 한 번 점프할 때 적용되는 힘
    public float jumpForce = 7f;

    // 플레이어의 Rigidbody
    private Rigidbody rb;

    // 현재 사용한 점프 횟수
    private int jumpCount = 0;

    // 플레이어가 공중에서 사용할 수 있는 최대 점프 횟수
    // 기본값 2 → 2단 점프
    public int maxJumpCount = 2;

    void Start()
    {
        // 플레이어 오브젝트에 붙어 있는 Rigidbody 가져오기
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // =========================
        // 이동
        // =========================

        // 키보드의 좌우 입력값
        float horizontal =
            Input.GetAxis("Horizontal");

        // 키보드의 앞뒤 입력값
        float vertical =
            Input.GetAxis("Vertical");

        // Y축 이동은 제외하고 X/Z 방향으로 이동 벡터 생성
        Vector3 movement =
            new Vector3(horizontal, 0f, vertical);

        // 이동 속도와 프레임 시간을 적용하여 플레이어 이동
        transform.position +=
            movement * moveSpeed * Time.deltaTime;


        // =========================
        // 점프
        // =========================

        // Space 키를 눌렀을 때 점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 최대 점프 횟수보다 적게 사용했다면 점프 가능
            if (jumpCount < maxJumpCount)
            {
                // 기존 Y축 속도를 초기화하여 점프 높이가 일정하게 유지되도록 함
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z
                );

                // 위쪽 방향으로 순간적인 힘을 가해 점프
                rb.AddForce(
                    Vector3.up * jumpForce,
                    ForceMode.Impulse
                );

                // 사용한 점프 횟수 증가
                jumpCount++;
            }
        }
    }

    // 다른 Collider와 충돌했을 때 호출
    private void OnCollisionEnter(Collision collision)
    {
        // 바닥(Ground)에 착지했는지 확인
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 착지하면 점프 횟수를 초기화
            jumpCount = 0;
        }
    }
}
