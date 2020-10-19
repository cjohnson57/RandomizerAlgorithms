using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class implements some miscellaneous functions used in the other classes
    //Most importantly is the shuffle method
    class Helpers
    {

        private static Random rng;

        public Helpers()
        {
            rng = new Random();
        }

        //Initialize helpers with seed
        public Helpers(int seed)
        {
            rng = new Random(seed);
        }

        //Overwrites a location in the worldgraph to place the specified item in that location
        public void Place(ref WorldGraph world, Location location, Item item)
        {
            world.Regions.First(x => x.Locations.Count(x => x.Name == location.Name) == 1).Locations.First(x => x.Name == location.Name).Item = item;
        }

        //Shuffles list items
        //Should be done every time a list is initialized and popped from
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        //Gets the first item from the list and removes it
        public T Pop<T>(IList<T> list)
        {
            T r = list[0];
            list.RemoveAt(0);
            return r;
        }

        //Takes away redundant items from list subfrom which are already in toremove
        //Currently not used
        public List<T> SubtractLists<T>(List<T> subfrom, List<T> toremove)
        {
            foreach(T elem in toremove)
            {
                subfrom.Remove(elem);
            }
            return subfrom;
        }
    }
}
