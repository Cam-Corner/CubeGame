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

    [SerializeField]
    private float destructionValue = 300;

    [SerializeField]
    private FloatVar destructionScore;

    private Rigidbody body;

    [SerializeField]
    private Transform childTransform;

    private const string DESTRUCTABLE_LAYER_NAME = "Destructables";
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
                Debug.Log("Destroyed Magnitude " + hitMagnitude);

                destructionScore.Value += destructionValue;
                Transform meshes = Instantiate(destructionMeshes, transform.position, transform.rotation, transform.parent);
                foreach (Transform mesh in meshes)
                {
                    mesh.gameObject.SetActive(false);
                }
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

            t.gameObject.layer = LayerMask.NameToLayer(DESTRUCTABLE_LAYER_NAME);
            Rigidbody newBody = t.gameObject.AddComponent<Rigidbody>();
            t.gameObject.AddComponent(typeof(BoxCollider));
            newBody.mass = body.mass;
            t.gameObject.AddComponent<TurnBasedPhysicsEntity>();
        }
        
        for(int i = 0; i < transforms.Count; i++)
        {
            transforms[i].parent = transform.parent;
            transforms[i].localScale = Vector3.Scale(childTransform.localScale, this.transform.localScale);
            transforms[i].gameObject.SetActive(true);
        }
        
        Destroy(this.gameObject);
    }
}
