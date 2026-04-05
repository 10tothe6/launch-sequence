using System;
using UnityEngine;

[System.Serializable]
public class num_precisevector3
{
    public static num_precisevector3 zero = new num_precisevector3(0,0,0);
    public static num_precisevector3 one = new num_precisevector3(1,1,1);

    // using the unity convention
    public static num_precisevector3 left = new num_precisevector3(-1,0,0);
    public static num_precisevector3 right = new num_precisevector3(1,0,0);
    public static num_precisevector3 up = new num_precisevector3(0,1,0);
    public static num_precisevector3 down = new num_precisevector3(0,-1,0);
    public static num_precisevector3 forward = new num_precisevector3(0,0,1);
    public static num_precisevector3 backward = new num_precisevector3(0,0,-1);


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
    public num_precisevector3(Vector3 v)
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
    public num_precisevector3 Add(Vector3 other)
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
        double l = Mag();
        return new num_precisevector3(x.Div(l), y.Div(l), z.Div(l));
    }
    public double Mag()
    {
        num_precise _x = x.Mul(x);
        num_precise _y = y.Mul(y);
        num_precise _z = z.Mul(z);

        return Math.Sqrt(_x.Add(_y).Add(_z).AsDouble());
    }


    public Vector3 ToVector3()
    {
        return new Vector3(x.AsFloat(), y.AsFloat(), z.AsFloat());
    }
    public DoubleVector3 ToDoubleVector3()
    {
        return new DoubleVector3(x.AsDouble(), y.AsDouble(), z.AsDouble());
    }
}
