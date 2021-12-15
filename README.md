# Unity ScriptableObjects Dialogue System
 Another approach for a Dialogue System avoiding overcomplicated node trees behaviours 

# Installation
1. Download this repository or import it using GitHub Desktop
2. Drag and Drop it into Unity
3. Install the following additional packages: TextMeshPro, NewInputSystem.

# How to use

DialogueUI:
   On your scene select an object and choose: AddComponent/DialogueUI.
   Thus it makes the object a controller for a Dialogue, you may have various of these around your scene, one for each NPC is recommended.
   
   Various settings of these added components can be adjusted, but the system only requires to have
   the DialogueUI component fiels filed. A prefab for a Dialogue Box is contained in /Prefabs/Text Box

DialogueObject:
   On your project assets folder create a new one for your dialogues.
   You can now create new Dialogue right-clicking on your Dialogues folder through ScriptableObjects/DialogueObject.
   
   Select your newly created object and start adjusting its properties, it is included:
   
   -Auto Dialogue: Does not require the player to pass the dialogue
    
   -Next Dialogue: Dialogue Object to be readed after finishing the current one
    
   -Possible Responses: Responses to branch the player Dialogue
    
   -DialogueString:
    -Dialoue Text: The text to be displayed with a typewriter effect, along with optional modifiers for each character 
                    if desired (Read: /Demo/DialogueTags.txt)               
    -Dialogue Portrait: A sprite to be displayed if added one 
    -Dialogue Speaker: The name of whom that phrase corresponds  
    -Dialogue Time: Used in case of being marked as Auto-Dialogue
