using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu(menuName: "")]
public class TypeWriterEffect : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] letterSounds;
    [SerializeField] private float basePitch = 1;

    [SerializeField] [Range(0f, 100f)] private float typeWriterSpeed = 25f;

    [SerializeField] private InputActionReference interactAction;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        interactAction.action.Enable();
    }

    public Coroutine Run(string textToType, TMP_Text textLabel, bool skipable)
    {
        return StartCoroutine(TypeText(textToType, textLabel, skipable));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel, bool skipable)
    {
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;
        int lastCharIndex = charIndex;

        while (charIndex < textToType.Length)
        {
            yield return null;
            t += Time.deltaTime * typeWriterSpeed;

            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i == textToType.Length - 1;
                textLabel.text = textToType[..(i + 1)];

                if (PunctuationMarks.IsPunctuation(textToType[i], out float waitTime) && !isLast && !PunctuationMarks.IsPunctuation(textToType[i + 1], out float _)) yield return new WaitForSeconds(waitTime);
            }

            //textLabel.text = textToType.Substring(0, charIndex);

            if (lastCharIndex != charIndex && letterSounds.Length > 0)
            {
                float range = 0.1f;
                audioSource.pitch = basePitch + Random.Range(-range, range);
                audioSource.PlayOneShot(letterSounds[Random.Range(0, letterSounds.Length)]);
            }

            if (skipable && interactAction.action.triggered) break;

            lastCharIndex = charIndex;
        }

        textLabel.text = textToType;
        yield return null;  //Wait one more frame not to skip to next dialogue yet
    }

    public static class PunctuationMarks
    {
        private static readonly List<Punctuation> marksWaitingTime = new()
        {
            new Punctuation(new HashSet<char> {'.', '?', '!'}, 0.6f),
            new Punctuation(new HashSet<char> {',', ';', ':'}, 0.3f),
        };

        public static bool IsPunctuation(char character, out float waitTime)
        {
            foreach (var punctuation in marksWaitingTime)
            {
                if (punctuation.punctuations.Contains(character))
                {
                    waitTime = punctuation.waitTime;
                    return true;
                }
            }

            waitTime = default;
            return false;
        }

        private struct Punctuation
        {
            public readonly HashSet<char> punctuations;
            public readonly float waitTime;

            public Punctuation(HashSet<char> punctuations, float waitTime)
            {
                this.punctuations = punctuations;
                this.waitTime = waitTime;
            }
        }
    }
}
