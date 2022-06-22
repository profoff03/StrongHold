using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySaveScript : MonoBehaviour
{
    public GameObject IslandEnemy;
    public GameObject CastleEnemy;
    public GameObject MineEnemy;
    public GameObject VillageEnemy;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("DeleteEnemyIsland"))
        {
            if (PlayerPrefs.GetInt("DeleteEnemyIsland") == 1)
            {
                Destroy(IslandEnemy);
            }
        }

        if (PlayerPrefs.HasKey("DeleteEnemyCastle"))
        {
            if (PlayerPrefs.GetInt("DeleteEnemyCastle") == 1)
            {
                Destroy(CastleEnemy);
            }
        }

        if (PlayerPrefs.HasKey("DeleteEnemyMine"))
        {
            if (PlayerPrefs.GetInt("DeleteEnemyMine") == 1)
            {
                Destroy(MineEnemy);
            }
        }
        
        if (PlayerPrefs.HasKey("DeleteEnemyVillage"))
        {
            if (PlayerPrefs.GetInt("DeleteEnemyVillage") == 1)
            {
                Destroy(VillageEnemy);
            }
        }

    }

    private void IslandEnemySave()
    {
        if (IslandEnemy.transform.childCount < 1)
        {
            PlayerPrefs.SetInt("DeleteEnemyIsland", 1);
            PlayerPrefs.Save();
        }
    }

    private void CastleEnemySave()
    {
        if(CastleEnemy.transform.childCount < 1)
        {
            PlayerPrefs.SetInt("DeleteEnemyCastle", 1);
            PlayerPrefs.Save();
        }
    }

    private void MineEnemySave()
    {
        if(MineEnemy.transform.childCount < 1)
        {
            PlayerPrefs.SetInt("DeleteEnemyMine", 1);
            PlayerPrefs.Save();
        }
    }

    private void VillagaEnemySave()
    {
        if(VillageEnemy.transform.childCount < 1)
        {
            PlayerPrefs.SetInt("DeleteEnemyVillage", 1);
            PlayerPrefs.Save();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Comma))
        { PlayerPrefs.DeleteAll(); Debug.Log("DeleteAll"); }
        
        IslandEnemySave();
        CastleEnemySave();
        MineEnemySave();
        VillagaEnemySave();
    }
}
