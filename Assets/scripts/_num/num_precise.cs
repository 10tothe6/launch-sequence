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

    public double AsDouble()
    {
        // we divide the number AFTER converting to a double, lest we erase the digits
        // in order to move the decimal place forward four spaces, we divide by 10,000
        return ((double)raw)/Mathf.Pow(10, numDecimalDigits);
    }
    public double AsFloat()
    {
        // we divide the number AFTER converting to a double, lest we erase the digits
        // in order to move the decimal place forward four spaces, we divide by 10,000
        return ((float)raw)/Mathf.Pow(10, numDecimalDigits);
    }
}
