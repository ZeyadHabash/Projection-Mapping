using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;


public class Skinnedmeshtomesh : MonoBehaviour
{
    [SerializeField] public SkinnedMeshRenderer skinnedmesh;
    [SerializeField] public VisualEffect VFXGraph;
    [SerializeField] public float refreshRate;

    void Start()
    {
        StartCoroutine (UpdateVFXGraph());
    }

    IEnumerator UpdateVFXGraph()
    {
        while (gameObject.activeSelf)
        {
            Mesh m = new Mesh();
            skinnedmesh.BakeMesh(m);
            VFXGraph.SetMesh("Mesh",m);

            yield return new WaitForSeconds (refreshRate);
        }
    }
}
