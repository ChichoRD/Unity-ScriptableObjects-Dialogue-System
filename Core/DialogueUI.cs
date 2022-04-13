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

using ChichoExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(TypeWriterEffect))]
[RequireComponent(typeof(ResponseHandler))]
[DisallowMultipleComponent]
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private bool playOnEnable;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject initialDialogue;

    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text narratorNameText;

    private bool applyTextEffects = true;

    private TypeWriterEffect typeWriterEffect;
    private ResponseHandler responseHandler;
    [SerializeField] private InputActionReference interactAction;

    private Coroutine textEffectsRoutine = null;
    [SerializeField] private List<TextEvent> textEvents = null;
    private UnityEvent<bool> dialoguePlaying;

    internal Dictionary<string, object> TextVariables { get; private set; } = new();

    public void RegisterTextVariable<T>(string name, T value) => TextVariables.Add(name, value);

    public bool TryGetVariableByName(string name, out object value)
    {
        value = default;

        if (!TextVariables.ContainsKey(name)) return false;

        value = TextVariables[name];
        return true;
    }

    internal bool IsDialoguePlaying
    {
        set
        {
            dialoguePlaying?.Invoke(value);
        }
    }

    private bool HasEffects => textEffectsRoutine != null;

    private void Start()
    {
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        interactAction?.action.Enable();

        if (playOnEnable) ShowInitialDialogue();
    }

    private void OnValidate() => ExtensionMethods.SpeedMeasurer(() => RegisterTextEvents(initialDialogue, new List<DialogueObject>()));

    public void ShowInitialDialogue() => ShowDialogue(initialDialogue);

    public void ShowDialogue(DialogueObject dialogue)
    {
        if (dialogue == null)
        {
            ActiveDialogueBox(false, !dialogue.AutoDialogue);
            return;
        }

        if (HasEffects) StopCoroutine(textEffectsRoutine);

        ActiveDialogueBox(true, !dialogue.AutoDialogue);
        StartCoroutine(StepThroughDialogue(dialogue));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        bool skipToNext = false;

        var waitUntilAction = new WaitUntil(() => interactAction.action.triggered);

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            DialogueString currentDialogue = dialogueObject.Dialogue[i];

            string rawText = currentDialogue.@string;
            string parsedText = string.Empty;
            TagsInfo[] tagsInfo = null;

            ExtensionMethods.SpeedMeasurer(() => parsedText = ParseText(rawText, out tagsInfo));        //Remove tags and catch tagEffects

            if (parsedText == string.Empty) { skipToNext = true; continue; };

            AddExtraDialogueInfo(currentDialogue);

            if (tagsInfo.Length > 0) textEffectsRoutine = StartCoroutine(ApplyTextEffect(tagsInfo));

            yield return typeWriterEffect.Run(parsedText, textLabel, !dialogueObject.AutoDialogue);

            if (i == dialogueObject.Dialogue.Length - 1 && (dialogueObject.HasResponses || dialogueObject.HasOtherDialogue)) break;

            if (dialogueObject.AutoDialogue)
                yield return new WaitForSeconds(currentDialogue.time);
            else
                yield return waitUntilAction;

            if (HasEffects) StopCoroutine(textEffectsRoutine);
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
            yield break;
        }

        if (dialogueObject.HasOtherDialogue)
        {
            yield return new WaitUntil(() => skipToNext || interactAction.action.triggered);
            ShowDialogue(dialogueObject.NextDialogue);
            yield break;
        }

        ActiveDialogueBox(false, !dialogueObject.AutoDialogue);

        void AddExtraDialogueInfo(DialogueString currentDialogue)
        {
            if (portrait != null)
                portrait.sprite = currentDialogue.portrait;

            string narr = currentDialogue.narrator;

            if (narratorNameText == null) return;
            if (string.IsNullOrWhiteSpace(narr))
            {
                narratorNameText.text = string.Empty;
                return;
            }

            narratorNameText.text = narr;
        }
    }


    private void ActiveDialogueBox(bool active, bool activationCallbacks)
    {
        if (activationCallbacks)
            IsDialoguePlaying = active;

        dialogueBox.SetActive(active);

        if (active) return;

        ResetDialogueBox();

        void ResetDialogueBox()
        {
            if (HasEffects) StopCoroutine(textEffectsRoutine);
            textLabel.text = string.Empty;

            if (portrait != null)
                portrait.sprite = null;

            if (narratorNameText != null)
                narratorNameText.text = string.Empty;
        }
    }

    const string TAG_PATTERN = @"<[^>]*>";
    private string ParseText(string input)
    {
        return Regex.Replace(input, TAG_PATTERN, string.Empty);
    }

    private string ParseText(string input, out TagsInfo[] tagsInfo)
    {
        const string VALUE_PATTERN = @"(?<=,[ ]).+";
        const string COLOR_CODE_PATTERN = @"[^#]+";
        const string VARIABLE_PATTERN = @"{.+}";

        var variablesFound = Regex.Matches(input, VARIABLE_PATTERN);

        for (int i = 0; i < variablesFound.Count; i++)
        {
            string name = variablesFound[i].Value.Replace("{", string.Empty).Replace("}", string.Empty);
            Match variable = variablesFound[i];

            if (!TryGetVariableByName(name, out object value))
            {
                input = Regex.Replace(input, variable.Value, string.Empty);
                continue;
            }

            input = Regex.Replace(input, variable.Value, value.ToString());
        }

        var tagsFound = Regex.Matches(input, TAG_PATTERN);

        int[] tagsFoundIndexes = new int[tagsFound.Count];
        int[] tagsFoundLenghts = new int[tagsFound.Count];

        AdjustTagsIndexes(tagsFound, tagsFoundIndexes, tagsFoundLenghts);

        TagsInfo[] newTagsInfo = new TagsInfo[tagsFound.Count];

        for (int i = 0; i < tagsFound.Count; i++)
        {
            if (tagsFound[i].Value[1] == '/') continue;

            newTagsInfo[i].startIndex = tagsFoundIndexes[i];

            string effectMatch = RemoveTagArrows(tagsFound[i].Value);

            for (int j = i + 1; j < tagsFound.Count; j++)
            {
                var tagCheck = RemoveTagArrows(tagsFound[j].Value);

                Debug.Log(tagsFound[i].Value);
                Debug.Log(tagsFound[j].Value);

                if (effectMatch[0] == tagCheck[1])
                {
                    newTagsInfo[i].endIndex = tagsFoundIndexes[j];
                    break;
                }
            }

            Debug.Log(newTagsInfo[i].startIndex);
            Debug.Log(newTagsInfo[i].endIndex);

            TextEffect gottenEffect = GetEffect(effectMatch[0]);
            newTagsInfo[i].effect = gottenEffect;

            if (gottenEffect == TextEffect.Event)
            {
                foreach (var tEvent in textEvents)
                {
                    tEvent.GetEvent(Regex.Match(effectMatch, VALUE_PATTERN).Value);
                }

                continue;
            }

            if (gottenEffect == TextEffect.Color)
            {
                string hexColorString = Regex.Match(effectMatch, COLOR_CODE_PATTERN).Value;

                Color color = ExtensionMethods.ColorFromHex(hexColorString);
                newTagsInfo[i].textColor = color;

                continue;
            }

            newTagsInfo[i].textColor = Color.white;

            if (effectMatch.Length < 4) continue;
            newTagsInfo[i].magnitude = (float)Convert.ToDouble(Regex.Match(effectMatch.Replace('.', ','), VALUE_PATTERN).Value);

        }

        tagsInfo = newTagsInfo;

        return ParseText(input);

        static void AdjustTagsIndexes(MatchCollection tagsFound, int[] tagsFoundIndexes, int[] tagsFoundLenghts)
        {
            for (int i = 0; i < tagsFound.Count; i++)
            {
                tagsFoundIndexes[i] = tagsFound[i].Index;
                tagsFoundLenghts[i] = tagsFound[i].Length;

                for (int j = 0; j < i; j++)
                {
                    tagsFoundIndexes[i] -= tagsFoundLenghts[j];
                }
            }
        }

        static string RemoveTagArrows(string tag)
        {
            string result = tag.Replace("<", string.Empty);
            result = result.Replace(">", string.Empty);

            return result;
        }
    }

    private TextEffect GetEffect(char input)
    {
        return input switch
        {
            'w' => TextEffect.Wave,
            's' => TextEffect.Shake,
            '#' => TextEffect.Color,
            'r' => TextEffect.Rainbow,
            'e' => TextEffect.Event,
            _ => TextEffect.None,
        };
    }

    [Obsolete]
    private IEnumerator ApplyTextEffect(TagsInfo tagsInfo)
    {
        //Action<TMP_Text, TagsInfo> textEffectMethods = tagsInfo.effect switch
        //{
        //    TextEffect.None => null,
        //    TextEffect.Wave => PerCharacterEffect.WobbleText,
        //    TextEffect.Shake => PerCharacterEffect.ShakeText,
        //    TextEffect.Color => PerCharacterEffect.ColorText,
        //    TextEffect.Rainbow => throw new NotImplementedException(),
        //    _ => null,
        //};

        //applyTextEffects = true;

        //if (textEffectMethods == null) yield break;

        //while (applyTextEffects)
        //{
        //    textEffectMethods.Invoke(textLabel, tagsInfo);
        //    yield return null;
        //}

        yield return null;
    }

    private IEnumerator ApplyTextEffect(TagsInfo[] tagsInfos)
    {
        applyTextEffects = true;

        List<TagsInfo> notEmptyTags = new();

        for (int i = 0; i < tagsInfos.Length; i++)
        {
            if (tagsInfos[i].effect == TextEffect.None) continue;

            notEmptyTags.Add(tagsInfos[i]);
        }

        TagsInfo[] finalTags = notEmptyTags.ToArray();

        while (applyTextEffects)
        {
            yield return null;
            PerCharacterEffect.ApplyMixedEffects(textLabel, in finalTags);
            yield return new WaitForEndOfFrame();
        }
    }

    private void RegisterTextEvents(DialogueObject currentDialogue, List<DialogueObject> cachedDialogues)
    {
        if (currentDialogue == null || cachedDialogues.Contains(currentDialogue)) return;

        const string EVENT_PATTERN = @"(?<=<e, )[^>]+";
        Debug.Log($"Checking {currentDialogue.name}");

        if (!textEvents.Select(e => e.ownerDialogue).Contains(currentDialogue))
        {
            foreach (var match in from dialogue in currentDialogue.Dialogue
                                  let text = dialogue.@string
                                  let match = Regex.Match(text, EVENT_PATTERN)
                                  select match)
            {
                if (!match.Success) continue;
                textEvents.Add(new TextEvent(match.Value, currentDialogue));
            }
        }

        cachedDialogues.Add(currentDialogue);

        if (currentDialogue.HasOtherDialogue)
        {
            RegisterTextEvents(currentDialogue.NextDialogue, cachedDialogues);
        }

        if (currentDialogue.HasResponses)
        {
            foreach (var responseDialogue in currentDialogue.Responses)
            {
                RegisterTextEvents(responseDialogue.DialogueObject, cachedDialogues);
            }
        }
    }

    public void SetInitialDialogue(DialogueObject dialogue) => initialDialogue = dialogue;
}

public struct TagsInfo
{
    internal int startIndex;
    internal int endIndex;
    internal TextEffect effect;
    internal float magnitude;
    internal Color textColor;

    public int StartIndex { get => startIndex; }
    public int EndIndex { get => endIndex; }
    public TextEffect Effect { get => effect; }
    public float Magnitude { get => magnitude; }
    public Color TextColor { get => textColor; }
}

public enum TextEffect
{
    None,
    Wave,
    Shake,
    Color,
    Rainbow,
    Event,
}

[Serializable]
public sealed class TextEvent
{
    public void GetEvent(string eventName)
    {
        if (eventName == this.eventName) dialogueEvent?.Invoke();
    }

    [HideInInspector] public string eventName;
    [SerializeField] private UnityEvent dialogueEvent;
    [HideInInspector] public DialogueObject ownerDialogue;

    public TextEvent(string eventName)
    {
        this.eventName = eventName;
    }

    public TextEvent(string eventName, DialogueObject ownerDialogue) : this(eventName)
    {
        this.ownerDialogue = ownerDialogue;
    }
}
