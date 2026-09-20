using UnityEngine;

// 플레이어의 부메랑 발사와 쿨타임을 관리하는 스크립트
public class PlayerBoomerang : MonoBehaviour
{
    // 생성할 부메랑 프리팹
    public GameObject boomerangPrefab;

    // 부메랑이 생성될 위치와 회전값
    public Transform spawnPoint;

    [Header("추격자")]
    // 추격자가 생성되는 위치
    // 생성된 부메랑에게 전달하여 돌아오는 위치를 판단하는 데 사용
    public Transform chaserSpawnPoint;

    [Header("쿨타임")]
    // 부메랑을 던진 후 적용되는 기본 쿨타임
    public float normalCooldown = 7f;

    // 부메랑을 플레이어가 직접 회수했을 때 적용되는 짧은 쿨타임
    public float caughtCooldown = 4f;

    // 현재 남아 있는 쿨타임
    private float cooldownTimer = 0f;

    void Update()
    {
        // 쿨타임이 남아 있다면 매 프레임 시간만큼 감소
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // E키를 눌렀을 때 부메랑 발사 
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 쿨타임이 끝난 경우에만 발사
            if (cooldownTimer <= 0f)
            {
                ThrowBoomerang();
            }
        }
    }

    // 부메랑을 생성하고 필요한 정보를 전달하는 함수
    void ThrowBoomerang()
    {
        // 부메랑 프리팹을 spawnPoint 위치와 회전값으로 생성
        GameObject boomerang =
            Instantiate(
                boomerangPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        // 생성된 부메랑에서 Boomerang 스크립트를 가져온다.
        Boomerang boomerangScript =
            boomerang.GetComponent<Boomerang>();

        if (boomerangScript != null)
        {
            // 부메랑에게 플레이어 정보를 전달
            // → 플레이어와 충돌했을 때 회수 처리를 하기 위해 필요
            boomerangScript.SetPlayer(transform);

            // 부메랑에게 추격자 스폰 위치를 전달
            // → 부메랑이 돌아오는 과정에서 삭제될 위치를 계산하기 위해 필요
            boomerangScript.SetChaserSpawnPoint(chaserSpawnPoint);
        }

        // 부메랑을 던졌으므로 기본 쿨타임 적용
        cooldownTimer = normalCooldown;
    }

    // 부메랑을 플레이어가 직접 회수했을 때 Boomerang에서 호출
    public void BoomerangCaught()
    {
        // 일반 발사보다 짧은 쿨타임 적용
        cooldownTimer = caughtCooldown;
    }
}
