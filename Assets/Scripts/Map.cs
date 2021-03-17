using System.Collections.Generic;

public class Map {
    public float timeRemaining = 300; // 300 seconds = 5 minutes

    public string name {
        get; set;
    }

    public List<string> players {
        get; set;
    }

    public List<string> playersAlive {
        get; set;
    }

    public float timer {
        get; set;
    }

    // 0 = lobby, 1 = game, 2 = winning
    public int status {
        get; set;
    }

    public Map(string name) {
        this.name = name;
        players = new List<string>();
        playersAlive = new List<string>();
        timer = 180;
        status = 0;
    }
}