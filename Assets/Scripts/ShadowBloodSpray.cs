using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBloodSpray : MonoBehaviour
{
    [SerializeField] GameObject bloodSpray;
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifetime = 1;
  
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
        GameObject _bloodSpray = Instantiate(bloodSpray, transform.position, transform.rotation);
        _bloodSpray.transform.parent = transform;

    }

    private void FixedUpdate()
    {
        transform.position += speed * transform.right;

    }
    //detect hit
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.tag == "Enemy")
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(damage, (_other.transform.position - transform.position).normalized, hitForce);
        }
    }
}