using UnityEngine;

// 부메랑의 이동, 회수, 장애물 파괴, 추격자 처리 등을 담당하는 스크립트
public class Boomerang : MonoBehaviour
{
    [Header("부메랑 이동")]
    public float forwardSpeed = 15f; // 부메랑이 앞으로 날아가는 속도
    public float returnSpeed = 20f;  // 부메랑이 뒤로 돌아오는 속도

    [Header("최대 발사 거리")]
    public float maxForwardDistance = 20f; // 발사 위치에서 이동할 수 있는 최대 거리

    [Header("추격자 스폰 지점 이후 삭제 거리")]
    public float destroyDistanceAfterSpawn = 3f; // 추격자 스폰 지점을 지나 이 거리만큼 이동하면 부메랑 삭제

    // 부메랑이 처음 생성된 위치
    private Vector3 startPosition;

    // 부메랑이 처음 날아갈 방향
    private Vector3 forwardDirection;

    // 부메랑을 던진 플레이어
    private Transform player;

    // 추격자가 생성되는 위치
    private Transform chaserSpawnPoint;

    // false = 앞으로 이동 / true = 플레이어 쪽으로 돌아오는 중
    private bool returning = false;

    // PlayerBoomerang에서 부메랑을 생성한 직후 호출
    // 부메랑이 누구에게 돌아가야 하는지와 최초 이동 방향을 설정한다.
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;

        // 부메랑의 최초 위치를 저장
        startPosition = transform.position;

        // 현재 부메랑이 바라보는 방향을 이동 방향으로 사용
        forwardDirection = transform.forward;

        // 런게임에서는 위아래 이동이 필요하지 않으므로 Y축 방향 제거
        forwardDirection.y = 0f;

        // 방향 벡터를 길이 1로 정규화
        forwardDirection.Normalize();
    }

    // 추격자 스폰 위치를 전달받는다.
    // 부메랑이 돌아올 때 이 위치를 기준으로 삭제 여부를 판단한다.
    public void SetChaserSpawnPoint(Transform spawnPoint)
    {
        chaserSpawnPoint = spawnPoint;
    }

    void Update()
    {
        // 아직 돌아오는 상태가 아니라면 앞으로 이동
        if (!returning)
        {
            // =========================
            // 앞으로 날아가는 단계
            // =========================

            // 최초 방향으로 일정한 속도로 이동
            transform.position +=
                forwardDirection * forwardSpeed * Time.deltaTime;

            // 처음 발사된 위치로부터 현재까지의 거리 계산
            float distance =
                Vector3.Distance(startPosition, transform.position);

            // 최대 이동 거리에 도달하면 돌아오기 시작
            if (distance >= maxForwardDistance)
            {
                returning = true;
            }
        }
        else
        {
            // =========================
            // 뒤로 돌아오는 단계
            // =========================

            // 처음 날아간 방향의 반대 방향으로 이동
            transform.position -=
                forwardDirection * returnSpeed * Time.deltaTime;

            // 추격자 스폰 위치가 설정되어 있을 때만 삭제 위치 확인
            if (chaserSpawnPoint != null)
            {
                float distanceFromSpawn =
                    Vector3.Distance(
                        transform.position,
                        chaserSpawnPoint.position
                    );

                // 추격자 스폰 위치를 지나 충분히 돌아왔으면 부메랑 삭제
                if (distanceFromSpawn <= destroyDistanceAfterSpawn)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    // 부메랑의 Trigger Collider가 다른 Collider와 충돌했을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // =================================
        // 1. 플레이어가 부메랑을 회수
        // =================================
        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                // 플레이어의 PlayerBoomerang 스크립트를 가져온다.
                PlayerBoomerang playerBoomerang =
                    player.GetComponent<PlayerBoomerang>();

                if (playerBoomerang != null)
                {
                    // 회수 성공 → 쿨타임을 짧은 값으로 변경
                    playerBoomerang.BoomerangCaught();
                }
            }

            // 회수된 부메랑은 씬에서 삭제
            Destroy(gameObject);
            return;
        }

        // =================================
        // 2. 장애물 파괴
        // =================================
        if (other.CompareTag("Obstacle"))
        {
            // 부메랑과 충돌한 장애물을 삭제
            Destroy(other.gameObject);
            return;
        }

        // =================================
        // 3. 추격자
        // =================================
        if (other.CompareTag("Chaser"))
        {
            // 추격자 오브젝트의 부모에 있는 Chaser 스크립트를 가져온다.
            Chaser chaser =
                other.GetComponentInParent<Chaser>();

            if (chaser != null)
            {
                // 추격자를 원래 스폰 위치로 돌아가게 한다.
                chaser.ReturnToStart();
            }
        }
    }
}
