using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class implements functions to compute the following properties of a world:
    //Complexity (Does not consider item placement)
    //Bias (Considers item placement)
    //Interestingness? (Considers item placement)
    class Statistics
    {
        public double CalcWorldComplexity(WorldGraph world)
        {
            Search searcher = new Search();
            Parser parser = new Parser();
            List<string> totalrules = new List<string>();
            /*
             * For each location, calculate a total rule
             * Total rule meaning it includes every possible path to get there plus the rule for the location itself
             * ex, 2 paths to location: Region A -> Region B -> Region C, Region A -> Region C
             * Location has Rule X
             * Then the total rule will equal:
             * ((A->B and B->C) or (A->C)) and Rule X
             */
            foreach(Region r in world.Regions)
            {
                //Must calculate every possible path (that doesn't go back on itself) from root to the region r
                List<List<Region>> paths = searcher.PathsToRegion(world, r);
                string regionstring = "";
                if(paths.Count > 0) //If it equals 0, current region is root, do not need region string
                {
                    for(int i = 0; i < paths.Count; i++)
                    {
                        List<Region> path = paths[i];
                        string pathstring = "";
                        for (int j = 0; j < path.Count - 1; j++) //Last region is dest, don't need to check thus paths.Count - 1
                        {
                            if (j > 0)
                            {
                                pathstring += " and "; // Every requirement on this path must be met, so use "and"
                            }
                            pathstring += "(" + path[j].Exits.First(x => x.ToRegionName == path[j + 1].Name).Requirements + ")";
                        }
                        if(i > 0)
                        {
                            regionstring += " or "; //The pathstrings are different options, so use "or"
                        }
                        regionstring += "(" + pathstring + ")";
                    }

                }
                foreach(Location l in r.Locations)
                {
                    string totalrule = "";
                    if(string.IsNullOrEmpty(regionstring)) //For when region is root
                    {
                        totalrule = l.Requirements;
                    }
                    else
                    {
                        totalrule = "(" + regionstring + ") and " + l.Requirements; //Must meet at least one path requirement to reach the region and the location requirement
                    }
                    totalrule = parser.Simplify(totalrule.Replace("None", "true"));
                    totalrules.Add(totalrule);
                }
            }


            return 0;
        }

    }
}
