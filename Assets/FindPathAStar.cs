using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker //this class will basically be keeping track of all of the map positions
                        //���� ����� �� ���� ����� ����������� ��� ������� �� �����.
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
    bool done = false; //������� ������ ��������, ����� �� ��������� �� ����� ����.


    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    void RemoveAllMarkers() //����� ������� �� � ����� �������� �������
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

        //������ ������ � ��������� ��� ������������ ��� ��� ����� ���������,
        //������� �� �������� ������, � ��������� ������. ����������� ���� ������, ��������� Shuffle(),
        //� ����� ������ �������� �� ������.
        List<MapLocation> locations = new List<MapLocation>();
        //����� �� ���������� ������� ��������� ����� ��� ������ ����� �����.
        for (int z = 1; z < maze.depth - 1; z++)
        {
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x, z] != 1) //�� ���� �� �����
                {
                    locations.Add(new MapLocation(x, z)); //��������� ������� � ����
                }
            }
        }
        //��� ���� ��� ���� ��������, ����������� � ������� ����� ����� ������,
        //������� �� ������ ������� � ������������ � �������� ��������� ��������������,
        //� ����� � �������� ���������� ��������������.
        locations.Shuffle();

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0,
                                            Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0,
                                            Instantiate(end, goalLocation, Quaternion.identity), null);
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
    }
}
