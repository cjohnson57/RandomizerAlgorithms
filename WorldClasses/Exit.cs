using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class defines an edge to a specific region and the requirements to traverse it
    //Only the destination region is recorded as exits are not necessarily bidirectional; requirements to go one way can be different from requirements to go the other way
    class Exit
    {
        public string ToRegionName;
        public string Requirements; //Key item requirements to traverse this edge

        //Parameterless constructor
        //Just used for initialization
        public Exit()
        {
            ToRegionName = "";
            Requirements = "";
        }
        
        //Constructor which specifies the to region name
        //Requirement initialized to nothing
        public Exit(string to)
        {
            ToRegionName = to;
            Requirements = "None";
        }

        //Constructor which specifies both parameters
        public Exit(string to, string requirements)
        {
            ToRegionName = to;
            Requirements = requirements;
        }
    }
}
