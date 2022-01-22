using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public virtual void CombineMeshes(bool isStatic = true, bool AddCollider = true)
    {
        //Get all mesh filters and combine
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length <= 0 || meshFilters.Length - 1 <= 0)
            return;
        //dont look at the parent object as we are not combine this mesh we are just adding the new combinded mesh to it
        List<MeshFilter> meshfilter = new List<MeshFilter>();
        for (int ii = 1; ii < meshFilters.Length; ii++)
            meshfilter.Add(meshFilters[ii]);

        CombineInstance[] combine = new CombineInstance[meshfilter.Count];

        int i = 0;

        while (i < meshfilter.Count)
        {
            combine[i].mesh = meshfilter[i].sharedMesh;
            combine[i].transform = meshfilter[i].transform.localToWorldMatrix;
            meshfilter[i].gameObject.SetActive(false);
            i++;
        }

        // build and optimize new mesh
        var mesh = transform.GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.CombineMeshes(combine, true, true);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        transform.gameObject.SetActive(true);

        // destroy all object we just made mesh from
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        //Add collider to mesh (if needed)
        if (AddCollider) gameObject.AddComponent<MeshCollider>();

        // set go to static
        gameObject.isStatic = isStatic;
    }
}
