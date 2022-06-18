using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriiger : MonoBehaviour
{
    public Dialogue dialogue;

    internal void TriggerDialogue()
    {
        FindObjectOfType<ManagerDialogue>().StartDialogue(dialogue);
    }

}
