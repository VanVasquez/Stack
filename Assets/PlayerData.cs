using System;

[Serializable]
public class PlayerData
{
    public string Name;
    public int Score;

    public PlayerData(string name, int score)
    {
        Name = name;
        this.Score = score;
    }
}
