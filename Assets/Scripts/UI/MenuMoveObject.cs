using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMoveObject : MonoBehaviour
{
    private Vector3 axisRotation;
    
    private float rotationSpeed = 100.0f;
    private float moveSpeed = 1.0f;
    private Vector3 endPos;
    private Vector3 dir;
    void Start()
    {
        transform.rotation = Quaternion.Euler(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360));  
        axisRotation = new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.forward*Time.deltaTime * rotationSpeed+ transform.rotation.eulerAngles);
        transform.position += dir * moveSpeed * Time.deltaTime;
        if(dir.x > 0 && transform.position.x > endPos.x || dir.x < 0  && transform.position.x < endPos.x)
        {
            Destroy(gameObject);
        }
    }

    public void SetMovement(Vector3 startPos, Vector3 endPos, float moveSpeed, float rotationSpeed)
    {
        transform.position = startPos;
        this.endPos = endPos;
        this.rotationSpeed = rotationSpeed;
        this.moveSpeed = moveSpeed;
        this.dir = (endPos - startPos).normalized;
    }
}
