using UnityEngine;

public class Plane : MonoBehaviour
{
    public ColorManager colorManager;
    public TheStack t;
    private Material planeMaterial;

    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        planeMaterial = meshRenderer.material;
    }

    void Update()
    {
        ColorMesh(GetComponent<MeshFilter>().mesh);
    }

    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(t.scoreCount * 0.25f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = colorManager.Lerp4(colorManager.gameColors[0], colorManager.gameColors[1], colorManager.gameColors[2], colorManager.gameColors[3], f);
        }
        mesh.colors32 = colors;
    }
}
