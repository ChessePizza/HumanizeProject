using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RankManager : MonoBehaviour
{
    public GameObject rankDataPrototype;
    public Transform rankPanel;
    
    public List<PlayerData> playerDatas;
    
    // Start is called before the first frame update
    void Start()
    {
        CreateRankData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRankData() 
    {
        for (int i = 0; i < playerDatas.Count; i++)
        {
            GameObject rankObj = Instantiate(rankDataPrototype, rankPanel) as GameObject;
            RankData rankData = rankObj.GetComponent<RankData>();
            rankData.playerData = new PlayerData(playerDatas[i].rankNumber, playerDatas[i].playerName, playerDatas[i].playerScore);
            rankData.UpdateData();
        }

        
    }
}
