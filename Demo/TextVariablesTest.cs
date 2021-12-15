using UnityEngine;

public class TextVariablesTest : MonoBehaviour
{
    private DialogueUI dialogueUI;

    // Start is called before the first frame update
    void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();

        var randAge = Random.Range(0, 78);
        var ageName = "age";
        
        dialogueUI.RegisterTextVariable(ageName, randAge);
    }
}
