using UnityEngine;

public class CloudGroupMover : MonoBehaviour
{
    public Transform leftPortal;
    public Transform rightPortal;
    public float speed = 2f;

    void Update()
    {
        // Movimiento hacia la izquierda
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Si el grupo de nubes pasó el portal izquierdo, lo reposicionamos al derecho
        if (transform.position.x < leftPortal.position.x)
        {
            float deltaX = rightPortal.position.x - leftPortal.position.x;

            // Mueve todo el grupo la misma distancia entre portales
            transform.position += new Vector3(deltaX, 0f, 0f);
        }
    }
}
