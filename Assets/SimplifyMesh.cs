using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;


public class SimplifyMesh : MonoBehaviour
{
    public float quality = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

            var originalMesh = GetComponent<MeshFilter>().sharedMesh;

            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();

            meshSimplifier.Initialize(originalMesh);

            meshSimplifier.SimplifyMesh(quality);

            var destMesh = meshSimplifier.ToMesh();

            GetComponent<MeshFilter>().sharedMesh = destMesh;

    }
}
