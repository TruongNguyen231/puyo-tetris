using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using System.IO;
using UnityEngine;
using System.Linq;

public class RankBoard : MonoBehaviour
{
    public Transform entryContainer;
    public GameObject rankItem;
    // Start is called before the first frame update
    void Start()
    {
        LoadRankList();
    }

    private void LoadRankList()
    {
        List<Rank> ranksList;
        string filePath = Path.Combine(Application.streamingAssetsPath, "rank.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            RankList rankListWrapper = JsonUtility.FromJson<RankList>(json);
            ranksList = rankListWrapper.scores.Take(30).ToList();
        }
        else
        {
            Debug.Log("File not found");
            ranksList = new List<Rank>();
        }
        for (int i = 0; i < ranksList.Count; i++)
        {
            var rankItem = Instantiate(this.rankItem, entryContainer);
            rankItem.GetComponent<RankItem>().SetRankItem(i + 1, ranksList[i].scores, ranksList[i].destroyedPuyo);
        }
    }
}