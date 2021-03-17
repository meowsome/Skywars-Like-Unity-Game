using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExperienceManagement : MonoBehaviour {
    public float experience;
    public int level;

    void Start() {
        //TODO set these to actual values from database
        experience = 0;
        level = 0;
    }

    public void Add(float amount) {
        experience += amount;

        int newLevel = getLevelForXP(experience);

        if (newLevel != level) handleLevelup(newLevel);
    }

    private int getLevelForXP(float xp) {
        // Use formula ceil(2 * (xp)^(e^(0.01 * xp)))
        return (int) Math.Ceiling(2 * Math.Pow(xp, Math.Exp(0.01 * xp)));
    }

    private void handleLevelup(int newLevel) {
        level = newLevel;

        // TODO
    }
}
