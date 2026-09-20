using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어 삭제
            Destroy(collision.gameObject);

            // 장애물 삭제
            Destroy(gameObject);
        }
    }
}