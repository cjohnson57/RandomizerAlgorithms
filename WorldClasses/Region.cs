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
            Name = "";
            Exits = new HashSet<Exit>();
            Locations = new HashSet<Location>();
        }

        //Another name constructor but with a string name instead of another region
        //Used in world generation to create new regions
        public Region(string name)
        {
            Name = name;
            Exits = new HashSet<Exit>();
            Locations = new HashSet<Location>();
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
