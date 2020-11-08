using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class implements the algorithms to fill the world locations with items
    class Fill
    {

        public static Helpers helper;
        public static Search searcher;

        public Fill()
        {
            helper = new Helpers();
            searcher = new Search(helper);
        }

        //Initialize with specified seed
        public Fill(int seed)
        {
            helper = new Helpers(seed);
            searcher = new Search(helper);
        }

        //G: Graph of world locations (called world in code)
        // A node in G initially has a null value, this value can be filled with a key item
        // An edge in G may require certain items to traverse
        //R: Graph of reachable locations (called reachable in code)
        //I: Set of items owned, determines R, called owneditems in code
        //I*: Set of items not owned, inverse of I, called itempool in code
        //Goal: A specific item in G which signifies the end of the game
        //Start: A specific region in G which the player starts in


        /*
         * This algorithm simply places a random key item in a random location until either of these
         * sets are empty (Usually items). After placing all a check is done to see if the game is beatable. If
         * not it runs the algorithm again. In complex world this could potentially take hundreds of attempts.
         */
        public WorldGraph RandomFill(WorldGraph world, List<Item> itempool)
        {
            //Initialize owneditems to empty and locations to all that are empty
            List<Item> owneditems = new List<Item>(); 
            List<Location> locations = world.GetAllEmptyLocations();
            helper.Shuffle(locations);
            helper.Shuffle(itempool);
            while (locations.Count > 0 && itempool.Count > 0)
            {
                Location location = helper.Pop(locations); //Select random location
                helper.Shuffle(locations);
                Item item = helper.Pop(itempool); //Take random item from item pool
                helper.Shuffle(itempool);
                helper.Place(ref world, location, item); //Place random item in random location
                owneditems.Add(item); //Add to owned items
            }
            return world; //World has been filled with items, return
        }

        /*
         * This algorithm initializes set R to be the reachable locations from the start of the game. It
         * then chooses an item from the item pool I* and places it in a random location in set R, meaning
         * it is also added to I. This location is then removed from consideration and all locations that
         * become reachable are added to R. Repeat until R or I* is empty. To be clear, items related to
         * progression are placed first, and then everything else is filled in with helpful or junk items. In
         * fact, usually the helpful and junk items are placed using random fill since it’s faster and
         * placement doesn’t matter.
         */
        public WorldGraph ForwardFill(WorldGraph world, List<Item> itempool)
        {
            List<Item> owneditems = new List<Item>(); //Initialize owneditems to empty
            WorldGraph reachable = searcher.GetReachableLocations(world, owneditems); //Initially R should only equal locations reachable from the start of the game
            List<Location> locations = reachable.GetAllEmptyLocations();
            helper.Shuffle(locations);
            helper.Shuffle(itempool);
            while (locations.Count > 0 && itempool.Count > 0)
            {
                Location location = helper.Pop(locations); //Get random location and item
                Item item = helper.Pop(itempool);
                helper.Shuffle(itempool);
                helper.Place(ref world, location, item); //Place random item in random reachable location
                owneditems.Add(item); //Add new item to owned items, R will expand
                reachable = searcher.GetReachableLocations(world, owneditems); //Recalculate R now that more items are owned
                locations = reachable.GetAllEmptyLocations();
                helper.Shuffle(locations);

            }
            return world; //World has been filled with items, return
        }

        /*
         * This algorithm begins by assuming the player has access to all items, which implies all
         * locations are reachable, so R is initialized to the entire game and I contains all items. A random
         * item is selected from I and is removed from the player’s assumed item list, meaning that some
         * locations may become unreachable, and removed from R. A random, empty location which is
         * still reachable will then be selected and the previously removed item will be placed there. The
         * way this algorithm initially assumes all items are available and slowly removes them reducing
         * reachable areas, it can be thought of as a reverse fill.
        */
        public WorldGraph AssumedFill(WorldGraph world, List<Item> itempool)
        {
            List<Item> owneditems = itempool; //In contrast to other two algos, I is initialized to all items and itempool is empty
            itempool = new List<Item>();
            WorldGraph reachable = searcher.GetReachableLocationsAssumed(world, owneditems); //Initially R should equal all locations in the game
            List<Location> reachablelocations = reachable.GetAllEmptyLocations();
            helper.Shuffle(owneditems);
            while (reachablelocations.Count > 0 && owneditems.Count > 0)
            {
                Item item = helper.Pop(owneditems); //Pop random item from I, R will shrink
                helper.Shuffle(owneditems);
                reachable = searcher.GetReachableLocationsAssumed(world, owneditems); //Recalculate R now that less items are owned
                reachablelocations = reachable.GetAllEmptyLocations(); //Get empty locations which are reachable
                helper.Shuffle(reachablelocations);
                Location location = new Location();
                try
                {
                    location = helper.Pop(reachablelocations); //Remove location from list
                }
                catch //If this happens, means there are no reachable locations left and must return, usually indicates uncompletable permutation
                {
                    break;
                }
                helper.Place(ref world, location, item); //Place random item in random location
                itempool.Add(item); //Add item to item pool
            }
            return world; //World has been filled with items, return
        }
    }
}
