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
    private FloatVar destructionScore;

    private Rigidbody body;

    [SerializeField]
    private Transform childTransform;

    [SerializeField]
    private AudioClip destroySound;

    private DistractionRadius m_DR;

    private const string DESTRUCTABLE_LAYER_NAME = "Destructables";
    private void Start() 
    {
        body = GetComponent<Rigidbody>();
        m_DR = GetComponent<DistractionRadius>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.transform == playerScriptable.Player)
        {
            float hitMagnitude = playerScriptable.Body.velocity.magnitude;
            if(hitMagnitude >= destructionMagnitude)
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
                StartCoroutine(ApplyObjectDestruction(meshes, collision.collider.transform, hitMagnitude));
            }
            else
            {
                float ObjecthitMagnitude = body.velocity.magnitude;
                m_DR.MakeNoise(ObjecthitMagnitude, false);
            }
        }
        else
        {
            float hitMagnitude = body.velocity.magnitude;
            m_DR.MakeNoise(hitMagnitude, false);
        }

    }

    private IEnumerator ApplyObjectDestruction(Transform meshes, Transform other, float hitMagnitude)
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

}
