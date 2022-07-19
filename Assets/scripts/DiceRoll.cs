using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* DO NOT USE THIS CLASS
 * DO NOT USE THIS CLASS
 * DO NOT USE THIS CLASS
 * DO NOT USE THIS CLASS */
public class RollComponent
{
    protected RollComponent child;

    public RollComponent()
    {
        Debug.Log("oh god oh fuck no no no no no");
        Debug.Log("someone's done something very wrong");
        child = new _Dice(4);//for safety :3
    }
    virtual public int Evaluate(int mode)
    {
        return 0;
    }
    public int Evaluate() { return this.Evaluate(0); }
    virtual public int GetDiceType()
    {
        return child.GetDiceType();
    }
}
/* DO NOT USE THIS CLASS
 * DO NOT USE THIS CLASS
 * DO NOT USE THIS CLASS
 * DO NOT USE THIS CLASS */

// VV USE THESE VV

//DICE ROLL
public class _Dice : RollComponent
{

    public int diceType { get; private set; }

    public static _Dice d4 = new _Dice(4);
    public static _Dice d6 = new _Dice(6);
    public static _Dice d8 = new _Dice(8);
    public static _Dice d10 = new _Dice(10);
    public static _Dice d12 = new _Dice(12);
    public static _Dice d20 = new _Dice(20);

    private _Dice(int d)
    {
        diceType = d;
    }

    override public int  Evaluate(int mode)
    {
        switch (mode)
        {
            case 1: return 1;
            case 2: return diceType;
            default: return Roll();
        }
    }
    private int Roll() { return Random.Range(1, diceType + 1); }

    public override int GetDiceType()
    {
        return diceType;
    }
}

//MODIFIER
public class _Mod : RollComponent
{
    public int mod { get; private set; }
    public _Mod(RollComponent c, int m)
    {
        child = c;
        mod = m;
    }

    override public int Evaluate(int mode)
    {
        return child.Evaluate(mode) + mod;
    }
}

//REROLL
public class _Reroll : RollComponent
{
    public int rerollThreshhold { get; private set; }
    public _Reroll(RollComponent c, int threshholdSize)
    {
        child = c;
        rerollThreshhold = c.Evaluate(1) + threshholdSize - 1;
    }

    override public int Evaluate(int mode)
    {
        int r = child.Evaluate(mode);
        if (r <= rerollThreshhold)
        {
            r = child.Evaluate(mode);
        }
        return r;
    }
}

//MIN
public class _Min : RollComponent
{
    public int min { get; private set; }
    public _Min(RollComponent c, int m)
    {
        child = c;
        min = c.Evaluate(1) + m;
    }

    override public int Evaluate(int mode)
    {
        return Mathf.Max(child.Evaluate(mode), min);
    }
}

//ADVANTAGE
public class _Advantage : RollComponent
{
    public _Advantage(RollComponent c)
    {
        child = c;
    }
    override public int Evaluate(int mode)
    {
        return Mathf.Max(child.Evaluate(mode), child.Evaluate(mode));
    }
}

//EXPLODE
public class _Explode : RollComponent
{
    public int explodeThreshhold { get; private set; }
    public _Explode(RollComponent c, int threshholdSize)
    {
        child = c;
        explodeThreshhold = c.Evaluate(2) - threshholdSize + 1;
    }
    override public int Evaluate(int mode)
    {
        int r = child.Evaluate(mode);
        if (r >= explodeThreshhold) { if (mode == 2) { r += child.Evaluate(mode);/* limit to 1 explode when calculating max */ } else { r += Evaluate(mode); } }
        return r;
    }
}

public class _Crit : RollComponent
{
    public int critThreshhold { get; private set; }
    public _Crit(RollComponent c, int threshholdSize)
    {
        child = c;
        critThreshhold = c.Evaluate(2) - threshholdSize + 1;
    }

    override public int Evaluate(int mode)
    {
        return child.Evaluate(mode) * 2;
    }
}

