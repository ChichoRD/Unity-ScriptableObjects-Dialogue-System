using UnityEngine;
using TMPro;
using System;

public class PerCharacterEffect : LettersEffect
{
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        WobbleText(textMesh, new TagsInfo { effect = TextEffect.Wave, startIndex = startIndex, endIndex = endIndex, magnitude = wobbleMagnitude});
    }

    public static void WobbleText(TMP_Text textMesh, in TagsInfo tagsInfo)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = tagsInfo.StartIndex; i < tagsInfo.EndIndex; i++)
        {
            if (i > textMesh.textInfo.characterCount || i >= textMesh.textInfo.characterInfo.Length || textMesh.textInfo.characterInfo[i].character == ' ') continue;        //Check

            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = WobbleWave(Time.time + i, 0, tagsInfo.Magnitude);       //Constants
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    public static void ShakeText(TMP_Text textMesh, in TagsInfo tagsInfo)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = tagsInfo.StartIndex; i < tagsInfo.EndIndex; i++)
        {
            if (i > textMesh.textInfo.characterCount || i >= textMesh.textInfo.characterInfo.Length || textMesh.textInfo.characterInfo[i].character == ' ') continue;        //Check

            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = Shake(tagsInfo.Magnitude);       //Constants
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    public static void ColorText(TMP_Text textMesh, in TagsInfo tagsInfo)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Color[] colors = mesh.colors;

        Color color = tagsInfo.TextColor;

        for (int i = tagsInfo.startIndex; i < tagsInfo.EndIndex; i++)
        {
            if (i > textMesh.textInfo.characterCount || i >= textMesh.textInfo.characterInfo.Length || textMesh.textInfo.characterInfo[i].character == ' ') continue;        //Check

            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            colors[index] = color;
            colors[index + 1] = color;
            colors[index + 2] = color;
            colors[index + 3] = color;
        }

        mesh.colors = colors;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    public static void ApplyMixedEffects(TMP_Text textMesh, in TagsInfo[] tagsInfos)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;

        Color[] colors = mesh.colors;
        Vector3[] vertices = mesh.vertices;

        foreach (var tag in tagsInfos)
        {
            if (tag.Effect == TextEffect.Color)
            {
                Color color = tag.TextColor;

                for (int i = tag.StartIndex; i < tag.EndIndex; i++)
                {
                    if (i > textMesh.textInfo.characterCount || i >= textMesh.textInfo.characterInfo.Length || textMesh.textInfo.characterInfo[i].character == ' ') continue;        //Check

                    TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

                    int index = c.vertexIndex;

                    for (int j = 0; j < 4; j++)     //4 vertices has a mesh
                    {
                        colors[index + j] = color;
                    }
                }

                continue;
            }

            for (int i = tag.StartIndex; i < tag.EndIndex; i++)
            {
                if (i > textMesh.textInfo.characterCount || i >= textMesh.textInfo.characterInfo.Length || textMesh.textInfo.characterInfo[i].character == ' ') continue;        //Check

                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

                int index = c.vertexIndex;

                Vector3 offset = tag.Effect switch
                {
                    TextEffect.Wave => WobbleWave(Time.time + i, 0, tag.Magnitude),
                    TextEffect.Shake => Shake(tag.Magnitude),
                    _ => Vector3.zero,
                };

                for (int j = 0; j < 4; j++)     //4 vertices has a mesh
                {
                    vertices[index+ j] += offset;
                }
            }
        }

        mesh.colors = colors;
        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    public override void WobbleText(TMP_Text textMesh)
    {
        textMesh.ForceMeshUpdate();
        Mesh mesh = textMesh.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = WobbleWave(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

}
