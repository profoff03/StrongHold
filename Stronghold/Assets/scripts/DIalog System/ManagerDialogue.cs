using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerDialogue : MonoBehaviour
{
    private int sentenceCount = 0;

    private movePlayerToMe playerToMe;
    private GameObject hud;

    public Text NameText;
    public Text DialogueText;

    public Animator animator;

    Dialogue thisDialouge;

    private Queue<string> sentences;
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        playerToMe = GameObject.Find("wizzard(first)").GetComponent<movePlayerToMe>();
        hud = GameObject.Find("HUD");
    }

    public void StartDialogue(Dialogue dialogue)
    {
        thisDialouge = dialogue;
        hud.SetActive(false);
        animator.SetBool("IsOpen", true);
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (sentenceCount % 2 == 0) NameText.text = thisDialouge.NPCName[0];
        else NameText.text = thisDialouge.NPCName[1];
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        sentenceCount++;
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        DialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void EndDialogue()
    {
        thisDialouge = null;
        sentenceCount = 0;
        hud.SetActive(true);
        playerToMe._isDestroy = true;
        animator.SetBool("IsOpen", false);
    }
}
