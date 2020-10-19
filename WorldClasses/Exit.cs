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
    }
}
