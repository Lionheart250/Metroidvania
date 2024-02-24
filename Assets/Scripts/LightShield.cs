using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShield : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifetime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
{
    // Rotate the LightShield if the player is not facing right
    if (!PlayerStateList.Instance.lookingRight)
    {
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }
    else
    {
        transform.localRotation = Quaternion.identity;
    }

    // Move the LightShield
    //transform.position += speed * transform.right;
}
    //detect hit
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            ReflectEnemyProjectile(other.gameObject);
        }
        //else if (other.CompareTag("Enemy"))
        //{
            // Adjust the logic for damaging enemies based on your requirements
            //other.GetComponent<Enemy>().EnemyGetsHit(damage, -((other.transform.position - transform.position).normalized), -hitForce);
        //}
    }

   private void ReflectEnemyProjectile(GameObject projectile)
{
    // Check if the projectile has the "EnemyProjectile" tag
    if (projectile.CompareTag("EnemyProjectile"))
    {
        // Optionally, perform any actions before destroying the projectile (e.g., play a sound, spawn effects)
        
        // Destroy the projectile
        Destroy(projectile);
    }
}
}