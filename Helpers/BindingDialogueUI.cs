using UnityEngine;
using UnityEngine.InputSystem;

public class BindingDialogueUI : MonoBehaviour
{
    public InputBinding.DisplayStringOptions DisplayStringOptions
    {
        get => m_DisplayStringOptions;
        set
        {
            m_DisplayStringOptions = value;
            UpdateBindingText();
        }
    }

    [Tooltip("Reference to action that is to be rebound from the UI.")]
    [SerializeField]
    private InputActionReference m_Action;

    [SerializeField]
    private string m_BindingId;

    [SerializeField]
    private InputBinding.DisplayStringOptions m_DisplayStringOptions;

    [SerializeField]
    private DialogueUI m_DialogueUI;
    
    [SerializeField]
    private string m_BindingVariableName;

    /// <summary>
    /// Trigger a refresh of the currently displayed binding.
    /// </summary>
    public void UpdateBindingText()
    {
        var displayString = string.Empty;
        var deviceLayoutName = default(string);
        var controlPath = default(string);

        // Get display string from action.
        var action = m_Action?.action;
        if (action != null)
        {
            var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
            if (bindingIndex != -1)
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, DisplayStringOptions);
        }

        if (m_DialogueUI != null)
            m_DialogueUI.RegisterTextVariable(m_BindingVariableName, displayString);
    }

    private void Awake()
    {
        UpdateBindingText();
    }
}
