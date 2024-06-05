using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker //this class will basically be keeping track of all of the map positions
                        //этот класс по сути будет отслеживать все позиции на карте.
{
    public MapLocation location; //its location on map
    public float G, H, F;
    public GameObject marker; //which will be a physical marker itself
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject m, PathMarker p)
    //make constructor and we will pass through the map location l
    //also, we're going to have a path marker for p to pass through the parent
    {
        location = l;
        G = g;
        H = h;
        F = f;
        marker = m;
        parent = p;
    }

    public override bool Equals(object obj)
    //We also are going to have to compare if one path marker is the same as another path marker
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.Equals(((PathMarker)obj).location); //you can just go location, which is your map location equals.
        }
    }
    public override int GetHashCode()
    {
        return 0;
    }
}

public class FindPathAStar : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;
    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    PathMarker goalNode;
    PathMarker startNode;
    PathMarker lastPos;
    bool done = false; //которое станет истинным, когда мы доберемся до конца пути.


    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    void RemoveAllMarkers() //чтобы удалить их с карты нажатием клавиши
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject m in markers)
        {
            Destroy(m);
        }
    }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        //Лучший способ — поместить все пространства или все места лабиринта,
        //которые не являются стеной, в небольшой список. Перетасуйте этот список, используя Shuffle(),
        //а затем просто выберите их сверху.
        List<MapLocation> locations = new List<MapLocation>();
        //Затем мы собираемся создать вложенные циклы для обхода нашей карты.
        for (int z = 1; z < maze.depth - 1; z++)
        {
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x, z] != 1) //то есть не стена
                {
                    locations.Add(new MapLocation(x, z)); //добавляем локацию в лист
                }
            }
        }
        //Это даст нам пару значений, находящихся в верхней части этого списка,
        //которые мы сможем выбрать и использовать в качестве конечного местоположения,
        //а также в качестве начального местоположения.
        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0,
                                            Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0,
                                            Instantiate(end, goalLocation, Quaternion.identity), null);

        //after this we can start search
        open.Clear();
        closed.Clear();
        open.Add(startNode);
        lastPos = startNode;
    }

    void Search(PathMarker thisNode)
    {
        //to make sure that pressin C do nothing
        if (thisNode == null)
        {
            return;
        }
        //test if we hit the goal
        if (thisNode.Equals(goalNode))
        {
            done = true;
            return;
        } //goal has been found

        //loop through and find all neighbours
        /*public List<MapLocation> directions = new List<MapLocation>() {
                                            new MapLocation(1,0),
                                            new MapLocation(0,1),
                                            new MapLocation(-1,0),
                                            new MapLocation(0,-1) };*/
        foreach (MapLocation dir in maze.directions)
        {
            //neighbur is just a position on map!
            MapLocation neighbour = dir + thisNode.location;
            //если стена - продолжаем
            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            //если выходит за края лабиринта(от 1 да 30)
            if (neighbour.x < 1 || neighbour.x >= maze.width || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            //if neighbour in the closed list
            if (IsClosed(neighbour)) continue;

            //calculating 
            //по теореме Пифагора находим расстояние от данной ноды до его соседа
            float G = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float F = G + H;

            //create object to put on this path
            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0, neighbour.z * maze.scale), Quaternion.identity);

            //показать значения G,H,F
            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();

            values[0].text = "G: " + G.ToString("0.00");
            values[1].text = "H: " + H.ToString("0.00");
            values[2].text = "F: " + F.ToString("0.00");

            if (!UpdateMarker(neighbour, G, H, F, thisNode))
            {
                open.Add(new PathMarker(neighbour, G, H, F, pathBlock, thisNode));
            }
        }
    }

    //проверяем есть и объект в open list
    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker)
    {
        foreach (PathMarker p in closed)
        {
            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            BeginSearch();
        }
        if(Input.GetKeyDown(KeyCode.C) && !done)
        {
            Search(lastPos);
        }
    }
}
