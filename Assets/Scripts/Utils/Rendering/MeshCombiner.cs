using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public virtual void CombineMeshes(bool isStatic = true, bool AddCollider = true)
    {
        //Get all mesh filters and combine
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
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
