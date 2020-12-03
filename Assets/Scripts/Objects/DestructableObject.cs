using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class DestructableObject : MonoBehaviour
{
    [SerializeField]
    private float destructionMagnitude;

    [SerializeField]
    private GlobalMissionSettings m_MissionSettings;

    [SerializeField]
    private PlayerScriptable playerScriptable;

    [SerializeField]
    private Transform destructionMeshes;

    [SerializeField]
    private float destructionValue = 300;

    [SerializeField]
    private float fallDestroySpeed = 0.5f;

    [SerializeField]
    private float minSecondsInFall = 0.2f;

    [SerializeField]
    private float detectFloorDistance = 2.0f;

    [SerializeField]
    private FloatVar destructionScore;

    private Rigidbody body;

    [SerializeField]
    private Transform childTransform;

    [SerializeField]
    private AudioClip destroySound;


    private DistractionRadius m_DR;

    private bool IsFalling = false;
    private float fallTime = 0;

    private Collider destCollider;

    private const string DESTRUCTABLE_LAYER_NAME = "Destructables";
    private void Start() 
    {
        body = GetComponent<Rigidbody>();
        destCollider = GetComponent<Collider>();
        if(destCollider == null)
        {
            destCollider = GetComponentInChildren<Collider>();
        }
        m_DR = GetComponent<DistractionRadius>();

    }

    private void Update() 
    {
        if(!IsFalling)
        {
            IsFalling = body.velocity.y < 0;
            if(IsFalling)
            {
                fallTime = Time.time;
            }
        }
        else if(IsFalling && Mathf.Approximately(body.velocity.y,0))
        {
            IsFalling = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.transform == playerScriptable.Player)
        {
            float hitMagnitude = playerScriptable.Body.velocity.magnitude;
            if(hitMagnitude >= destructionMagnitude)
            {
                BreakObject(hitMagnitude);
            }
            else
            {
                float ObjecthitMagnitude = body.velocity.magnitude;
                m_DR.MakeNoise(ObjecthitMagnitude, false);
            }
        }
        else
        {
            if(IsFalling)
            {
                float fallDuration = (Time.time - fallTime);
                
                Debug.Log($"{gameObject.name} Fall Impact Magnitude({Mathf.Abs(body.velocity.y)}) Time in Air({fallDuration})");
                if(Mathf.Abs(body.velocity.y) > fallDestroySpeed && fallDuration > minSecondsInFall)
                {
                    BreakObject(destructionMagnitude);
                }
                IsFalling = false;
                fallTime = 0;
            }
            float hitMagnitude = body.velocity.magnitude;
            m_DR.MakeNoise(hitMagnitude, false);
        }

    }

    private void BreakObject(float hitMagnitude)
    {
        m_DR.MakeNoise(hitMagnitude, true);
        GameSoundBoard.Instance.PlayDestructionSound();
        //Debug.Log("Destroyed Magnitude " + hitMagnitude);

        destructionScore.Value += destructionValue;
        Transform meshes = Instantiate(destructionMeshes, transform.position, transform.rotation, transform.parent);
        foreach (Transform mesh in meshes)
        {
            mesh.gameObject.SetActive(false);
        }

        m_MissionSettings.AddBrokenPosition(transform.position);
        StartCoroutine(ApplyObjectDestruction(meshes, hitMagnitude));
    }

    private IEnumerator ApplyObjectDestruction(Transform meshes, float hitMagnitude)
    {
        yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();

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

    private void OnDrawGizmos()
    {
        if(destCollider == null)
        {
            destCollider = GetComponent<Collider>();
            if(destCollider == null)
            {
                destCollider = GetComponentInChildren<Collider>();
            }
        }
        Ray r = new Ray(destCollider.bounds.center, -Vector3.up *detectFloorDistance);
        Gizmos.DrawRay(r);
    }

}
