using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerDialogue : MonoBehaviour
{
    private int sentenceCount = 0;

    private movePlayerToMe playerToMeFirst;
    private movePlayerToMe playerToMeSecond;
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
        playerToMeFirst = GameObject.Find("wizzard(first)").GetComponent<movePlayerToMe>();
        playerToMeSecond = GameObject.Find("wizzard(second)").GetComponent<movePlayerToMe>();
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
        sentenceCount = 0;
        hud.SetActive(true);
        if (thisDialouge.layerNum == 9)
        {
            playerToMeFirst._isDestroy = true;
            hud.GetComponent<HUDBarScript>().isHeal = false;
        }   
        if (thisDialouge.layerNum == 10)
            playerToMeSecond._isDestroy = true;
        thisDialouge = null;
        animator.SetBool("IsOpen", false);
    }
}
