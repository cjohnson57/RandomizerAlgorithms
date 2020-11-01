using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RandomizerAlgorithms
{
    class WorldGenerator
    {
        private static Random rng;
        private static Helpers Helper;

        private static int Regions;
        private static List<Item> MajorItemList;

        public WorldGraph Generated;


        //Constructor which only specifies a number of items which are then generated
        public WorldGenerator(int regions, int items)
        {
            rng = new Random();
            Helper = new Helpers();
            Regions = regions;
            MajorItemList = GenItemList(items);
            Generated = new WorldGraph();
        }

        //Constructor which specifies an item list
        public WorldGenerator(int regions, List<Item> itemlist, int seed)
        {
            rng = new Random(seed);
            Helper = new Helpers(seed);
            Regions = regions;
            MajorItemList = itemlist;
            Generated = new WorldGraph();
        }

        //Generate a random worldgraph using the specified number of regions and item list
        public WorldGraph Generate()
        {
            HashSet<Region> regions = new HashSet<Region>();
            for (int i = 0; i < Regions; i++)
            {
                Region r = new Region("Region-" + i.ToString()); //Each region is named Region_x. So Region-1, Region-2, etc.
                regions.Add(r);
            }
            //Not must loop through each region to add exits
            //Separate loop from the previous so that all regions are available to add as exits
            foreach(Region r in regions)
            {
                //The first region has some specific conditions:
                // 1. First exit is guaranteed to have no requirement and goes to hub region
                // 2. There is a guaranteed second exit, that will have a single item requirement
                // 3. There is a 50% chance to have a third exit, which has a 50% chance between a single item and no item
                if(r.Name == "Region-0")
                {
                    //Add exit to hub region with no requirement
                    List<string> currentexits = new List<string>();
                    Region hub = regions.First(x => x.Name == "Region-1");
                    AddExitsNoRequirement(regions, r, hub);
                    currentexits.Add("Region-1");
                    //Add exit to 2nd region with single requirement
                    Region second = GetRandomAvailableRegion(regions, r, currentexits);
                    AddExitsOneRequirement(regions, r, second);
                    currentexits.Add(second.Name);
                    //50% chance to add a third region
                    int random = rng.Next(1, 3); //Either 1 or 2
                    if(random == 2)
                    {
                        Region third = GetRandomAvailableRegion(regions, r, currentexits);
                        random = rng.Next(1, 3);
                        //50% chance to have 1 requirement, 50% chance to have none
                        if(random == 2)
                        {
                            AddExitsNoRequirement(regions, r, third);
                        }
                        else
                        {
                            AddExitsOneRequirement(regions, r, third);

                        }
                    }
                }
                //The second region is the hub region and also has some specific conditions:
                // 1. Will connect to 5 regions besides the start region
                // 2. Half of its exits will have no item requirement, the other half will have one
                else if (r.Name == "Region-1")
                {
                    List<string> currentexits = new List<string>();
                    currentexits.Add("Region-0");
                    for(int i = 0; i < 5; i++) //Run for 5 iterations
                    {
                        Region to = GetRandomAvailableRegion(regions, r, currentexits);
                        if(i < 2) //First 3 exits (including start region) have no requirement
                        {
                            AddExitsNoRequirement(regions, r, to);
                        }
                        else //Next 3 iterations will have 1 requirement
                        {
                            AddExitsOneRequirement(regions, r, to);

                        }
                        currentexits.Add(to.Name);
                    }
                }
                //Every other region will have a number of exits in [1, 4], however max of 2 chosen at generation, 2 more can be added by a later region
                else
                {
                    int ExitNum = rng.Next(1, 3); //Generate random number in [1, 2]
                    //In case r already has exits, create a list which contains all its current exits
                    List<string> currentexits = new List<string>();
                    foreach(Exit e in r.Exits)
                    {
                        currentexits.Add(e.ToRegionName);
                    }
                    while(r.Exits.Count < ExitNum) //Possible that location already has specified number of exits, no big deal if so
                    {
                        Region to = GetRandomAvailableRegion(regions, r, currentexits);
                        if(!string.IsNullOrEmpty(to.Name))
                        {
                            AddExits(regions, r, to); //Add exit from r to the random region
                            currentexits.Add(to.Name); //Also add dest region to list so it does not get added twice
                        }
                        else //Don't want to do this if r has 0 exits, but that logic is handled in GetRandomAvailableRegion
                        {
                            break;
                        }
                    }
                }

            }
            //Must make sure all locations are reachable
            Generated = new WorldGraph("Region-0", "Goal", regions.ToHashSet(), MajorItemList);
            List<Region> unreachable = Generated.GetUnreachableRegions();
            while(unreachable.Count > 0) //At least one reachable location
            {
                //Create a connection from a random reachable location to a random unreachable location
                List<Region> regionscopy = regions.ToList();
                Helper.Shuffle(regionscopy);
                Region from = regionscopy.First(x => !unreachable.Contains(x)); //Not in unreachable, so it is reachable
                Helper.Shuffle(unreachable);
                Region to = unreachable.First(); //Unreachable
                AddExits(regions, from, to); //Add connection between two regions to join subgraphs
                Generated = new WorldGraph("Region-0", "Goal", regions.ToHashSet(), MajorItemList);
                unreachable = Generated.GetUnreachableRegions(); //Recompute reachability 
            }
            //Now before adding items, we will find the region which is farthest from the start and place the goal there- No other items will be placed there
            Generated = new WorldGraph("Region-0", "Goal", regions.ToHashSet(), MajorItemList);
            Region goalregion = Generated.GetFarthestRegion();
            Item goalitem = new Item("Goal", 3); //Create goal item
            Location goallocation = new Location("Final Boss", GenerateComplexRandomRequirement(), goalitem); //Create location for goal item with complex requirement
            regions.First(x => x == goalregion).Locations.Add(goallocation);
            //Finally, generate item locations and place the location in the region
            foreach(Region r in regions)
            {
                //The first region has some specific conditions:
                // 1. Three locations with no requirement
                // 2. 50% chance of a 4th location with one requirement
                if (r.Name == "Region-0")
                {
                    int random = rng.Next(3, 5);
                    for(int i = 0; i < random; i++)
                    {
                        if(i < random - 1) //Guaranteed 3 locations with no requirement
                        {
                            Location l = new Location("Region-0_Location-" + i.ToString(), "None", new Item());
                            regions.First(x => x == r).Locations.Add(l); //Add generated location to region
                        }
                        else //Possible 4th location, have 1 requirement
                        {
                            Location l = new Location("Region-0_Location-" + i.ToString(), GenerateOneRandomRequirement(), new Item());
                            regions.First(x => x == r).Locations.Add(l); //Add generated location to region
                        }
                    }
                }
                //The second region is the hub region and also has some specific conditions:
                // 1. Two locations with no requirement
                // 2. One location with one requirement
                // 3. One location with two requirements
                else if (r.Name == "Region-1")
                {
                    for(int i = 0; i < 4; i++)
                    {
                        if (i < 2)
                        {
                            Location l = new Location("Region-1_Location-" + i.ToString(), "None", new Item());
                            regions.First(x => x == r).Locations.Add(l); //Add generated location to region
                        }
                        else if (i == 2)
                        {
                            Location l = new Location("Region-1_Location-" + i.ToString(), GenerateOneRandomRequirement(), new Item());
                            regions.First(x => x == r).Locations.Add(l); //Add generated location to region
                        }
                        else if (i == 3)
                        {
                            Location l = new Location("Region-1_Location-" + i.ToString(), GenerateTwoRandomRequirements(), new Item());
                            regions.First(x => x == r).Locations.Add(l); //Add generated location to region
                        }
                    }
                }
                //Every other region will generate 2 to 4 locations, unless region contains goal, in which case we want that to be the only location in that region
                else if (r != goalregion)
                {
                    //Generate 2 to 4 locations per region
                    int random = rng.Next(2, 5);
                    for (int i = 0; i < random; i++)
                    {
                        //Generate a location with:
                        // Name: Region-x_Location-y, ex Region-5_Location-2
                        // Requirement: Randomly Generated
                        // Item: null item
                        Location l = new Location(r.Name + "_Location-" + i.ToString(), GenerateRandomRequirement(), new Item());
                        regions.First(x => x == r).Locations.Add(l); //Add generated location to region
                    }
                }
            }
            //Now that we have a total number of regions and a count of major items, must generate junk items to fill out the item list
            List<Item> ItemList = MajorItemList; //Copy major item list and add goal item
            ItemList.Add(goalitem);
            Generated = new WorldGraph("Region-0", "Goal", regions.ToHashSet(), ItemList.OrderByDescending(x => x.Importance).ThenBy(x => x.Name).ToList()); //Remake generated now that items have been added
            int locationcount = Generated.GetLocationCount(); //Get location count and find difference so we know how many junk items to generate
            int difference = locationcount - MajorItemList.Count();
            for (int i = 0; i < difference; i++)
            {
                //For a junk item, importance will be either 0 or 1, so generate one of those numbers randomly
                int importance = rng.Next(0, 2);
                Item newitem = new Item("JunkItem" + importance.ToString(), importance); //Name will either be JunkItem0 or JunkItem1
                ItemList.Add(newitem);
            }
            Generated = new WorldGraph("Region-0", "Goal", regions.ToHashSet(), ItemList.OrderByDescending(x => x.Importance).ThenBy(x => x.Name).ToList()); //Remake generated now that items have been added
            return Generated;
        }

        //Adds a new exit with the specified target region name to the specified from region, and vice versa
        //This function makes sure exit has no requirement
        private void AddExitsNoRequirement(HashSet<Region> regions, Region from, Region to)
        {
            //Use constructor that uses default none value for requirement
            Exit e1 = new Exit(to.Name); //Exit to be appended to the "from" region
            Exit e2 = new Exit(from.Name); //Exit to be appended to the "to" region
            regions.First(x => x == from).Exits.Add(e1);
            regions.First(x => x == to).Exits.Add(e2);
        }

        //Adds a new exit with the specified target region name to the specified from region, and vice versa
        //This function makes sure exit has a single requirement
        private void AddExitsOneRequirement(HashSet<Region> regions, Region from, Region to)
        {
            string requirement = GenerateOneRandomRequirement(); //For simplicity we will use same requirement both ways
            //Use constructor that specifies requirement, get requirement from one requirement function
            Exit e1 = new Exit(to.Name, requirement); //Exit to be appended to the "from" region
            Exit e2 = new Exit(from.Name, requirement); //Exit to be appended to the "to" region
            regions.First(x => x == from).Exits.Add(e1);
            regions.First(x => x == to).Exits.Add(e2);
        }

        //Adds a new exit with the specified target region name to the specified from region, and vice versa
        //This function generates a completely random requirement
        private void AddExits(HashSet<Region> regions, Region from, Region to)
        {
            string requirement = GenerateRandomRequirement(); //For simplicity we will use same requirement both ways
            //Use constructor that specifies requirement, get requirement from random requirement function
            Exit e1 = new Exit(to.Name, requirement); //Exit to be appended to the "from" region
            Exit e2 = new Exit(from.Name, requirement); //Exit to be appended to the "to" region
            regions.First(x => x == from).Exits.Add(e1);
            regions.First(x => x == to).Exits.Add(e2);
        }

        //Simply return a random item name from the major item list
        private string GenerateOneRandomRequirement()
        {
            Helper.Shuffle(MajorItemList); //Shuffle major item list to provide randomness
            return MajorItemList.First().Name;
        }

        //Return a string with two items, joined by either an "and" or an "or"
        private string GenerateTwoRandomRequirements()
        {
            string item1 = GenerateOneRandomRequirement();
            string item2 = GenerateOneRandomRequirement();
            while (item2 == item1) //Make sure items are not equal
            {
                item2 = GenerateOneRandomRequirement();
            }
            //50% chance to join with "or", 50% chance to join with "and"
            string andor = GetAndOrOr();
            //Put pieces together and return string
            return item1 + andor + item2;
        }

        //Return a string with three items, each can be joined by "and" or "or"
        private string GenerateThreeRandomRequirements()
        {
            string firstpart = GenerateTwoRandomRequirements();
            string secondpart = GenerateOneRandomRequirement();
            while(firstpart.Contains(secondpart)) //Make sure first part of the string doesn't already contain new string
            {
                secondpart = GenerateOneRandomRequirement();
            }
            //50% chance to join with "or", 50% chance to join with "and"
            string andor = GetAndOrOr();
            //Put pieces together and return string
            return firstpart + andor + secondpart;
        }

        //Generate a random requirement of variable complexity
        private string GenerateRandomRequirement()
        {
            //12.5% chance to have no requirement
            int random = rng.Next(1, 9);
            if(random == 1)
            {
                return "None";
            }
            //50% chance at this point to be a single requirement (43.75% total)
            random = rng.Next(1, 3);
            if(random == 2)
            {
                return GenerateOneRandomRequirement();
            }
            //50% chance at this point to have 2 requirements (21.875% total)
            random = rng.Next(1, 3);
            if(random == 2)
            {
                return GenerateTwoRandomRequirements();
            }
            random = rng.Next(1, 3);
            //50% chance at this point to have 3 requirements (10.9375% total)
            if (random == 2)
            {
                return GenerateThreeRandomRequirements();
            }
            //50% chance at this point to have a complex requirement (10.9375% total)
            else
            {
                return GenerateComplexRandomRequirement();
            }

        }

        //Specifically attempts to generate a complex requirement string
        //It wil consist of 2-3 portions which each consist of 1-3 requirements
        //Portions will be surrounded by parenthesis and joined with and or or
        //Redundancy in item requirement between portions will not be checked
        private string GenerateComplexRandomRequirement()
        {
            string requirement = "";
            //50% chance of 2 portions, 50% chance of 3
            int portions = rng.Next(2, 4);
            for(int i = 0; i < portions; i++)
            {
                string portion = "";
                //20% chance of 1 requirement, 40% chance of 2, 40% chance of 3 per portion
                int random = rng.Next(1, 6);
                if(random == 1) //Get one requirement
                {
                    portion = GenerateOneRandomRequirement();
                }
                else if(random > 1 && random < 4) //Get two requirements
                {
                    portion = GenerateTwoRandomRequirements();
                }
                else //Get three requirements
                {
                    portion = GenerateThreeRandomRequirements();
                }
                requirement += "(" + portion + ")"; //Add portion surrounded by parenthesis
                if(i < portions - 1) //If not the last portion, add "and" or "or" between this portion and the next
                {
                    requirement += GetAndOrOr();
                }
            }
            return requirement;
        }

        //Get an available region
        private Region GetRandomAvailableRegion(HashSet<Region> regions, Region r, List<string> currentexits)
        {
            //Do not want start region, hub region, self, or any region which there is already an exit from r to; enforce at most 4 exits per region
            List<Region> Available = regions.Where(x => x.Name != "Region-0" && x.Name != "Region-1" && x != r && !currentexits.Contains(x.Name) && x.Exits.Count < 4).ToList();
            if (Available.Count == 0 && r.Exits.Count > 0) //No regions with less than 4 exits, just break, UNLESS this region has no exits, must have at least 1 (no disconnected nodes!)
            {
                return new Region(); //Will indicate to calling code that it should break
            }
            else if (r.Exits.Count == 0) //Handle case when r has 0 exits
            {
                Available = regions.Where(x => x.Name != "Region-0" && x.Name != "Region-1" && x != r).ToList(); //Looser region requirements
            }
            Helper.Shuffle(Available); //Shuffle list
            return Helper.Pop(Available); //Get random region from list
        }

        //Has a 50% chance to return "or", 50% chance to return "and"
        private string GetAndOrOr()
        {
            string andor = " or ";
            int random = rng.Next(1, 3);
            if (random == 2)
            {
                andor = " and ";
            }
            return andor;
        }

        private List<Item> GenItemList(int count)
        {
            List<Item> list = new List<Item>();
            for(int i = 0; i < count; i++)
            {
                Item item = new Item(Helper.NumToLetters(i), 2); //Generate major item with a name of an incrementing letter string. A, B.... Z, AA, AB, etc.
                list.Add(item); //Add to list
            }
            return list;
        }

        public double GetComplexity()
        {
            Statistics stats = new Statistics();
            return stats.CalcWorldComplexity(Generated);
        }

    }
}
