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
    //Interestingness (Considers item placement)
    class Statistics
    {
        Search searcher = new Search();
        Parser parser = new Parser();

        //Calculate complexity of the base graph (Not considering items, only rules for location reachability)
        public TestComplexityOutput CalcWorldComplexity(WorldGraph world)
        {
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
                    //Go through each path and calculate the rule for that path to construct an absolute rule for the region
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
                scores.Add(parser.CalcRuleScore(rule));
            }
            //Use list of scores to calculate final score and return
            return ComplexityScoreCalculation(scores);
        }

        //Use list of scores for each rule in world to calculate final score based on some statistic
        //Possibilities:
        // Simple sum of scores
        // Average of scores
        // Average of top x%
        // Max score
        // Sum of Squares
        //For now, using sum of squares, may change later
        private TestComplexityOutput ComplexityScoreCalculation(List<double> scores)
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
            scores = scores.OrderBy(x => x).ToList();
            //Top 50 percent
            int start = Convert.ToInt32(scores.Count() * (1 - .5));
            List<double> top50percent = new List<double>(0);
            for (int i = start; i < scores.Count; i++)
            {
                top50percent.Add(scores[i]);
            }
            double avgtop50percent = top50percent.Average(); ////This was chosen as the metric for complexity
            //Top 75 percent
            start = Convert.ToInt32(scores.Count() * (1 - .75));
            List<double> top75percent = new List<double>(0);
            for(int i = start; i < scores.Count; i++)
            {
                top75percent.Add(scores[i]);
            }
            double avgtop75percent = top75percent.Average();
            TestComplexityOutput test = new TestComplexityOutput();
            test.sum = sum;
            test.average = avg;
            test.max = max;
            test.sumofsquares = sumofsquares;
            test.top50 = avgtop50percent;
            test.top75 = avgtop75percent;
            return test;
        }
        //Calculates the bias for a given permutation of items in the world graph
        public BiasOutput CalcDistributionBias(WorldGraph world)
        {
            //Get total counts for majors and items so a percent can be calculated
            int totalmajorcount = world.Items.Where(x => x.Importance >= 2).Count();
            int totallocationcount = world.GetLocationCount();
            //Find spheres in randomized world
            Search searcher = new Search();
            List<WorldGraph> spheres = searcher.SphereSearch(world).Spheres;
            //Initialize variables to use in the loop
            double[] spherebias = new double[spheres.Count];
            int rollingmajorcount = 0;
            int rollinglocationcount = 0;
            for (int i = 0; i < spheres.Count; i++)
            {
                WorldGraph sphere = spheres[i];
                //Check the number of major items in the sphere, not counting those in previous spheres
                int majoritemcount = sphere.CollectMajorItems().Count - rollingmajorcount;
                rollingmajorcount += majoritemcount;
                //Check the number of locations in the sphere, not counting those in previous spheres
                int locationcount = sphere.GetLocationCount() - rollinglocationcount;
                rollinglocationcount += locationcount;
                //Find the percentage of major items and locations in this sphere
                double majorpercent = majoritemcount / (double)totalmajorcount;
                double locationpercent = locationcount / (double)totallocationcount;
                //Now find the difference between the two percentages
                double difference = majorpercent - locationpercent;
                spherebias[i] = difference;
            }
            //Now that we have a list of biases find the sum of their absolute values to determine absolute bias
            //Also use the positivity of bias before and after the median to determine bias direction
            double overallsum = 0;
            double beforesum = 0; //Sums bias before median so a bias direction can be computed
            double aftersum = 0; //Sums bias after median so a bias direction can be computed
            bool even = spherebias.Length % 2 == 0; //Want to check if even so can determine when after the median is
            int median = spherebias.Length / 2; //Use median to determine bias direction
            for (int i = 0; i < spherebias.Length; i++)
            {
                overallsum += Math.Abs(spherebias[i]);
                if (i < median) //Before median, add to that sum
                {
                    beforesum += spherebias[i];
                }
                else if ((i >= median && even) || (i > median && !even)) //After median, add to that sum. If it's even then >= makes sense so every index is checked, if odd then skip middle
                {
                    aftersum = spherebias[i];
                }
            }
            BiasOutput output = new BiasOutput();
            output.bias = overallsum / spherebias.Length; //Get average of absolute value to determine overall bias
            output.direction = beforesum < aftersum; //If bias is more positive before the median, the direction is toward the beginning, otherwise toward end
            return output;
        }

        //Calculate info about human-like playthrough and then return the score
        public double CalcDistributionInterestingness(WorldGraph world)
        {
            PlaythroughInfo info = searcher.PlayThrough(world);
            return ScorePlaythrough(info);
        }

        /*
         * Score info about human-like playthrough
         * Several considerations:
         * 1. Number of locations collected for each region traversed
         * 2. Number of regions traversed between finding major or helpful items
         * 3. Number of regions traversed between finding major items
         */
        public double ScorePlaythrough(PlaythroughInfo input)
        {
            double score = 0;

            return score;
        }
    }

    struct TestComplexityOutput
    {
        public double sum;
        public double average;
        public double max;
        public double sumofsquares;
        public double top50; //Chosen metric for complexity
        public double top75;
    }

    struct BiasOutput
    {
        public double bias;
        public bool direction; //0: Toward beginning, 1: Toward end
    }

}
