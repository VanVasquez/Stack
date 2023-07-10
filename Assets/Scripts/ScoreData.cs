using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
public class ScoreData
{
    public List<PlayerData> scores;

    public ScoreData()
    {
        scores = FileHandler.ReadListFromJSON<PlayerData>("leaderboards.json");
    }
}