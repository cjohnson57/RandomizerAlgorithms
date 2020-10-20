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
        //Calculate complexity of the base graph (Not considering items, only rules for location reachability)
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
                //Now calculate the total rule for each location
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
                    totalrule = parser.Simplify(totalrule.Replace("None", "true")); //Simplifies the boolean expression
                    totalrules.Add(totalrule);
                }
            }

            //We now have a list for the total rule of every location in the game
            //Calculate score for each rule and add them all to list
            List<double> scores = new List<double>();
            foreach(string rule in totalrules)
            {
                double score = 1; //Base score for each location is 1
                if(!(rule == "1")) //If false, can reach from beginning of game, just use base scoer
                {
                    char[] tokens = rule.ToCharArray();
                    foreach(char token in tokens)
                    {
                        if(token == '&')
                        {
                            score += .5; //Add .5 to score for each AND since they add complexity
                        }
                        else if (token == '|')
                        {
                            score -= .5; //Subtract .5 from score for each OR since they reduce complexity
                        }
                        else if (token != '(' && token != ')') //Only possible values in string are &, |, (, ), and a variable; so if this condition is true then it's a variable
                        {
                            score += 1; //Add 1 to score for each variable in expression
                        }
                    }
                }
                scores.Add(score);
            }
            //Use list of scores to calculate final score and return
            return ScoreCalculation(scores);
        }

        //Use list of scores for each rule in world to calculate final score based on some statistic
        //Possibilities:
        // Simple sum of scores
        // Average of scores
        // Average of top x%
        // Max score
        // Sum of Squares
        //For now, using sum of squares, may change later
        private double ScoreCalculation(List<double> scores)
        {
            double sum = scores.Sum();
            double avg = scores.Average();
            double max = scores.Max();
            double sumofsquares = 0;
            foreach (double score in scores)
            {
                double deviation = score - avg;
                sumofsquares += deviation * deviation;
            }
            return sumofsquares;
        }
    }
}
