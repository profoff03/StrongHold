using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("DestroyPortal3"))
        {
            if (PlayerPrefs.GetInt("DestroyPortal3") == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    
}
