using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollResult
{
    public int result;
    public List<int> results;

    public RollResult()
    {
        result = 0;
        results = new List<int>();
    }

    /* OPERATOR
     * OVERLOADS
     */

    // ARITHMETIC OVERLOADS
    public static RollResult operator +(RollResult a, RollResult b)
    {
        a += b.result;
        foreach(int r in b.results)
        {
            a.results.Add(r);
        }
        return a;
    }
    public static RollResult operator +(RollResult a, int b)
    {
        a.result += b;
        return a;
    }
    public static RollResult operator +(int a, RollResult b) => b + a;

    public static RollResult operator *(RollResult a, int b)
    {
        a.result *= b;
        return a;
    }
    public static RollResult operator *(int a, RollResult b) => b * a;

    // COMPARISON OVERLOADS
    public static bool operator <(RollResult a, RollResult b) => a < b.result;
    public static bool operator >(RollResult a, RollResult b) => a > b.result;
    public static bool operator ==(RollResult a, RollResult b) => a == b.result;
    public static bool operator !=(RollResult a, RollResult b) => a != b.result;
    public static bool operator <(RollResult a, int b) => b > a;
    public static bool operator >(RollResult a, int b) => b < a;
    public static bool operator ==(RollResult a, int b) => b == a;
    public static bool operator !=(RollResult a, int b) => b != a;
    public static bool operator <(int a, RollResult b) => a < b.result;
    public static bool operator >(int a, RollResult b) => a > b.result;
    public static bool operator ==(int a, RollResult b) => a == b.result;
    public static bool operator !=(int a, RollResult b) => a != b.result;
    public override bool Equals(object obj) => obj is RollResult && GetHashCode() == obj.GetHashCode();
    public override int GetHashCode() => HashCode.Combine(result, results);

    public static implicit operator int(RollResult a) => a.result;
    public static explicit operator RollResult(int a)
    {
        RollResult r = new RollResult { result = a, results = new List<int> { a } };
        return r;
    }
}

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
        child = _Dice.d4;
    }
    virtual public RollResult Evaluate(int mode)
    {
        return new RollResult();
    }
    public RollResult Evaluate() { return this.Evaluate(0); }
    virtual public int GetDiceType()
    {
        return child.GetDiceType();
    }
    virtual public int NumberUsed()
    {
        return child.NumberUsed();
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

    private int diceType;

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

    override public RollResult Evaluate(int mode)
    {
        switch (mode)
        {
            case 1: return (RollResult)1;
            case 2: return (RollResult)diceType;
            default: return (RollResult)Roll();
        }
    }
    private int Roll() { return UnityEngine.Random.Range(1, diceType + 1); }

    public override int GetDiceType()
    {
        return diceType;
    }
    public override int NumberUsed()
    {
        return 1;
    }
}

//MULTIPLIER
public class _Multi : RollComponent
{
    public int multi { get; private set; }
    public _Multi(RollComponent c, int m)
    {
        child = c;
        multi = m + 1;
    }

    public override RollResult Evaluate(int mode)
    {
        RollResult result = new RollResult();
        RollResult[] rollResults = new RollResult[multi];
        foreach(RollResult r in rollResults) { result += r; }

        return result;
    }
    public override int NumberUsed()
    {
        return multi * child.NumberUsed();
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

    override public RollResult Evaluate(int mode)
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

    override public RollResult Evaluate(int mode)
    {
        RollResult r = child.Evaluate(mode);
        if ((int)r <= rerollThreshhold)
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

    override public RollResult Evaluate(int mode)
    {
        RollResult r = child.Evaluate(mode);
        r.result = Mathf.Min(min, r.result);
        return r;
    }
}

//ADVANTAGE
public class _Advantage : RollComponent
{
    public _Advantage(RollComponent c)
    {
        child = c;
    }
    override public RollResult Evaluate(int mode)
    {
        RollResult r1 = child.Evaluate(mode);
        RollResult r2 = child.Evaluate(mode);
        if (r1 < r2) { return r2; }
            else { return r1; }
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
    override public RollResult Evaluate(int mode)
    {
        RollResult r = child.Evaluate(mode);
        if (r >= explodeThreshhold) { if (mode == 2) { r += child.Evaluate(mode); } /* limit to 1 explode when calculating max */
            else { r += this.Evaluate(mode); } }
        return r;
    }
}

//CRIT
public class _Crit : RollComponent
{
    public int critThreshhold { get; private set; }
    public _Crit(RollComponent c, int threshholdSize)
    {
        child = c;
        critThreshhold = c.Evaluate(2) - threshholdSize + 1;
    }

    override public RollResult Evaluate(int mode)
    {
        RollResult result = child.Evaluate(mode);

        if(result >= critThreshhold) { result += child.Evaluate(mode); }

        return result;
    }
}

