/*
Copyright(c) 2021 Chicho Studio

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class DialogueObject : ScriptableObject
{
    private void OnEnable()
    {
        if (HasResponses && HasOtherDialogue)
        {
            Debug.LogError($"Dialogue '{name}' cannot contain both responses and extra-dialogue\nThe response has been removed");
            nextDialogue = null;
        }
    }

    [SerializeField] private bool autoDialogue;
    [SerializeField] private DialogueString[] dialogue;
    [SerializeField] private ResponseOptions[] responses;
    [SerializeField] private DialogueObject nextDialogue;

    public bool AutoDialogue => autoDialogue;
    public DialogueString[] Dialogue => dialogue;
    public ResponseOptions[] Responses => responses;
    public DialogueObject NextDialogue => nextDialogue;

    public bool HasResponses => Responses != null && Responses.Length > 0;
    public bool HasOtherDialogue => NextDialogue != null;

}

[Serializable]
public sealed class ResponseOptions
{
    [SerializeField] [TextArea(3, 5)] private string responseText;
    [SerializeField] private DialogueObject nextDialogue;

    public string ResponseText => responseText;
    public DialogueObject DialogueObject => nextDialogue;
}

[Serializable]
public sealed class DialogueString
{
    public Sprite portrait;
    public string narrator;

    [TextArea(3, 5)] public string @string;
    [Min(0)] public float time = 5f;
}