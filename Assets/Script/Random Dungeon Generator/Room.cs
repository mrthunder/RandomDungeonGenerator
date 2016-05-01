using UnityEngine;

/*
 * (C) Lucas de Souza Góes 2016 
 */

/// <summary>
/// The room will create the rooms.
/// </summary>
public class Room
{

    //Room size X and Z
    private int m_iSizeX;
    private int m_iSizeZ;
    public int SizeX { get { return m_iSizeX; } }
    public int SizeZ { get { return m_iSizeZ; } }
    //Position in the main array
    private int m_iPositionX;
    private int m_iPositionZ;
    public int PositionX { get { return m_iPositionX; } }
    public int PositionZ { get { return m_iPositionZ; } }
    //The room that will be placing in the main array.
    private DungeonComponents[][] m_ThisRoom;

    //This are the bounds of the room.
    public int RightSide { get { return PositionX + SizeX; } }
    public int LeftSide { get { return PositionX; } }
    public int Top { get { return PositionZ + SizeZ; } }
    public int Bottom { get { return PositionZ; } }
    public Vector2 Center { get { return new Vector2(LeftSide + (SizeX / 2), Bottom + (SizeZ / 2)); } }

    //This check if the room was connected to another room.
    private bool m_bConnectRoom = false;
    public bool ConnnectRoom { get { return m_bConnectRoom; } }
    /// <summary>
    /// This set all the variables to create a room
    /// </summary>
    /// <param name="PositionX">position where will be created</param>
    /// <param name="PositionZ">position where will be created</param>
    /// <param name="SizeX">Size of the room. Normal is 4</param>
    /// <param name="SizeZ">Size of the room. Normal is 4</param>
    public Room(int PositionX, int PositionZ, int SizeX = 4, int SizeZ = 4)
    {
        m_iPositionX = PositionX;
        m_iPositionZ = PositionZ;
        m_iSizeX = SizeX;
        m_iSizeZ = SizeZ;
        CreateRoom();
    }
    /// <summary>
    /// This method initialize the array that represent the room.
    /// </summary>
    private void CreateRoom()
    {
        m_ThisRoom = new DungeonComponents[m_iSizeX][];
        for (int i = 0; i < m_iSizeX; i++)
        {
            m_ThisRoom[i] = new DungeonComponents[m_iSizeZ];
            for (int j = 0; j < m_iSizeZ; j++)
            {
                if(i == 0 || i == m_iSizeX-1)
                {
                    m_ThisRoom[i][j] = DungeonComponents.Wall;
                }else if (j == 0 || j == m_iSizeZ-1)
                {
                    m_ThisRoom[i][j] = DungeonComponents.Wall;
                }
                else
                {
                    m_ThisRoom[i][j] = DungeonComponents.Floor;
                }
                
            }
        }

    }

    /// <summary>
    /// Here the room array will be added in the map.
    /// </summary>
    /// <param name="map"></param>
    public void PlaceRoomInMap(DungeonComponents[][] map)
    {
        for (int i = 0; i < m_iSizeX; i++)
        {
            for (int j = 0; j < m_iSizeZ; j++)
            {
                map[m_iPositionX +i][m_iPositionZ+j] = m_ThisRoom[i][j];
            }
        }
    }

    /// <summary>
    /// Here I get the closest room base on the length of the vector distance. The lower distance is the closest one.
    /// </summary>
    /// <param name="rooms">All the rooms</param>
    /// <returns></returns>
    public Room ClosestRoom(Room[] rooms)
    {
        var ExcludeRooms = System.Array.FindAll(rooms, x => x != this);
        float distance = DistanceBetweenRooms(this, ExcludeRooms[0]);
        int index = 0;
        for (int i = 1; i < ExcludeRooms.Length; i++)
        {
            if (!ExcludeRooms[i].ConnnectRoom)
            {
                float temp = DistanceBetweenRooms(this, ExcludeRooms[i]);
                if (distance - temp > 0)
                {

                    distance = temp;
                    index = i;
                }
            }
        }
        m_bConnectRoom = true;
        return ExcludeRooms[index];
    }

    /// <summary>
    /// Get the length of the vector distance.
    /// </summary>
    /// <param name="roomA">Base room</param>
    /// <param name="roomB">Next room</param>
    /// <returns></returns>
    float DistanceBetweenRooms(Room roomA, Room roomB)
    {

        Vector2 distance = roomA.Center - roomB.Center;

        return distance.magnitude;
    }

    /// <summary>
    /// This method checks if is possible to create a room in the map.
    /// </summary>
    /// <param name="map">The map</param>
    /// <param name="newRoom">The room</param>
    /// <returns></returns>
    public static bool CanCreateARoom(DungeonComponents[][] map, Room newRoom)
    {
        for (int i = 0; i < newRoom.SizeX; i++)
        {
            for (int j = 0; j < newRoom.SizeZ; j++)
            {
                if ((newRoom.PositionX + i) >= map.Length-1 || (newRoom.PositionZ + j) >= map[i].Length-1)
                {
                    return false;
                }
                if (map[newRoom.PositionX + i][newRoom.PositionZ + j] != DungeonComponents.None)
                {
                    return false;
                }
            }
        }


        return true;

    }

   
    
}
