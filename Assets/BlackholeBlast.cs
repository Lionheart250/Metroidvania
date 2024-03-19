using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeBlast : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifetime = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += speed * transform.right;
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.tag == "Enemy")
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(damage, (_other.transform.position - transform.position).normalized, hitForce);
            
        }
    }
}
