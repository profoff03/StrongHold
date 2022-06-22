using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthWall: MonoBehaviour
{
    [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("DestroyPortal2"))
        {
            if (PlayerPrefs.GetInt("DestroyPortal2") == 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
