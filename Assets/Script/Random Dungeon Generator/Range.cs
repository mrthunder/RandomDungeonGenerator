using UnityEngine;

/// <summary>
/// This class gets the range and returns a random number.
/// </summary>
[System.Serializable]
public class Range {

    public int m_imin = 0;
    public int m_imax = 0;

    public Range(int min, int max)
    {
        m_imin = min;
        m_imax = max;
    }

    public int RandomNumber ()
    {
        return Random.Range(m_imin, m_imax); 
    }
	
}
