using System.Numerics;
using UnityEngine;

// should be simple enough?
// we can replace this system with a compound double system if we need to, in any case

[System.Serializable]
public class num_precise
{
    // for this game, by convention, we're using 4
    public int numDecimalDigits;
    public BigInteger raw;

    public num_precise(string data)
    {
        raw = BigInteger.Parse(data);
    }

    // a few, very similar-looking constructors
    public num_precise(BigInteger raw)
    {
        numDecimalDigits = 4;
        this.raw = raw;
    }
    public num_precise()
    {
        numDecimalDigits = 4;
        raw = new BigInteger();
    }
    public num_precise(float num)
    {
        numDecimalDigits = 4;
        raw = new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
    }
    public num_precise(double num)
    {
        numDecimalDigits = 4;
        raw = new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
    }
    public num_precise(int num)
    {
        numDecimalDigits = 4;
        raw = new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
    }

    // all manipulation functions return the class (this) for convinience, allowing for repeat operations

    // TODO: make these use the constructors, cuz that's good convention

    // okay okay, time to make a note
    // at the end of the manipulation functions,
    // DO NOT return this; YOU FUCKING DUMBASS
    // that is all

    // ********************
    // division
    // ********************
    public num_precise Div(num_precise num)
    {
        // just dividing the raw numbers should work?
        return Div(num.AsDouble());
    }
    public num_precise Div(double num)
    {
        return new num_precise(raw / new BigInteger(num));
    }
    public num_precise Div(float num)
    {
        return new num_precise(raw / new BigInteger(num));
    }
    // ********************
    // multiplication
    // ********************
    public num_precise Mul(num_precise num)
    {
        if (num.AsDouble() < 1 && num.AsDouble() > 0)
        {
            return Div(1 / num.AsDouble()); // again, just manipulating the raw numbers should be fine
        }
        else
        {
            return Mul(num.AsDouble()); // again, just manipulating the raw numbers should be fine
        }
    }
    public num_precise Mul(double num)
    {
        return new num_precise(raw * new BigInteger(num));
    }
    public num_precise Mul(float num)
    {
        return new num_precise(raw * new BigInteger(num));
    }
    // ********************
    // subtraction
    // ********************
    public num_precise Sub(double num)
    {
        return new num_precise(raw - new BigInteger(num * Mathf.Pow(10, numDecimalDigits)));
    }
    public num_precise Sub(float num)
    {
        return new num_precise(raw - new BigInteger(num * Mathf.Pow(10, numDecimalDigits)));
    }
    public num_precise Sub(num_precise num)
    {
        return new num_precise(raw - num.raw);
    }
    // ********************
    // addition
    // ********************
    public num_precise Add(double num)
    {
        return new num_precise(raw + new BigInteger(num * Mathf.Pow(10, numDecimalDigits)));
    }
    public num_precise Add(float num)
    {
        return new num_precise(raw + new BigInteger(num * Mathf.Pow(10, numDecimalDigits)));
    }
    public num_precise Add(num_precise num)
    {
        return new num_precise(raw + num.raw);
    }
    // ********************
    // assignment
    // ********************
    public num_precise Set(double num)
    {
        return new num_precise(new BigInteger(num * Mathf.Pow(10, numDecimalDigits)));
    }
    public num_precise Set(float num)
    {
        return new num_precise(new BigInteger(num * Mathf.Pow(10, numDecimalDigits)));
    }
    public num_precise Set(num_precise num)
    {
        return new num_precise(num.raw);
    }



    // ********************
    // other
    // ********************
    public double AsDouble()
    {
        // we divide the number AFTER converting to a double, lest we erase the digits
        // in order to move the decimal place forward four spaces, we divide by 10,000
        return ((double)raw)/Mathf.Pow(10, numDecimalDigits);
    }
    public float AsFloat()
    {
        // we divide the number AFTER converting to a double, lest we erase the digits
        // in order to move the decimal place forward four spaces, we divide by 10,000
        return ((float)raw)/Mathf.Pow(10, numDecimalDigits);
    }
}
