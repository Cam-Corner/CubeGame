using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    [SerializeField]
    private float destructionMagnitude;

    [SerializeField]
    private PlayerScriptable playerScriptable;

    [SerializeField]
    private Transform destructionMeshes;

    private Rigidbody body;

    private void Start() 
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.transform == playerScriptable.Player)
        {
            float hitMagnitude = playerScriptable.Body.velocity.magnitude;
            if(hitMagnitude >= destructionMagnitude)
            {
                Transform meshes = Instantiate(destructionMeshes, transform.position, transform.rotation);
                StartCoroutine(ApplyObjectDestruction(meshes, collision.collider.transform, hitMagnitude));
            }
        }
    }

    private IEnumerator ApplyObjectDestruction(Transform meshes, Transform other, float hitMagnitude)
    {
        yield return new WaitForEndOfFrame();
        List<Transform> transforms = new List<Transform>();
        foreach(Transform t in meshes.transform)
        {
            if(t == meshes.transform)
            {
                continue;
            }
            transforms.Add(t);


            Rigidbody newBody = t.gameObject.AddComponent<Rigidbody>();
            t.gameObject.AddComponent(typeof(BoxCollider));
            newBody.mass = body.mass;
        }
        
        for(int i = 0; i < transforms.Count; i++)
        {
            transforms[i].parent = null;
        }
        
        Destroy(this.gameObject);
    }
}
