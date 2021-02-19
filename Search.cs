using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class implements the searching methods used to compute reachability and trace a path through the game world
    class Search
    {

        private Parser parser = new Parser();

        //Utilizes BFS search algorithm to find all locations in the world which are reachable with the current item set
        //Important note is that throughout this function the owned items are static, they are not collected throughout (as they are in sphere search)
        //It is used for forward search, where there is no need to check for items currently within R, as well as other places such as sphere search
        public WorldGraph GetReachableLocations(WorldGraph world, List<Item> owneditems)
        {
            WorldGraph reachable = new WorldGraph(world);
            Region root = world.Regions.First(x => x.Name == world.StartRegionName);
            Queue<Region> Q = new Queue<Region>();
            HashSet<Region> visited = new HashSet<Region>();
            Q.Enqueue(root);
            visited.Add(root);

            //Implementation of BFS
            while (Q.Count > 0)
            {
                Region r = Q.Dequeue();
                Region toadd = new Region(r);

                foreach (Exit e in r.Exits)
                {
                    //Normally in BFS, all exits would be added
                    //But in this case, we only want to add exits which are reachable
                    if (parser.RequirementsMet(e.Requirements, owneditems))
                    {
                        toadd.Exits.Add(e);
                        Region exitto = world.Regions.First(x => x.Name == e.ToRegionName); //Get the region this edge leads to
                        if (!visited.Contains(exitto)) //Don't revisit already visited nodes on this path
                        {
                            Q.Enqueue(exitto);
                            visited.Add(exitto);
                        }
                    }
                }
                //Subsearch to check each edge to a location in the current region
                //If requirement is met, add it to reachable locations
                foreach (Location l in r.Locations)
                {
                    if (parser.RequirementsMet(l.Requirements, owneditems))
                    {
                        toadd.Locations.Add(l);
                    }
                }
                reachable.Regions.Add(toadd); //Add every reachable exit and location discovered in this iteration
            }
            return reachable; //Return graph of reachable locations
        }

        //Utilizes BFS search algorithm to find all locations in the world which are reachable with the current item set
        //In this algorithm, we want to check for items which have been removed from I but are still contained within R, so an initial search is done to collect items, 
        //then repeated iteratively until no new items are found, at which point the final reachability graph is returned.
        public WorldGraph GetReachableLocationsAssumed(WorldGraph world, List<Item> owneditems)
        {
            WorldGraph copy = world.Copy(); //Used so items may be removed from world at will
            List<Item> newitems = ItemSearch(copy, owneditems); //Find items within R
            List<Item> combined = owneditems.ToList(); //Copy list
            while (newitems.Count > 0)
            {
                combined.AddRange(newitems); //Add items to currently used items
                newitems = ItemSearch(copy, combined); //Find items within R
            }
            return GetReachableLocations(world, combined); //Use that combined list to find final search result
        }

        //Very similar to GetReachableLocations except that it returns items found within those locations
        //Used in GetReachableLocationsAssumed
        public List<Item> ItemSearch(WorldGraph world, List<Item> owneditems)
        {
            List<Item> newitems = new List<Item>();
            Region root = world.Regions.First(x => x.Name == world.StartRegionName);
            Queue<Region> Q = new Queue<Region>();
            HashSet<Region> visited = new HashSet<Region>();
            Q.Enqueue(root);
            visited.Add(root);

            //Implementation of BFS
            while (Q.Count > 0)
            {
                Region r = Q.Dequeue();

                foreach (Exit e in r.Exits)
                {
                    //Normally in BFS, all exits would be added
                    //But in this case, we only want to add exits which are reachable
                    if (parser.RequirementsMet(e.Requirements, owneditems))
                    {
                        Region exitto = world.Regions.First(x => x.Name == e.ToRegionName); //Get the region this edge leads to
                        if (!visited.Contains(exitto)) //Don't revisit already visited nodes on this path
                        {
                            Q.Enqueue(exitto);
                            visited.Add(exitto);
                        }
                    }
                }
                //Subsearch to check each edge to a location in the current region
                //If requirement is met, add it to reachable locations
                foreach (Location l in r.Locations)
                {
                    if (parser.RequirementsMet(l.Requirements, owneditems))
                    {
                        if(l.Item.Importance == 2) //If location contains a major item
                        {
                            newitems.Add(l.Item);
                            l.Item = new Item(); //Remove item so it isn't added again in future iterations
                        }
                    }
                }
            }
            return newitems;
        }

        private List<List<Region>> paths = new List<List<Region>>(); //Declared outside of function scope so multiple instances of following two functions can access

        //Use DFS to find all possible paths from the root to the specified region
        //Not including paths that go back on themselves
        public List<List<Region>> PathsToRegion(WorldGraph world, Region dest)
        {
            Region root = world.Regions.First(x => x.Name == world.StartRegionName);

            List<Region> visited = new List<Region>();
            paths = new List<List<Region>>();
            if(root == dest) //If root and dest equal, return empty list
            {
                return paths;
            }

            RecursiveDFSForPathList(world, root, dest, visited); //Recursively run DFS, when dest found add the path to paths var

            return paths; //Return list of paths
        }

        //Recursively check exits with copy of visited list
        //It's done this way so that after the destination or a dead end is met, the code flow "backs up"
        public void RecursiveDFSForPathList(WorldGraph world, Region r, Region dest, List<Region> visited)
        {
            visited.Add(r); //Add to visited list
            if (r == dest)
            {
                paths.Add(visited); //If this is the dest, then visited currently equals a possible path
                return;
            }
            foreach (Exit e in r.Exits)
            {
                Region exitto = world.Regions.First(x => x.Name == e.ToRegionName); //Get the region this edge leads to
                if (!visited.Contains(exitto)) //Don't revisit already visited nodes on this path
                {
                    List<Region> copy = new List<Region>(visited); //If don't do this List is passed by reference, algo doesn't work
                    RecursiveDFSForPathList(world, exitto, dest, copy);
                }
            }

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
        public SphereSearchInfo SphereSearch(WorldGraph world)
        {
            SphereSearchInfo output = new SphereSearchInfo();
            output.Spheres = new List<WorldGraph>();
            List<Item> owneditems = new List<Item>();
            //Initial sphere s0 includes items reachable from the start of the game
            WorldGraph s0 = GetReachableLocations(world, owneditems);
            owneditems = s0.CollectMajorItems(); //College all major items reachable from the start of the game
            output.Spheres.Add(s0); //Add initial sphere to sphere list
            //sx indicates every sphere after the first. Any major items found in s0 means sx should be bigger.
            WorldGraph sx = GetReachableLocations(world, owneditems);
            int temp = owneditems.Where(x => x.Importance >= 2).Count(); //Temp is the count of previously owned major items
            owneditems = sx.CollectAllItems(); //Used to find new count of major items
            //If counts are equal then no new major items found, stop searching
            while (owneditems.Where(x => x.Importance >= 2).Count() > temp) //If new count is not greater than old count, that means all currently reachable locations have been found
            {
                output.Spheres.Add(sx); //If new locations found, add to sphere list
                //Take the same steps taken before the loop: Get new reachable locations, collect new major items, and check to see if new count is larger than old count
                sx = GetReachableLocations(world, owneditems);
                temp = owneditems.Where(x => x.Importance >= 2).Count(); //Only want to consider count of major items
                owneditems = sx.CollectAllItems();
            }
            //At this point, either a dead end has been found or all locations have been discovered
            //If the goal item is in the list of owned items, means the end has been found and thus the game is completable
            output.Completable = owneditems.Count(x => x.Name == world.GoalItemName) > 0;
            return output;
        }

        /*
         * The goal of this function is to traverse the game world like a player of the game rather than like an algorithm.
         * It's assumed that the player has decent knowledge of the game, meaning they know where locations are and
         * wether or not those locations and regions are accessible.
         * Therefore a heuristic is used to score each posisble exit a player could take based on number of item locations
         * and how close they are to the current location.
         * Whichever exit has the maximum score (meaning maximum potential to gain new items) is taken.
         * We keep several counts which can be used to gauge interestingness of a seed:
         * 1. Number of locations collected for each region traversed
         * 2. Number of regions traversed between finding major or helpful items
         * 3. Number of regions traversed between finding major items
         */
        public PlaythroughInfo PlaythroughSearch(WorldGraph world)
        {
            List<int> BetweenMajorOrHelpfulList = new List<int>(); //This did not end up being utilized.
            List<int> BetweenMajorList = new List<int>();
            List<int> LocationsPerTraversal = new List<int>();
            List<int> LocationsUnlockedPerMajorFound = new List<int>();

            Region current = world.Regions.First(x => x.Name == world.StartRegionName);
            Region previous = new Region();
            List<Item> owneditems = new List<Item>();
            List<Region> traversed = new List<Region>();

            int BetweenMajorOrHelpfulCount = 0;
            int BetweenMajorCount = 0;
            bool gomode = false;
            int prevlocationsunlocked = GetReachableLocations(world, owneditems).GetLocationCount(); //Initial count of unlocked locations
            int initial = prevlocationsunlocked; //Saved to add to output later
            while(owneditems.Count(x => x.Importance == 3) < 1) //Loop until goal item found, or dead end reached (see break statement below)
            {
                //If count gets this high usually indicates search is stuck in a loop... not great but just retry with a new permutation...
                //Seems to happen roughly one in every 5000 permutations of most complex world (World5) so fairly rare occurrence
                if (traversed.Count > world.Regions.Count() * 20)
                {
                    throw new Exception();
                }
                traversed.Add(current);
                int checkcount = 0;
                int prevcount = -1;
                while(prevcount < owneditems.Count()) //While loop to re-check locations if major item is found in this region
                {
                    prevcount = owneditems.Count();
                    //First, check each location in the current region, if accessible and not already searched check it for major items
                    foreach (Location l in current.Locations)
                    {
                        if (l.Item.Importance > -1 && parser.RequirementsMet(l.Requirements, owneditems))
                        {
                            checkcount++; //Add to check count
                            Item i = l.Item;
                            l.Item = new Item(); //Remove item, location importance set to -1
                            if (i.Importance == 1) //Helpful item, did not end up being utilized.
                            {
                                //Update helpful list only and reset counts
                                BetweenMajorOrHelpfulList.Add(BetweenMajorOrHelpfulCount);
                                BetweenMajorOrHelpfulCount = 0;
                            }
                            else if (i.Importance == 2) //Major item
                            {
                                owneditems.Add(i); //Collect item
                                //Update both lists and reset counts
                                BetweenMajorOrHelpfulList.Add(BetweenMajorOrHelpfulCount);
                                BetweenMajorList.Add(BetweenMajorCount);
                                BetweenMajorOrHelpfulCount = 0;
                                BetweenMajorCount = 0;
                                //Find number of locations unlocked, add to list, update count of locations unlocked
                                int locationsunlocked = GetReachableLocations(world, owneditems).GetLocationCount();
                                int newlocations = locationsunlocked - prevlocationsunlocked;
                                LocationsUnlockedPerMajorFound.Add(newlocations);
                                prevlocationsunlocked = locationsunlocked;
                            }
                            else if (i.Importance == 3) //Goal item, break loop here
                            {
                                owneditems.Add(i); //Collect goal item, indicates successful completion
                            }
                        }
                    }
                }
                if(!gomode) //Update this traversal with the number of locations checked within it, unless player is in go mode
                {
                    LocationsPerTraversal.Add(checkcount);
                }
                List<double> exitscores = new List<double>();
                if (current.Exits.Count == 1) //Only one exit, take it unless need to break
                {
                    double score = ExitScore(world, owneditems, current, traversed, current.Exits.First());
                    if(score > 0) //If score is -1 and this is the only exit, then a dead end has been reached; if score is 0, then there are no items left to find; otherwise simply take exit since it is the only one
                    {
                        current = world.Regions.First(x => x.Name == current.Exits.First().ToRegionName); //Move to region
                        if(score >= 10000000000) //Score this high indicates player is in "go mode" where they are now rushing the end of the game
                        {
                            gomode = true;
                        }
                        //Update count of regions between finding items
                        BetweenMajorOrHelpfulCount++;
                        BetweenMajorCount++;
                    }
                    else
                    {
                        break; //Break loop in failure
                    }
                }
                else
                {
                    List<double> scores = new List<double>();
                    //Calculate score for each exit
                    foreach (Exit e in current.Exits)
                    {
                        scores.Add(ExitScore(world, owneditems, current, traversed, e));
                    }
                    if(scores.Count(x => x > 0) > 0) //If none of the scores are greater than 0, all exits are either untraversable or have no available items, indicating dead end has been reached
                    {
                        int maxindex = scores.IndexOf(scores.Max()); //Get index of the maximum score
                        previous = current;
                        current = world.Regions.First(x => x.Name == current.Exits.ElementAt(maxindex).ToRegionName); //Move to region with maximum score
                        if (scores.Max() >= 10000000000) //Score this high indicates player is in "go mode" where they are now rushing the end of the game
                        {
                            gomode = true;
                        }
                        //Update count of regions between finding items
                        BetweenMajorOrHelpfulCount++;
                        BetweenMajorCount++;
                    }
                    else
                    {
                        break; //Break loop in failure
                    }
                }
            }
            //Package all lists into list of lists and return
            PlaythroughInfo output = new PlaythroughInfo();
            output.BetweenMajorOrHelpfulList = BetweenMajorOrHelpfulList;
            output.BetweenMajorList = BetweenMajorList;
            output.LocationsPerTraversal = LocationsPerTraversal;
            output.LocationsUnlockedPerMajorFound = LocationsUnlockedPerMajorFound;
            output.Traversed = traversed;
            output.Completable = owneditems.Count(x => x.Importance == 3) > 0; //Has goal item, so game is completable
            output.InitialReachableCount = initial;
            return output;
        }

        //Used to represent the maximum nodes traversed in this search
        private int maxtraversed;

        //Utilizes the recusive DFS for exit score function to return a score for this exit if its requirement is met, otherwise returns -1
        private double ExitScore(WorldGraph world, List<Item> owneditems, Region current, List<Region> traversed, Exit exit)
        {
            if(parser.RequirementsMet(exit.Requirements, owneditems))
            {
                double score = 0;
                List<Region> visited = new List<Region>();
                visited.Add(current); //Do this so path does not go through current region
                Region exitto = world.Regions.First(x => x.Name == exit.ToRegionName); //Get the region this edge leads to
                maxtraversed = 0;
                score += RecursiveDFSForExitScore(world, exitto, visited, owneditems, 1); //Add score from a recursive search which scores item locations, scored lower more regions traversed
                int multiplier = maxtraversed == 1 ? 2 : 1; //Give a multiplier if this edge leads to a single, dead-end region

                //Give divider based on how recently the region was visited
                int divider = 1;
                int lastindex = traversed.FindLastIndex(x => x.Name == exit.ToRegionName);
                if(lastindex > -1)
                {
                    int howrecent = traversed.Count - 2 - lastindex;
                    divider = 16 - howrecent; //Most recent region divided by 8, 2nd most recent divided by 7, etc to minimum of 1
                    divider = divider < 1 ? 1 : divider;
                }
                return score * multiplier / divider;
            }
            else //Can not traverse exit
            {
                return -1; //Return -1 to indicate it cannot be crossed
            }
        }

        //Scores an exit by recurisvely searching for items, adding score for every available location, with less weight if farther away
        public double RecursiveDFSForExitScore(WorldGraph world, Region r, List<Region> visited, List<Item> owneditems, int traversed)
        {
            maxtraversed = Math.Max(traversed, maxtraversed);
            visited.Add(r); //Add to visited list
            double score = 0;
            double multiplier = Math.Max(1 / 8, 1 / (double)traversed); //Max multiplier is 1, Minimum multiplier is 1/8
            //First look at all locations in region to add to score
            foreach (Location l in r.Locations)
            {
                if (parser.RequirementsMet(l.Requirements, owneditems)) //Only want to consider available locations
                {
                    if (l.Item.Importance == 3) //Path contains goal item, add large amount to score, add 100 / traversed so that shorter paths to goal preferred
                    {
                        score += 10000000000 + (100 / traversed);
                    }
                    else if (l.Item.Importance > -1) //Else just add 1 if item isn't already collected (remember, although player knows there is a location here, they don't know what item it is unless it's the goal)
                    {
                        score += 1 * multiplier;
                    }
                }
            }
            //Now recursively look through each exit
            foreach (Exit e in r.Exits)
            {
                if (parser.RequirementsMet(e.Requirements, owneditems)) //Only consider exit if it can be traversed
                {
                    Region exitto = world.Regions.First(x => x.Name == e.ToRegionName); //Get the region this edge leads to
                    if (!visited.Contains(exitto)) //Don't revisit already visited nodes on this path
                    {
                        //Recursively call this function, adding 1 to traversed, score will be added to our score and returned
                        //We purposely pass visited by reference rather than by value, ensuring that locations are only visited once
                        score += RecursiveDFSForExitScore(world, exitto, visited, owneditems, traversed + 1);
                    }

                }
            }
            return score;
        }
    }

    //Struct to record list of spheres and completability bool
    struct SphereSearchInfo
    {
        public List<WorldGraph> Spheres;
        public bool Completable;
    }

    //Struct to record different playthrough metrics and completability bool used to score interestingness metrics
    struct PlaythroughInfo
    {
        public List<int> BetweenMajorOrHelpfulList;
        public List<int> BetweenMajorList;
        public List<int> LocationsPerTraversal;
        public List<int> LocationsUnlockedPerMajorFound;
        public List<Region> Traversed;
        public int InitialReachableCount;
        public bool Completable;
    }
}
