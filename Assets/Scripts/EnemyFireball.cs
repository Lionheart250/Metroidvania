using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireBall : MonoBehaviour
{
    [SerializeField] public float damage;
    [SerializeField] public float hitForce;
    [SerializeField] public int speed;
    [SerializeField] public float lifetime = 1;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += speed * transform.right;
    }
    //detect hit
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.tag == "Player")
        {
            PlayerController.Instance.TakeDamage(damage);
        }
    }
}
