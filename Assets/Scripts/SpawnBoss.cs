using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public static SpawnBoss Instance;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject boss;
    [SerializeField] Vector2 exitDirection;
    bool callOnce;
    BoxCollider2D col;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player"))
        {
            if(!callOnce)
            {
                StartCoroutine(WalkIntoRoom());
                callOnce = true;
            }
        }
    }
    IEnumerator WalkIntoRoom()
    {
        StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, 1));
        yield return new WaitForSeconds(1f);
        col.isTrigger = false;
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }
    public void IsNotTrigger()
    {
        col.isTrigger = true;
    }
}
