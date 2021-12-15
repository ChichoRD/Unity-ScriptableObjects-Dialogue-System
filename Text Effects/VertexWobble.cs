using UnityEngine;
using TMPro;

public class VertexWobble : LettersEffect
{
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        WobbleText(textMesh, startIndex, endIndex);
    }

    public override void WobbleText(TMP_Text textMesh)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = WobbleWave(Time.time + i);

            vertices[i] = vertices[i] + offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    public void WobbleText(TMP_Text textMesh, int startCharacter, int endCharacter)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = startCharacter * 4; i < endCharacter * 4; i++)     //Each char has 4 vertices
        {
            if (i > vertices.Length || endCharacter * 4 > vertices.Length) continue;           //Check

            Vector3 offset = WobbleWave(Time.time + i);

            vertices[i] = vertices[i] + offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

}

public abstract class LettersEffect : MonoBehaviour
{
    protected TMP_Text textMesh;

    [Range(0f, 15f)] public float wobbleMagnitude = 5f;
    public bool lockX, lockY;

    public int startIndex, endIndex;

    public Vector2 WobbleWave(float time)
    {
        return new Vector2(lockX ? 0 : Mathf.Cos(time * wobbleMagnitude), lockY ? 0 : Mathf.Sin(time * wobbleMagnitude));
    }

    public static Vector2 WobbleWave(float time, float wobbleMagnitudeX, float wobbleMagnitudeY)
    {
        return new Vector2(Mathf.Cos(time * wobbleMagnitudeX), Mathf.Sin(time * wobbleMagnitudeY));
    }

    public static Vector2 Shake(float magnitude)
    {
        return Random.insideUnitCircle.normalized * magnitude;
    }


    public abstract void WobbleText(TMP_Text textMesh);
}
