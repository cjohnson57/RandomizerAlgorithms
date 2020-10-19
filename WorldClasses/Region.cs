using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class represents the regions of the world, each with their own exits to other regions and potential item locations
    class Region
    {
        public string Name;
        public HashSet<Exit> Exits; //List of edges to other regions
        public HashSet<Location> Locations; //List of edges to item locations


        public Region()
        { 

        }

        //Constructor with specified name
        //Used when constructing partial graph of world
        public Region(Region r)
        {
            Name = r.Name;
            Exits = new HashSet<Exit>();
            Locations = new HashSet<Location>();
        }
    }
}
