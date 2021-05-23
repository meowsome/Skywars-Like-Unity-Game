using System;
using System.Collections.Generic;

// Track how much damage each player does in a game

public class DamageTracker {
    public Dictionary<string, Dictionary<string, float>> projectileHits = new Dictionary<string, Dictionary<string, float>>(); // {Sender: [{Receiver: Amount}, ...]}

    // Add amount to 
    public void addDamage(string sender, string receiver, float amount) {
        if (projectileHits.ContainsKey(sender)) {
            // Dict contains sender
            if (projectileHits[sender].ContainsKey(receiver)) {
                // Dict contains receiver
                projectileHits[sender][receiver] += amount;
            } else {
                // Dict doesn't contain receiver
                projectileHits[sender].Add(receiver, amount);
            }
        } else {
            // Dict doesn't contain sender or receiver
            projectileHits.Add(sender, new Dictionary<string, float>());
            projectileHits[sender].Add(receiver, amount);
        }
    }

    public void resetDamage() {
        projectileHits = new Dictionary<string, Dictionary<string, float>>();
    }
}