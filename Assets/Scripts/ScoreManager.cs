using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] string filename;
    private ScoreData sd;

    private void Start()
    {
        sd = new ScoreData();
    }

    public IEnumerable<PlayerData> GetHighScores()
    {
        return sd.scores.OrderByDescending(x => x.Score).Take(6);
    }

    public void AddScore(PlayerData score)
    {
        sd.scores.Add(score);
    }

    private void OnDestroy()
    {
        SaveScore();
    }

    public void SaveScore()
    {
        var json = JsonUtility.ToJson(sd);
        PlayerPrefs.SetString("scores", json);
    }
}
 