using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class represents an edge from a region to an item location and records what item is placed in that location
    class Location
    {
        public string Name;
        public string Requirements; //Key item requirements to traverse this edge
        public Item Item = new Item(); //What item is placed in this location
    }
}
