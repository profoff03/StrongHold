using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerDialogue : MonoBehaviour
{
    private movePlayerToMe playerToMe;
    private GameObject hud;

    public Text NameText;
    public Text DialogueText;

    public Animator animator;

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
        hud.SetActive(false);
        animator.SetBool("IsOpen", true);
        NameText.text = dialogue.NPCName;
        sentences.Clear();
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

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
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void EndDialogue()
    {
        hud.SetActive(true);
        playerToMe._isDestroy = true;
        animator.SetBool("IsOpen", false);
        Debug.Log("End of conversation");
    }
}
