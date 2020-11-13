using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObjects : MonoBehaviour
{
    [SerializeField]
    private Transform startPos;
    [SerializeField]
    private Transform endPos;
    [SerializeField]
    private MenuMoveObject[] prefabs;

    [SerializeField]
    private float spawnTime = 5.0f;

    [SerializeField]
    private float rotationSpeed = 100.0f;

    [SerializeField]
    private float moveSpeed = 1.0f;

    private float elapsedTime = 0.0f;

    private void Update() 
    {
        elapsedTime -= Time.deltaTime;
        if(elapsedTime <= 0)
        {
            Spawn();
            elapsedTime = spawnTime;
        }
    }

    private void Spawn()
    {
        int index = Random.Range(0, prefabs.Length);
        MenuMoveObject obj = Instantiate(prefabs[index], startPos.position, Quaternion.identity);
        obj.SetMovement(startPos.position, endPos.position, moveSpeed, rotationSpeed);
    }
}
