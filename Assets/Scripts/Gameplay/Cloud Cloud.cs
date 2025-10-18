using System.Collections;
using UnityEngine;

public class WindPush : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float pushForce = 1.5f;
    public float pushDuration = 3f;
    public int maxHits = 3;
    public float lifetime = 10f;

    private int currentHits = 0;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Se mueve hacia la derecha (enemigos se mueven hacia la izquierda)
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(PushEnemy(other.gameObject));
        }

        if (other.CompareTag("Enemy") || other.CompareTag("Projectile"))
        {
            currentHits++;
            if (currentHits >= maxHits)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator PushEnemy(GameObject enemy)
    {
        float timer = 0f;
        while (timer < pushDuration && enemy != null)
        {
            enemy.transform.Translate(Vector3.right * pushForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
