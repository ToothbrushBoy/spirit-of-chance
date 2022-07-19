using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RollComponent
{
    protected RollComponent child;
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

//DICE ROLL function
public class _Dice : RollComponent
{

    public int diceType { get; private set; }
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

//MODIFIER function
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

//REROLL function
public class _Reroll : RollComponent
{
    public int rerollThreshhold { get; private set; }
    public _Reroll(RollComponent c, int t)
    {
        child = c;
        rerollThreshhold = c.Evaluate(1) + t - 1;
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

//MIN function
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

//ADVANTAGE function
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

//EXPLODE function
public class _Explode : RollComponent
{
    public int explodeThreshhold { get; private set; }
    public _Explode(RollComponent c, int e)
    {
        child = c;
        explodeThreshhold = c.Evaluate(2) - e + 1;
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
    public _Crit(RollComponent c, int t)
    {
        child = c;
        critThreshhold = c.Evaluate(2) - t + 1;
    }

    override public int Evaluate(int mode)
    {
        return child.Evaluate(mode) * 2;
    }
}

