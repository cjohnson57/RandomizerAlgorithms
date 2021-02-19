using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace RandomizerAlgorithms
{
    class Program
    {
        //Number of trials to perform per algorithm per world
        const int trials = 100000;

        //Random, forward, and assumed fill; set to true to test on that algo
        static readonly bool[] dotests = { true, true, true };

        //Fill list with name of worlds you want to consider
        static readonly string[] testworlds = { "World1", "World2", "World3", "World4", "World5" };
        //static string[] testworlds = { "TestWorld" };

        //Class which abstracts the database used to store experimental results
        //If you don't want to bother with the database you can comment this line, as well as all related lines in Main
        //    Initialize countofexp to 0 if you do this
        //Result information could be outputted to console instead, potentially averaged
        private static ResultDB db = new ResultDB();

        //Experiment space
        static void Main(string[] args)
        {
            Fill filler = new Fill();
            Search searcher = new Search();
            Statistics stats = new Statistics();

            ////Uncomment to test different complexity measures and generate many worlds with different parameters.
            //double[] testaverages = new double[5];
            //for (int regioncount = 10; regioncount <= 50; regioncount += 5)
            //{
            //    for (int itemcount = 5; itemcount <= Math.Min(regioncount, 30); itemcount += 5)
            //    {
            //        List<TestComplexityOutput> complexity = AverageComplexity(regioncount, itemcount);
            //        Console.WriteLine("Regions: " + regioncount + ", Items: " + itemcount);
            //        Console.WriteLine("Sum: " + complexity.Average(x => x.sum));
            //        Console.WriteLine("Avg: " + complexity.Average(x => x.average));
            //        Console.WriteLine("Max: " + complexity.Average(x => x.max));
            //        Console.WriteLine("SOS: " + complexity.Average(x => x.sumofsquares));
            //        Console.WriteLine("Avg50: " + complexity.Average(x => x.top50));
            //        Console.WriteLine("Avg75: " + complexity.Average(x => x.top75));
            //        Console.Write(Environment.NewLine);
            //    }
            //}

            //Loop through each algorithm set to be used and each world in the list, performing specified algorithm on specified world and recording information about the result.
            string[] algos = { "Random", "Forward", "Assumed" };
            foreach (string worldname in testworlds)
            {
                DateTime expstart = DateTime.Now;
                string jsontext = File.ReadAllText("../../../WorldGraphs/" + worldname + ".json");
                WorldGraph world = JsonConvert.DeserializeObject<WorldGraph>(jsontext);
                //Loop to perform fill algorithms
                for(int i = 0; i < 3; i++) //0 = Random, 1 = Forward, 2 = Assumed
                {
                    if(dotests[i])
                    {
                        int savecounter = 0;
                        int countofexp = db.Results.Count(x => x.Algorithm == algos[i] && x.World == worldname);
                        while(countofexp < trials) //Go until there are trial number of records in db
                        {
                            InterestingnessOutput intstat = new InterestingnessOutput();
                            double difference = -1;
                            while(true) //If something goes wrong in playthrough search, may need to retry 
                            {
                                WorldGraph input = world.Copy(); //Copy so that world is not passed by reference and overwritten
                                List<Item> majoritempool = input.Items.Where(x => x.Importance == 2).ToList();
                                List<Item> minoritempool = input.Items.Where(x => x.Importance < 2).ToList();
                                WorldGraph randomizedgraph = new WorldGraph();
                                DateTime start = DateTime.Now; //Start timing right before algorithm
                                //Decide which algo to use based on i
                                switch (i)
                                {
                                    case 0:
                                        randomizedgraph = filler.RandomFill(input, majoritempool);
                                        break;
                                    case 1:
                                        randomizedgraph = filler.ForwardFill(input, majoritempool);
                                        break;
                                    case 2:
                                        randomizedgraph = filler.AssumedFill(input, majoritempool);
                                        break;
                                }
                                randomizedgraph = filler.RandomFill(randomizedgraph, minoritempool); //Use random for minor items always since they don't matter
                                //Calculate metrics 
                                DateTime end = DateTime.Now;
                                difference = (end - start).TotalMilliseconds;
                                try
                                {
                                    intstat = stats.CalcDistributionInterestingness(randomizedgraph);
                                    break; //Was successful, continue
                                }
                                catch { } //Something went wrong, retry fill from scratch
                                ////Uncomment to print the spheres of the result.
                                //SphereSearchInfo output = searcher.SphereSearch(randomizedgraph);
                                //Print_Spheres(output);
                            }
                            //Store result in database
                            Result result = new Result();
                            result.Algorithm = algos[i];
                            result.World = worldname;
                            result.Completable = intstat.completable;
                            result.ExecutionTime = difference;
                            result.Bias = intstat.bias.biasvalue;
                            result.BiasDirection = intstat.bias.direction;
                            result.Interestingness = intstat.interestingness;
                            result.Fun = intstat.fun;
                            result.Challenge = intstat.challenge;
                            result.Satisfyingness = intstat.satisfyingness;
                            result.Boredom = intstat.boredom;                        
                            db.Entry(result).State = EntityState.Added;
                            savecounter++;
                            if (savecounter >= 1000) //Save every 1000 results processed
                            {
                                db.SaveChanges();
                                savecounter = 0;
                            }
                            countofexp++;
                        }
                        db.SaveChanges(); //Save changes when combo of algo and world is done
                    }
                }
                DateTime expend = DateTime.Now;
                double expdifference = (expend - expstart).TotalMinutes;
                Console.WriteLine("Time to perform " + trials + " iterations for world " + worldname + ": " + expdifference + " minutes"); //Print how long this world took to do
            }
            Console.ReadLine();
        }

        //This function outputs text for each sphere in the calculated sphere list
        //To avoid redundancy, major items will not be printed more than once
        //Sample output:
        /*
        Sphere 0:
        Forest_Chest: Bow
        Forest_Quest: Magic
        Field_Hidden A: Bombs
        Field_Hidden B: Sword
        City_Quest: GateKey

        Sphere 1:
        Field_Hidden C: GrapplingHook
        Lake_Quest: Sling

        Sphere 2:
        Valley_Chest: Key

        Sphere 3:
        Dungeon_Boss: Goal

        Is Completable: True
        */
        static void Print_Spheres(SphereSearchInfo input)
        {
            List<WorldGraph> spheres = input.Spheres;
            List<Item> MajorItemsFound = new List<Item>(); //Keeps track of already printed items to avoid redundancy
            int i = 0;
            foreach(WorldGraph sphere in spheres) //Go through each sphere
            {
                Console.WriteLine(Environment.NewLine + "Sphere " + i + ":"); //Print the sphere index
                i++;
                List<Item> majors = sphere.CollectAllItems().Where(x => x.Importance >= 2).ToList(); //Only check for major items
                foreach(Item m in majors)
                {
                    if(!MajorItemsFound.Contains(m)) //Check if each major item has been printed already; if not print
                    {
                        MajorItemsFound.Add(m);
                        //This loop is used to find the location of the item so that it may be printed as well
                        Location l = new Location();
                        foreach(Region r in sphere.Regions)
                        {
                            if(r.Locations.Count(x => x.Item == m) > 0)
                            {
                                l = r.Locations.First(x => x.Item == m);
                            }
                        }
                        Console.WriteLine(l.Name + ": " + m.Name); //Print the item location and name
                    }
                }
            }
            Console.WriteLine(Environment.NewLine + "Is Completable: " + input.Completable + Environment.NewLine); //After finishing, print if completable or not
        }

        //Generates a world with a specific count of regions and items
        //First generates many worlds to determine an average complexity then returns a world generated with a certain tolerance of that complexity
        //This takes a while to run, mainly because each complexity calculation takes ~2 seconds due to running the external python script
        static string GenerateWorld(int regioncount, int itemcount)
        {
            double goalcomplexity = AverageComplexity(regioncount, itemcount).Average(x => x.top50);
            //Now generate worlds until one is generated within a certain tolerance of the average
            double tolerance = .10; //10%
            while (true)
            {
                //Generate a world, check its complexity
                WorldGenerator generator = new WorldGenerator(regioncount, itemcount);
                WorldGraph generated = generator.Generate();
                int test = generated.GetLocationCount();
                double complexity = generator.GetComplexity().top50;
                if (goalcomplexity * (1 - tolerance) < complexity && complexity < goalcomplexity * (1 + tolerance))
                {
                    //Once complexity within x% of average has been generated, return json of the world so it can be saved
                    return generated.ToJson();
                }
            }
        }

        //Calculate the average complexity from generating many worlds with a specific regioncount and itemcount
        static List<TestComplexityOutput> AverageComplexity(int regioncount, int itemcount)
        {
            //First do x trials to determine an average complexity
            int gentrials = 5;
            List<TestComplexityOutput> outputs = new List<TestComplexityOutput>();
            for (int i = 0; i < gentrials; i++)
            {
                WorldGenerator generator = new WorldGenerator(regioncount, itemcount);
                WorldGraph generated = generator.Generate();
                outputs.Add(generator.GetComplexity());
            }
             return outputs; //Determine average complexity, we want the goal complexity to be within some% of this
        }
    }
}
