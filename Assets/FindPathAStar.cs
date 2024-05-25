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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
