using UnityEngine;
using System.Collections;

/*
 * (C) Lucas de Souza Góes 2016 
 */

//The dungeon component is the component used to create the map.
public enum DungeonComponents { None, Floor, Wall };

/// <summary>
/// This class is responsible to create a dungeon base on parameters as size of the dungeon, size of the rooms
/// numbers of rooms and the prefabs.
/// </summary>
[AddComponentMenu("Dungeon/Dungeon Generator")]
public class DungeonGenerator : MonoBehaviour
{
    //This is the average size of a room. The range type is just return a random number.
    public Range RoomsSize = new Range(4, 10);
    //The Size of the map. This is basically the of the array and not a real measure.
    [SerializeField]
    private int m_iSizeX = 10;
    [SerializeField]
    private int m_iSizeZ = 10;
    //Number of rooms in the map
    [SerializeField]
    private int m_iNumberRooms = 4;
    //The map. The map will only contain information how the dungeon will be create.
    private DungeonComponents[][] Map;
    //The rooms in the map. I store then in an array to be have a better control of the map.
    private Room[] m_Rooms;
    //This is the position of the rooms. I use a range, because the position will be random. 
    private Range m_PositionX;
    private Range m_PositionZ;
    //This are the prefabs used to create the dungeon.
    public GameObject PrefabFloor;
    public GameObject PrefabWall;
    
    // Use this for initialization
    void Start()
    {
        //The arrays of rooms is initialize with the number of rooms
        m_Rooms = new Room[m_iNumberRooms];
        //I set the range base on the size
        m_PositionX = new Range(0, m_iSizeX);
        m_PositionZ = new Range(0, m_iSizeZ);
        //... then I initialize the map, by crating the array.
        InitializeMap();
        //Create the rooms.
        CreateRooms();
        //Connect the rooms with corridors.
        ConnectRooms();
        //Create the visual dungeon base on the map, using the prefabs.
        InstantiatePrefabs();
    }

    /// <summary>
    /// Initialize the array base on the size of the dungeon.
    /// </summary>
    void InitializeMap()
    {
        Map = new DungeonComponents[m_iSizeX][];
        for (int i = 0; i < m_iSizeX; i++)
        {
            Map[i] = new DungeonComponents[m_iSizeZ];
        }
    }
    /// <summary>
    /// In this method I try to create the rooms base on the size of the array.
    /// But the rooms can't be created in the same place, so if the room is going to conflict with another,
    /// I try to create in another place. This action will happend five times. In case of failure in the five
    /// attemps, the process of creating the rooms will be stoped (because the map can affort having more rooms, judging by the size).
    /// </summary>
    void CreateRooms()
    {
        
        for(int i = 0; i < m_Rooms.Length;i++)
        {
            int numberOfTries = 0;
            Room temporaryRoom = new Room(m_PositionX.RandomNumber(), m_PositionZ.RandomNumber(), RoomsSize.RandomNumber(), RoomsSize.RandomNumber());
            while(!Room.CanCreateARoom(Map, temporaryRoom))
            {
                if (numberOfTries > 5)
                {
                    return; // If the program tries five times to create a room but fail, i means that there is no space for it, so cancel it and the next rooms.
                }
                temporaryRoom = new Room(m_PositionX.RandomNumber(), m_PositionZ.RandomNumber(), RoomsSize.RandomNumber(), RoomsSize.RandomNumber());
                numberOfTries++;
            }
            
            m_Rooms[i] = temporaryRoom;
            m_Rooms[i].PlaceRoomInMap(Map);
        }
    }

    /// <summary>
    /// In this method I check if there is any room close to another. When i get the result I connect then with a corridor. 
    /// </summary>
    void ConnectRooms()
    {
        for (int i = 0; i < m_Rooms.Length; i++)
        {
            Room close = m_Rooms[i].ClosestRoom(m_Rooms);
            CreateCorridor(m_Rooms[i], close);
        }
    }

