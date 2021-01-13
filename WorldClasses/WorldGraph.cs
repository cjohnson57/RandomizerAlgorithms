using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace RandomizerAlgorithms
{
    //This is the top-level class that contains all the information about the game world
    class WorldGraph
    {
        public string StartRegionName; //Specifies which region to begin traversing the world in
        public string GoalItemName; //Specifies the name of the item which the game is considered finished after collecting
        public HashSet<Region> Regions; //Specifies all regions in the game
        public List<Item> Items; //Specifies the list of items, including the goal item, major/key items, and minor items

        //Empty constructpr
        public WorldGraph() { }

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

        //Every parameter specified
        //Used in world generation
        public WorldGraph(string start, string goal, HashSet<Region> regions, List<Item> items)
        {
            StartRegionName = start;
            GoalItemName = goal;
            Regions = regions;
            Items = items;
        }

        //Copy all properties of graph and return
        //Used so that input graph is not copied by reference and overwritten
        public WorldGraph Copy()
        {
            WorldGraph copy = new WorldGraph();
            copy.StartRegionName = StartRegionName;
            copy.GoalItemName = GoalItemName;
            copy.Regions = new HashSet<Region>();
            foreach(Region r in Regions)
            {
                Region rcopy = new Region();
                rcopy.Name = r.Name;
                rcopy.Exits = new HashSet<Exit>();
                foreach(Exit e in r.Exits)
                {
                    Exit ecopy = new Exit();
                    ecopy.ToRegionName = e.ToRegionName;
                    ecopy.Requirements = e.Requirements;
                    rcopy.Exits.Add(e);
                }
                rcopy.Locations = new HashSet<Location>();
                foreach(Location l in r.Locations)
                {
                    Location lcopy = new Location();
                    lcopy.Name = l.Name;
                    lcopy.Requirements = l.Requirements;
                    Item icopy = new Item();
                    icopy.Importance = l.Item.Importance;
                    icopy.Name = l.Item.Name;
                    lcopy.Item = icopy;
                    rcopy.Locations.Add(lcopy);
                }
                copy.Regions.Add(rcopy);
            }
            copy.Items = new List<Item>();
            foreach(Item i in Items)
            {
                Item icopy = new Item();
                icopy.Importance = i.Importance;
                icopy.Name = i.Name;
                copy.Items.Add(icopy);
            }
            return copy;
        }

        //Gets all locations in the graph which have not yet been filled by an item
        public List<Location> GetAllEmptyLocations()
        {
            List<Location> locations = new List<Location>();
            foreach(Region r in Regions)
            {
                foreach(Location l in r.Locations)
                {
                    if(l.Item.Importance == -1) //-1 signifies empty location
                    {
                        locations.Add(l); //Add to empty location list
                    }
                }
            }
            return locations;
        }

        //Overwrites a location in the worldgraph to place the specified item in that location
        public void Place(Location location, Item item)
        {
            Regions.First(x => x.Locations.Count(x => x.Name == location.Name) == 1).Locations.First(x => x.Name == location.Name).Item = item;
        }

        //Gets all items currently placed in a location in the graph
        public List<Item> CollectAllItems()
        {
            List<Item> items = new List<Item>();
            foreach (Region r in Regions)
            {
                foreach (Location l in r.Locations)
                {
                    if (l.Item.Importance > -1) //-1 signifies empty location, so if true this location is non empty
                    {
                        items.Add(l.Item); //Add to list of placed items
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
                    if (l.Item.Importance >= 2) //2 signifies major item, 3 is goal item
                    {
                        items.Add(l.Item); //Add to list of major items
                    }
                }
            }
            return items;
        }

        //Find the list of locations which are unreachable
        //Used for world generation to make sure all nodes are connected
        public List<Region> GetUnreachableRegions()
        {
            Search searcher = new Search();
            List<Region> unreachable = new List<Region>();
            foreach(Region r in Regions)
            {
                if(r.Name != StartRegionName) //Don't check start region
                {
                    List<List<Region>> paths = searcher.PathsToRegion(this, r); //List of all paths from start to r
                    if(paths.Count == 0) //If there are no paths, then this location is unreachable, so add it to the unreachable list
                    {
                        unreachable.Add(r);
                    }
                }
            }
            return unreachable;
        }

        //Convert the world to a json representation and return
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        //Get total location count
        public int GetLocationCount()
        {
            int locationcount = 0;
            foreach (Region r in Regions) //Count locations in each region and sum
            {
                locationcount += r.Locations.Count();
            }
            return locationcount;
        }
    }
}
