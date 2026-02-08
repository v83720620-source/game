using UnityEngine;

/// <summary>
/// Team identification.
/// </summary>
public enum Team
{
    None = 0,
    Team1 = 1,
    Team2 = 2
}

/// <summary>
/// Team data and configuration.
/// </summary>
[System.Serializable]
public class TeamData
{
    public Team teamId;
    public string teamName = "Team 1";
    public Color teamColor = Color.blue;
    public int score = 0;
    public int kills = 0;
    public int deaths = 0;
    
    public TeamData(Team id, string name, Color color)
    {
        teamId = id;
        teamName = name;
        teamColor = color;
        score = 0;
        kills = 0;
        deaths = 0;
    }
    
    public void AddKill()
    {
        kills++;
        score++;
    }
    
    public void AddDeath()
    {
        deaths++;
    }
    
    public void Reset()
    {
        score = 0;
        kills = 0;
        deaths = 0;
    }
}
