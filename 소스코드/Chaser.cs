using UnityEngine;

// 플레이어를 추적하고, 부메랑에 맞으면 스폰 위치로 돌아가는 추격자 스크립트
public class Chaser : MonoBehaviour
{
    // 추격할 플레이어
    public Transform player;

    // 추격자가 돌아갈 원래 스폰 위치
    public Transform spawnPoint;

    // 플레이어를 추적하는 속도
    public float speed = 1.5f;

    // 부메랑에 맞은 후 스폰 위치로 돌아가는 속도
    public float returnSpeed = 4f;

    // true = 스폰 위치로 돌아가는 중
    // false = 플레이어를 추적하는 중
    private bool returning = false;

    void Update()
    {
        // 추적할 플레이어가 지정되어 있지 않으면 아무것도 하지 않는다.
        if (player == null)
            return;

        // =========================
        // SpawnPoint로 돌아가는 중
        // =========================

        if (returning)
        {
            // 현재 위치에서 스폰 위치까지 일정 속도로 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                spawnPoint.position,
                returnSpeed * Time.deltaTime
            );

            // 스폰 위치에 거의 도착하면 정확한 위치로 맞춘다.
            if (Vector3.Distance(
                transform.position,
                spawnPoint.position) < 0.1f)
            {
                transform.position = spawnPoint.position;

                // 다시 플레이어를 추적할 수 있도록 상태 변경
                returning = false;
            }

            // 돌아가는 동안에는 아래의 플레이어 추적 코드를 실행하지 않는다.
            return;
        }

        // =========================
        // 플레이어 추적
        // =========================

        // 추격자에서 플레이어를 향하는 방향 계산
        Vector3 direction =
            player.position - transform.position;

        // 런게임이므로 높이(Y축)는 무시하고 수평 방향으로만 이동
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            // 방향 벡터를 정규화하여 이동 방향만 얻는다.
            direction.Normalize();

            // 플레이어 방향으로 이동
            transform.position +=
                direction * speed * Time.deltaTime;
        }
    }

    // =========================
    // 부메랑에 맞았을 때
    // =========================

    // Boomerang 스크립트에서 호출된다.
    // 추격자의 상태를 '스폰 위치로 돌아가는 중'으로 변경한다.
    public void ReturnToStart()
    {
        returning = true;
    }

    // =========================
    // 플레이어와 충돌
    // =========================

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어 삭제
            Destroy(collision.gameObject);

            // 추격자 자신도 삭제
            Destroy(gameObject);
        }
    }
}
