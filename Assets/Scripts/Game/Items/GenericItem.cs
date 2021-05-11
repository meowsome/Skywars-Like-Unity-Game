using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class GenericItem : NetworkBehaviour {
    // protected means I want to access this in the extended classes
    protected AttackHandler attackHandler;
    
    // "protected virtual" means that this start method will be run by an inherited class later
    protected virtual void Start() {
        attackHandler = GameObject.FindWithTag("Player").GetComponent<AttackHandler>();
    }

    public string type {
        get; set;
    }

    public bool stackable {
        get; set;
    }

    public float hitDamage {
        get; set;
    }

    public float shotDamage {
        get; set;
    }

    public bool removeAfterUse {
        get; set;
    }

    public float accuracy {
        get; set;
    }

    public float damage {
        get; set;
    }

    public float reload {
        get; set; // Reload time in seconds
    }

    public virtual bool use() {
        return false;
    }
}
