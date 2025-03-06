using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolRadius = 5f;
    private Vector2 startPosition;
    private Vector2 targetPosition;

    private void Start()
    {
        startPosition = transform.position;
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            targetPosition = startPosition + Random.insideUnitCircle * patrolRadius;
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(2f); // Ожидание перед следующим перемещением
        }
    }
}
