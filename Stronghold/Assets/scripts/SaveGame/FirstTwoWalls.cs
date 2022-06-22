using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTwoWalls : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstWallObj;
    [SerializeField]
    private GameObject _thirdWallObj;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("FirstBossKilled"))
        {
            if (PlayerPrefs.GetInt("FirstBossKilled") == 1)
            {
                Destroy(_firstWallObj);
                Destroy(_thirdWallObj);
                Destroy(gameObject, 1f);
            }
        }
    }


}
