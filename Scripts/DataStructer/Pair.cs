using System;

[Serializable]
public class Pair  // 为代替Tuple创建的新数据结构
{
    public int i;
    public int j;

    public Pair() { }
    public Pair(int i, int j)
    {
        this.i = i; this.j = j;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        Pair pairObj = obj as Pair;
        if (pairObj == null)
            return false;

        return this.i == pairObj.i && this.j == pairObj.j;
    }

    public override int GetHashCode()
    {
        // Using XOR for generating hashcode. There are many other ways to achieve this.
        return i.GetHashCode() ^ j.GetHashCode();
    }

    public static bool operator ==(Pair pair1, Pair pair2)
    {
        // Check if both are null or are same instance
        if (System.Object.ReferenceEquals(pair1, pair2))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)pair1 == null) || ((object)pair2 == null))
        {
            return false;
        }

        return pair1.i == pair2.i && pair1.j == pair2.j;
    }

    public static bool operator !=(Pair pair1, Pair pair2)
    {
        return !(pair1 == pair2);
    }
}
