using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordWobble : LettersEffect
{
    public bool rainbowEffect;

    List<int> wordIndexes;
    List<int> wordLengths;

    public Gradient rainbow;

    // Start is called before the first frame update
    void Start()
    {
        CalculateWordIndexes();
    }

    private void CalculateWordIndexes()
    {
        textMesh = GetComponent<TMP_Text>();

        wordIndexes = new List<int> { 0 };
        wordLengths = new List<int>();

        string s = textMesh.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
            wordIndexes.Add(index + 1);
        }

        wordLengths.Add(s.Length - wordIndexes[wordIndexes.Count - 1]);
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

        Color[] colors = mesh.colors;

        for (int w = 0; w < wordIndexes.Count; w++)
        {
            int wordIndex = wordIndexes[w];
            Vector3 offset = WobbleWave(Time.time + w);

            for (int i = 0; i < wordLengths[w]; i++)
            {
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[wordIndex + i];

                int index = c.vertexIndex;

                if (rainbowEffect)
                    RainbowEffect(vertices, colors, index);

                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    public void WobbleText(TMP_Text textMesh, int startWordIndex, int endWordIndex)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        Color[] colors = mesh.colors;

        if (startWordIndex < 0) return;        //Check

        for (int w = startWordIndex; w < endWordIndex; w++)
        {
            if (w > wordIndexes.Count || endWordIndex > wordIndexes.Count) continue;          //Check

            int wordIndex = wordIndexes[w];
            Vector3 offset = WobbleWave(Time.time + w);

            for (int i = 0; i < wordLengths[w]; i++)
            {
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[wordIndex + i];

                int index = c.vertexIndex;

                if (rainbowEffect)
                    RainbowEffect(vertices, colors, index);

                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        textMesh.canvasRenderer.SetMesh(mesh);
    }


    private void RainbowEffect(Vector3[] vertices, Color[] colors, int index)
    {
        colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x * 0.001f, 1f));
        colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x * 0.001f, 1f));
        colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x * 0.001f, 1f));
        colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x * 0.001f, 1f));
    }
}
