using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class implements the searching methods used to compute reachability and trace a path through the game world
    class Search
    {
        Helpers helper;
        Parser parser;

        public Search()
        {
            helper = new Helpers();
            parser = new Parser();
        }

        //Constructor with pre-specified helper so it may be seeded
        public Search(Helpers h)
        {
            helper = h;
            parser = new Parser();
        }

        //Utilizes BFS search algorithm to find all locations in the world which are reachable with the current item set
        //Important note is that throughout this function the owned items are static, they are not collected throughout (as they are in sphere search)
        public WorldGraph GetReachableLocations(WorldGraph world, List<Item> owneditems)
        {
            WorldGraph reachable = new WorldGraph(world);
            Region root = world.Regions.First(x => x.Name == world.StartRegionName);
            Queue<Region> TraverseOrder = new Queue<Region>();
            Queue<Region> Q = new Queue<Region>();
            HashSet<Region> S = new HashSet<Region>();
            Q.Enqueue(root);
            S.Add(root);

            //Implementation of BFS
            while(Q.Count > 0)
            {
                Region r = Q.Dequeue();
                Region toadd = new Region(r);
                TraverseOrder.Enqueue(r);

                foreach(Exit e in r.Exits)
                {
                    //Normally in BFS, all exits would be added
                    //But in this case, we only want to add exits which are reachable
                    if(parser.RequirementsMet(e.Requirements, owneditems))
                    {
                        toadd.Exits.Add(e);
                        Region exitto = world.Regions.First(x => x.Name == e.ToRegionName);
                        if(!S.Contains(exitto))
                        {
                            Q.Enqueue(exitto);
                            S.Add(exitto);
                        }
                    }
                }
                //Subsearch to check each edge to a location in the current region
                //If requirement is met, ad it to reachable locations
                foreach(Location l in r.Locations)
                {
                    if(parser.RequirementsMet(l.Requirements, owneditems))
                    {
                        toadd.Locations.Add(l);
                    }
                }
                reachable.Regions.Add(toadd); //Add every reachable exit and location discovered in this iteration
            }
            return reachable;
        }

        /*
         * Sphere Search is done iteratively in “Spheres” and is used to attempt to trace a path
         * from the beginning to the end of the game. The first sphere s is simply all locations which are
         * reachable from the beginning of the game. As it searches these locations, it adds key items
         * found to a temporary set; we do not want those items to affect reachability until the next sphere
         * iteration so we do not yet add them to I. After all reachable locations have been found, sphere s
         * is added to the list of spheres S, and all items in the temporary set are added to I. It then
         * iterates again with a new sphere s.
         */
        public SphereSearchOutput SphereSearch(WorldGraph world)
        {
            SphereSearchOutput output = new SphereSearchOutput();
            output.Spheres = new List<WorldGraph>();
            List<Item> owneditems = new List<Item>();
            //Initial sphere s0 includes items reachable from the start of the game
            WorldGraph s0 = GetReachableLocations(world, owneditems);
            owneditems = s0.CollectMajorItems(); //College all major items reachable from the start of the game
            output.Spheres.Add(s0); //Add initial sphere to sphere list
            //sx indicates every sphere after the first. Any major items found in s0 means sx should be bigger.
            WorldGraph sx = GetReachableLocations(world, owneditems);
            int temp = owneditems.Count(); //Temp is the count of previously owned major items
            owneditems = sx.CollectMajorItems(); //This is the new count of owned major items
            //If counts are equal then no new major items found, stop searching
            while (owneditems.Count > temp) //If new count is not greater than old count, that means all currently reachable locations have been found
            {
                output.Spheres.Add(sx); //If new locations found, add to sphere list
                //Take the same steps taken before the loop: Get new reachable locations, collect new major items, and check to see if new count is larger than old count
                sx = GetReachableLocations(world, owneditems);
                temp = owneditems.Count();
                owneditems = sx.CollectMajorItems();
            }
            //At this point, either a dead end has been found or the end of the game has
            //If the goal item is in the list of owned items, means the end has been found and thus the game is completable
            output.Completable = owneditems.Count(x => x.Name == world.GoalItemName) > 0;
            return output;
        }
    }

    //Class to record list of spheres and completability bool
    struct SphereSearchOutput
    {
        public List<WorldGraph> Spheres;
        public bool Completable;
    }
}
