using System;
using UnityEngine;

/// <summary>
/// Defines the current state of the match.
/// </summary>
public enum MatchState
{
    Waiting,    // Waiting for players
    Starting,   // Countdown before match
    Playing,    // Match in progress
    Ending,     // Match ending (showing results)
    Finished    // Match completely finished
}

/// <summary>
/// Data container for match state changes.
/// </summary>
[Serializable]
public class MatchStateData
{
    public MatchState State;
    public float TimeRemaining;
    public Team WinningTeam;
    public string Reason;

    public MatchStateData(MatchState state, float timeRemaining = 0f, Team winningTeam = Team.None, string reason = "")
    {
        State = state;
        TimeRemaining = timeRemaining;
        WinningTeam = winningTeam;
        Reason = reason;
    }
}
