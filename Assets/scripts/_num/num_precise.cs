using System.Numerics;
using UnityEngine;

// should be simple enough?
// we can replace this system with a compound double system if we need to, in any case

[System.Serializable]
public class num_precise
{
    // for this game, by convention, we're using 4
    int numDecimalDigits;
    BigInteger raw;

    // a few, very similar-looking constructors
    public num_precise()
    {
        numDecimalDigits = 4;
        raw = new BigInteger();
    }
    public num_precise(float num)
    {
        numDecimalDigits = 4;
        raw = new BigInteger(num);
    }
    public num_precise(double num)
    {
        numDecimalDigits = 4;
        raw = new BigInteger(num);
    }
    public num_precise(int num)
    {
        numDecimalDigits = 4;
        raw = new BigInteger(num);
    }

    // all manipulation functions return the class (this) for convinience, allowing for repeat operations

    // TODO: make these use the constructors, cuz that's good convention

    // ********************
    // division
    // ********************
    public num_precise Div(num_precise num)
    {
        // just dividing the raw numbers should work?
        raw = raw / num.raw;
        return this;
    }
    public num_precise Div(double num)
    {
        raw = raw / new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Div(float num)
    {
        raw = raw / new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    // ********************
    // multiplication
    // ********************
    public num_precise Mul(num_precise num)
    {
        raw = raw * num.raw; // again, just manipulating the raw numbers should be fine
        return this;
    }
    public num_precise Mul(double num)
    {
        raw = raw * new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Mul(float num)
    {
        raw = raw * new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    // ********************
    // subtraction
    // ********************
    public num_precise Sub(double num)
    {
        raw = raw - new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Sub(float num)
    {
        raw = raw - new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Sub(num_precise num)
    {
        raw = raw - num.raw;
        return this;
    }
    // ********************
    // addition
    // ********************
    public num_precise Add(double num)
    {
        raw = raw + new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Add(float num)
    {
        raw = raw + new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Add(num_precise num)
    {
        raw = raw + num.raw;
        return this;
    }
    // ********************
    // assignment
    // ********************
    public num_precise Set(double num)
    {
        raw = new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Set(float num)
    {
        raw = new BigInteger(num * Mathf.Pow(10, numDecimalDigits));
        return this;
    }
    public num_precise Set(num_precise num)
    {
        raw = num.raw;
        return this;
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
