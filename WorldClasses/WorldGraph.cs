using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace RandomizerAlgorithms
{
    //This is the top-level class that contains all the information about the game world
    class WorldGraph
    {
        public string StartRegionName; //Specifies which region to begin traversing the world in
        public string GoalItemName; //Specifies the name of the item which the game is considered finished after collecting
        public HashSet<Region> Regions; //Specifies all regions in the game
        public List<Item> Items; //Specifies the list of items, including the goal item, major/key items, and minor items

        public WorldGraph()
        {

        }

        //Constructor from a previous world graph
        //Copies the start region and goal but new items and regions
        //Used when constructing partial graph of world
        public WorldGraph(WorldGraph world)
        {
            StartRegionName = world.StartRegionName;
            GoalItemName = world.GoalItemName;
            Regions = new HashSet<Region>();
            Items = new List<Item>();
        }

        //Gets all locations in the graph which have not yet been filled by an item
        public List<Location> GetAllEmptyLocations()
        {
            List<Location> locations = new List<Location>();
            foreach(Region r in Regions)
            {
                foreach(Location l in r.Locations)
                {
                    if(l.Item.Importance == -1)
                    {
                        locations.Add(l);
                    }
                }
            }
            return locations;
        }

        //Gets all items currently placed in a location in the graph
        public List<Item> CollectAllItems()
        {
            List<Item> items = new List<Item>();
            foreach (Region r in Regions)
            {
                foreach (Location l in r.Locations)
                {
                    if (l.Item.Importance > -1)
                    {
                        items.Add(l.Item);
                    }
                }
            }
            return items;
        }

        //Gets all major items currently placed in a location on the graph
        public List<Item> CollectMajorItems()
        {
            List<Item> items = new List<Item>();
            foreach (Region r in Regions)
            {
                foreach (Location l in r.Locations)
                {
                    if (l.Item.Importance >= 2)
                    {
                        items.Add(l.Item);
                    }
                }
            }
            return items;

        }
    }
}
