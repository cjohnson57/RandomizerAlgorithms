using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class represents and item
    //Items are placed in locations, which are placed in regions
    class Item
    {
        public string Name;
        //3 = Goal Item
        //2 = Major/Key item, unlocks exits and locations
        //1 = Helpful item
        //0 = Junk Item
        //-1 = Empty Location
        public int Importance;

        //Default, empty item
        public Item()
        {
            Name = null;
            Importance = -1;
        }

        //Parameters specified
        public Item(string name, int importance)
        {
            Name = name;
            Importance = importance;
        }
    }
}
