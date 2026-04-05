[System.Serializable]
public class player_keypresspacket
{
    public bool forward; // w
    public bool left; // a
    public bool back; // s
    public bool right; // d

    public bool jump; // space

    public float horizontalMouse;
    public float verticalMouse;

    public player_keypresspacket() {}

    public float GetHorizontal()
    {
        return (left ? -1 : 0) + (right ? 1 : 0);
    }

    public float GetVertical()
    {
        return (back ? -1 : 0) + (forward ? 1 : 0);
    }
}