    /// <summary>
    /// The corridor is created by creating a path that goes to the x and z position of the room b.
    /// </summary>
    /// <param name="roomA">Initial room</param>
    /// <param name="roomB">Destination</param>
    void CreateCorridor(Room roomA, Room roomB)
    {
        int LengthX = (int)(roomB.Center.x - roomA.Center.x);
        int LengthZ = (int)(roomB.Center.y - roomA.Center.y);
        if (LengthX >= 0)
        {
            for (int i = 0; i < LengthX + 1; i++)
            {
                int x = (int)roomA.Center.x + i;
                int z = (int)roomA.Center.y;
                Map[x][z] = DungeonComponents.Floor;
                CreateWalls(x, z, true);
            }
            if (LengthZ >= 0)
            {
                for (int i = 0; i < LengthZ + 1; i++)
                {
                    int x = (int)roomA.Center.x + LengthX;
                    int z = (int)roomA.Center.y + i;
                    Map[x][z] = DungeonComponents.Floor;
                    CreateWalls(x, z, false);
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(LengthZ) + 1; i++)
                {
                    int x = (int)roomA.Center.x + LengthX;
                    int z = (int)roomA.Center.y - i;
                    Map[x][z] = DungeonComponents.Floor;
                    CreateWalls(x, z, false);
                }
            }
        }
        else
        {
            for (int i = 0; i < Mathf.Abs(LengthX) + 1; i++)
            {
                int x = (int)roomA.Center.x - i;
                int z = (int)roomA.Center.y;
                Map[x][z] = DungeonComponents.Floor;
                CreateWalls(x, z, true);
            }
            if (LengthZ >= 0)
            {
                for (int i = 0; i < LengthZ + 1; i++)
                {
                    int x = (int)roomA.Center.x + LengthX;
                    int z = (int)roomA.Center.y + i;
                    Map[x][z] = DungeonComponents.Floor;
                    CreateWalls(x, z, false);
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(LengthZ) + 1; i++)
                {
                    int x = (int)roomA.Center.x + LengthX;
                    int z = (int)roomA.Center.y - i;
                    Map[x][z] = DungeonComponents.Floor;
                    CreateWalls(x, z, false);
                }
            }

        }

    }

    /// <summary>
    /// This method creates the walls for the corridors, by checking if there is any empty space at the sides of the path.
    /// </summary>
    /// <param name="x">Row in the map</param>
    /// <param name="z">Column in the map</param>
    /// <param name="Horizontal">If is Horizontal or Vertical the path</param>
    void CreateWalls(int x, int z, bool Horizontal)
    {

        if (Horizontal)
        {
            if (z > 0 && z < m_iSizeZ)
            {
                if (Map[x][z + 1] != DungeonComponents.Floor)
                {
                    Map[x][z + 1] = DungeonComponents.Wall;
                }
                if (Map[x][z - 1] != DungeonComponents.Floor)
                {
                    Map[x][z - 1] = DungeonComponents.Wall;
                }
            }
            else if (z >= 0)
            {
                if (Map[x][z + 1] != DungeonComponents.Floor)
                {
                    Map[x][z + 1] = DungeonComponents.Wall;
                }
            }
            else
            {
                if (Map[x][z - 1] != DungeonComponents.Floor)
                {
                    Map[x][z - 1] = DungeonComponents.Wall;
                }
            }

        }
        else
        {
            if (x > 0 && x < m_iSizeX)
            {
                if (Map[x + 1][z] != DungeonComponents.Floor)
                {
                    Map[x + 1][z] = DungeonComponents.Wall;
                }
                if (Map[x - 1][z] != DungeonComponents.Floor)
                {
                    Map[x - 1][z] = DungeonComponents.Wall;
                }

            }
            else if (x >= 0)
            {
                if (Map[x + 1][z] != DungeonComponents.Floor)
                {
                    Map[x + 1][z] = DungeonComponents.Wall;
                }
            }
            else
            {
                if (Map[x - 1][z] != DungeonComponents.Floor)
                {
                    Map[x - 1][z] = DungeonComponents.Wall;
                }
            }

        }
    }

    /// <summary>
    /// This is the part where the visual map is created. The <see cref="PrefabFloor"/> will replace the floor in the map and the <seealso cref="PrefabWall"/> the walls.
    /// </summary>
    void InstantiatePrefabs()
    {
        for(int i = 0; i < Map.Length;i++)
        {
            for (int j = 0; j < Map[i].Length; j++)
            {
                if(Map[i][j] == DungeonComponents.Floor)
                {
                    Vector3 position = new Vector3(transform.position.x + 2 * i, transform.position.y, transform.position.z + 2 * j);
                    GameObject obj = Instantiate(PrefabFloor, position, transform.rotation) as GameObject;
                    obj.transform.parent = transform;
                    obj.name += "x: " + i + " | z: " + j;
                }else if (Map[i][j] == DungeonComponents.Wall)
                {
                    Vector3 position = new Vector3(transform.position.x + 2 * i, transform.position.y, transform.position.z + 2 * j);
                    GameObject obj = Instantiate(PrefabWall, position, transform.rotation) as GameObject;
                    obj.transform.parent = transform;
                    obj.name += "x: " + i + " | z: " + j;
                }

            }
        }
    }

   
}
