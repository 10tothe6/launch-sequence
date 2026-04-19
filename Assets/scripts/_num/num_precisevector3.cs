using System;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class num_precisevector3
{
    // i tried having things like 'zero' and 'one' here
    // however, that caused issues bc each one was a single COMMUNAL copy

    // ... so they're gone now


    public num_precise x;
    public num_precise y;
    public num_precise z;

    public num_precisevector3(double x, double y, double z)
    {
        this.x = new num_precise(x);
        this.y = new num_precise(y);
        this.z = new num_precise(z);
    }
    public num_precisevector3(float x, float y, float z)
    {
        this.x = new num_precise(x);
        this.y = new num_precise(y);
        this.z = new num_precise(z);
    }
    public num_precisevector3(num_precise x, num_precise y, num_precise z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public num_precisevector3(UnityEngine.Vector3 v)
    {
        //Debug.Log(v.x);
        this.x = new num_precise(v.x);
        this.y = new num_precise(v.y);
        this.z = new num_precise(v.z);
    }
    public num_precisevector3(DoubleVector3 v)
    {
        this.x = new num_precise(v.x);
        this.y = new num_precise(v.y);
        this.z = new num_precise(v.z);
    }

    public num_precisevector3(string xData, string yData, string zData)
    {
        this.x = new num_precise(xData);
        this.y = new num_precise(yData);
        this.z = new num_precise(zData);
    }

    // THESE HAVE TO BE FUNCTIONS BECAUSE THEY NEED TO RETURN NEW COPIES OF THE VARIABLE
    // OTHERWISE SHIT BREAKS
    // (i have seen this myself)
    // ****************************************
    public static num_precisevector3 Zero()
    {
        return new num_precisevector3(0,0,0);
    }
    public static num_precisevector3 One()
    {
        return new num_precisevector3(1,1,1);
    }


    // below are unity conventions
    public static num_precisevector3 Left()
    {
        return new num_precisevector3(-1,0,0);
    }
    public static num_precisevector3 Right()
    {
        return new num_precisevector3(1,0,0);
    }

    public static num_precisevector3 Up()
    {
        return new num_precisevector3(0,1,0);
    }
    public static num_precisevector3 Down()
    {
        return new num_precisevector3(0,-1,0);
    }

    public static num_precisevector3 Forward()
    {
        return new num_precisevector3(0,0,1);
    }
    public static num_precisevector3 Backward()
    {
        return new num_precisevector3(0,0,-1);
    }
    // ****************************************



    
    // MATH FUNCTIONS
    // ****************************************
    public num_precisevector3 Mul(num_precise other)
    {
        return new num_precisevector3(x.Mul(other), y.Mul(other), z.Mul(other));
    }
    public num_precisevector3 Mul(double other)
    {
        return new num_precisevector3(x.Mul(other), y.Mul(other), z.Mul(other));
    }
    public num_precisevector3 Mul(float other)
    {
        return new num_precisevector3(x.Mul(other), y.Mul(other), z.Mul(other));
    }


    public num_precisevector3 Div(num_precise other)
    {
        return new num_precisevector3(x.Div(other), y.Div(other), z.Div(other));
    }
    public num_precisevector3 Div(double other)
    {
        return new num_precisevector3(x.Div(other), y.Div(other), z.Div(other));
    }
    public num_precisevector3 Div(float other)
    {
        return new num_precisevector3(x.Div(other), y.Div(other), z.Div(other));
    }



    public num_precisevector3 Add(num_precisevector3 other)
    {
        return new num_precisevector3(x.Add(other.x), y.Add(other.y), z.Add(other.z));
    }
    public num_precisevector3 Add(UnityEngine.Vector3 other)
    {
        return new num_precisevector3(x.Add(other.x), y.Add(other.y), z.Add(other.z));
    }



    public num_precisevector3 Sub(num_precisevector3 other)
    {
        return new num_precisevector3(x.Sub(other.x), y.Sub(other.y), z.Sub(other.z));
    }
    // public num_precisevector3 Sub(Vector3 other)
    // {
    //     return new num_precisevector3(other.x - x, other.y - y, other.z - z);
    // }


    
    public num_precisevector3 Norm()
    {
        num_precise l = Mag();
        return new num_precisevector3(x.Div(l), y.Div(l), z.Div(l));
    }
    public double MagDouble()
    {
        num_precise _x = x.Mul(x);
        num_precise _y = y.Mul(y);
        num_precise _z = z.Mul(z);

        return Math.Sqrt(_x.Add(_y).Add(_z).AsDouble());
    }

    public num_precise Mag()
    {
        num_precise _x = x.Mul(x);
        num_precise _y = y.Mul(y);
        num_precise _z = z.Mul(z);

        return new num_precise(util_math.Sqrt(_x.Add(_y).Add(_z).raw * BigInteger.Pow(10, _x.numDecimalDigits)));
    }

    public static num_precise Distance(num_precisevector3 a, num_precisevector3 b)
    {
        return a.Sub(b).Mag();
    }
    // ****************************************

    // CONVERSIONS
    // ****************************************
    public UnityEngine.Vector3 ToVector3()
    {
        return new UnityEngine.Vector3(x.AsFloat(), y.AsFloat(), z.AsFloat());
    }
    public DoubleVector3 ToDoubleVector3()
    {
        return new DoubleVector3(x.AsDouble(), y.AsDouble(), z.AsDouble());
    }

    public string AsString()
    {
        return (x.raw / new BigInteger(Mathf.Pow(10,x.numDecimalDigits))).ToString() + "," + 
        (y.raw / new BigInteger(Mathf.Pow(10,y.numDecimalDigits))).ToString() + "," + 
        (z.raw / new BigInteger(Mathf.Pow(10,z.numDecimalDigits))).ToString();
    }
    public string AsRawString()
    {
        return x.raw.ToString() + "," + 
        y.raw.ToString() + "," + 
        z.raw.ToString();
    }

    public static num_precisevector3 FromString(string raw)
    {
        string[] elements = util_string.SplitByChar(raw,',');

        return new num_precisevector3(elements[0],elements[1],elements[2]);
    }
    // ****************************************
}
