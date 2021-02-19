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
        private Search searcher = new Search();
        private Parser parser = new Parser();

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
        //After much testing and consideration, decided to use average of top 50%
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
            //Package output and return
            BiasOutput output = new BiasOutput();
            output.biasvalue = overallsum / spherebias.Length; //Get average of absolute value to determine overall bias
            output.direction = beforesum < aftersum; //If bias is more positive before the median, the direction is toward the beginning, otherwise toward end
            return output;
        }

        //Calculate info about human-like playthrough and then return the score
        public InterestingnessOutput CalcDistributionInterestingness(WorldGraph world)
        {
            PlaythroughInfo info = new PlaythroughInfo();
            try
            {
                info = searcher.PlaythroughSearch(world.Copy());
            }
            catch
            {
                throw new Exception(); //Something went wrong, have calling code retry
            }
            BiasOutput biasinfo = CalcDistributionBias(world);
            return ScorePlaythrough(world, info, biasinfo);
        }

        /*
         * Score info about human-like playthrough
         * Several considerations:
         * 1. Bias
         * 2. Fun
         * 3. Challenge
         * 4. Satisfyingness
         * 5. Boredom
         */
        public InterestingnessOutput ScorePlaythrough(WorldGraph world, PlaythroughInfo input, BiasOutput biasinfo)
        {
            //First, calculate fun metric, which desires a consistently high rate of checking item locations
            Queue<int> RollingAvg = new Queue<int>();
            List<double> avgs = new List<double>();
            List<bool> highavg = new List<bool>();
            foreach (int num in input.LocationsPerTraversal)
            {
                if(RollingAvg.Count == 5) //Rolling average of last 5 values
                {
                    RollingAvg.Dequeue();
                }
                RollingAvg.Enqueue(num);
                double avg = RollingAvg.Average();
                highavg.Add(avg >= 1); //If average is above 1, considered high enough to be fun, so add true to list, else add false
                avgs.Add(avg);
            }
            double fun = (double)highavg.Count(x => x) / highavg.Count(); //Our "Fun" score is the percentage of high values in the list
            //Next calculate challenge metric, which desires rate at which items are found to be within some optimal range so that it is not too often or too rare
            double LocationToItemRatio = (double)world.GetLocationCount() / world.Items.Where(x => x.Importance == 2).Count();
            int low = (int)Math.Floor(LocationToItemRatio * .5);
            int high = (int)Math.Ceiling(LocationToItemRatio * 1.5);
            RollingAvg = new Queue<int>();
            avgs = new List<double>();
            List<bool> avginrange = new List<bool>();
            foreach (int num in input.BetweenMajorList)
            {
                if (RollingAvg.Count == 3) //Tighter rolling average of last 3 values
                {
                    RollingAvg.Dequeue();
                }
                RollingAvg.Enqueue(num);
                double avg = RollingAvg.Average();
                avginrange.Add(low <= avg && avg <= high); //If value is within range rather than too high or too low, add true to list to indicate it is within a good range
                avgs.Add(avg);
            }
            double challenge = (double)avginrange.Count(x => x) / avginrange.Count(); //Our "Challenge" score is the percentage of values in the list within desirable range
            //Next calculate satisfyingness metric based on how many locations are unlocked when an item is found
            double LocationToItemRatioWithoutInitial = (double)(world.GetLocationCount() - input.InitialReachableCount) / world.Items.Where(x => x.Importance == 2).Count();
            int satthreshold = (int)Math.Floor(LocationToItemRatioWithoutInitial); //Set threshold as number of not-immediately-accessible locations divided by number of major items
            List<bool> SatisfyingReachesThreshold = new List<bool>();
            foreach(int num in input.LocationsUnlockedPerMajorFound)
            {
                SatisfyingReachesThreshold.Add(num >= satthreshold);
            }
            double satisfyingness = (double)SatisfyingReachesThreshold.Count(x => x) / SatisfyingReachesThreshold.Count(); //Our "Satisfyingness" score is the percentage of values above the desired threshold
            //Finally calculate boredom by observing regions which were visited more often than is expected
            //First get a count of how many times each region was visited
            List<int> visitcounts = new List<int>();
            foreach(Region r in world.Regions)
            {
                visitcounts.Add(input.Traversed.Count(x => x.Name == r.Name));
            }
            //Calculate threshold with max number of times region should be visited being the number of traversals divided by number of regions
            double TraversedToRegionRatio = (double)input.Traversed.Count() / world.Regions.Count();
            int borethreshold = (int)Math.Ceiling(TraversedToRegionRatio);
            List<bool> VisitsAboveThreshold = new List<bool>();
            foreach(int num in visitcounts)
            {
                VisitsAboveThreshold.Add(num > borethreshold); //Again as before, add list of bool when value is above threshold
            }
            double boredom = (double)VisitsAboveThreshold.Count(x => x) / VisitsAboveThreshold.Count(); //Our "Boredom" score is the percentage of values above the desired threshold
            //Add calculated stats to output. If a result is NaN (possible when not completable) save as -1
            InterestingnessOutput output = new InterestingnessOutput();
            output.bias = biasinfo;
            output.fun = double.IsNaN(fun) ? -1 : fun;
            output.challenge = double.IsNaN(challenge) ? -1 : challenge;
            output.satisfyingness = double.IsNaN(satisfyingness) ? -1 : satisfyingness;
            output.boredom = double.IsNaN(boredom) ? -1 : boredom;
            //Use stats to calculate final interestingness score
            //Each score is a double in the range [0, 1]
            //Multiply each score (or its 1 - score if low score is desirable) by its percentage share of the total
            double biasscore = (1 - output.bias.biasvalue) * .2;
            double funscore = output.fun * .2;
            double challengescore = output.challenge * .2;
            double satscore = output.satisfyingness * .2;
            double borescore = (1 - output.boredom) * .2;
            double intscore = biasscore + funscore + challengescore + satscore + borescore;
            //If any components are NaN, consider interestingness as NaN as well
            if(double.IsNaN(fun) || double.IsNaN(challenge) || double.IsNaN(satisfyingness) || double.IsNaN(boredom))
            {
                output.interestingness = -1;
            }
            else
            {
                output.interestingness = intscore;
            }
            output.completable = input.Completable;
            return output;
        }
    }

    //Struct to store all the possible metrics for complexity used during testing, now only top50 is used.
    struct TestComplexityOutput
    {
        public double sum;
        public double average;
        public double max;
        public double sumofsquares;
        public double top50; //Chosen metric for complexity
        public double top75;
    }

    //Struct to store bias value and direction resulting from bias calculation
    struct BiasOutput
    {
        public double biasvalue;
        public bool direction; //0: Toward beginning, 1: Toward end
    }

    //Struct to store all measures considered in interestingness and final score, as well as whether the world was completable
    struct InterestingnessOutput
    {
        public BiasOutput bias;
        public double fun;
        public double challenge;
        public double satisfyingness;
        public double boredom;

        public double interestingness;

        public bool completable;
    }

}
