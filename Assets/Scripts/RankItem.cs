using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    public Text rankTop;
    public Text rankScore;
    public Text rankDestroyedPuyo;

    public void SetRankItem(int rank, long score, long destroyedPuyo)
    {
        rankTop.text = rank.ToString();
        rankScore.text = score.ToString();
        rankDestroyedPuyo.text = destroyedPuyo.ToString();
    }   
}
