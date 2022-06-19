using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriiger : MonoBehaviour
{
    public Dialogue dialogue;

    internal void TriggerDialogue()
    {
        dialogue.layerNum = gameObject.layer;
        FindObjectOfType<ManagerDialogue>().StartDialogue(dialogue);
    }

}
