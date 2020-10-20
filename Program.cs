using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace RandomizerAlgorithms
{
    class Program
    {
        const int trials = 1;
        //Random, forward, and assumed fill; set to true to test on that algo
        static bool[] dotests = { false, false, true };

        static void Main(string[] args)
        {
            Fill filler = new Fill(950834059);
            Search searcher = new Search();

            string jsontext = File.ReadAllText("../../../WorldGraphs/TestWorld.json");
            WorldGraph world = JsonConvert.DeserializeObject<WorldGraph>(jsontext);
            List<Item> majoritempool = world.Items.Where(x => x.Importance == 2).ToList();
            List<Item> minoritempool = world.Items.Where(x => x.Importance < 2).ToList();

            //Statistics teststatistics = new Statistics();
            //double complexityscore = teststatistics.CalcWorldComplexity(world);

            //Search testsearcher = new Search();
            //testsearcher.PathsToRegion(world, world.Regions.First(x => x.Name == "Waterfall"));

            //Parser testparse = new Parser();
            ////majoritempool.RemoveAt(8);
            ////majoritempool.RemoveAt(0);
            //bool result = testparse.RequirementsMet("(Sword and Bow) or Has(Key,2)", majoritempool);

            //string testjsontext = File.ReadAllText("../../../WorldGraphs/TestWorldOriginal.json");
            //WorldGraph testworld = JsonConvert.DeserializeObject<WorldGraph>(testjsontext);
            //SphereSearchOutput testoutput = searcher.SphereSearch(testworld);
            //Print_Spheres(testoutput);

            //Loop for random fill
            if (dotests[0])
            {
                for (int i = 0; i < trials; i++)
                {
                    WorldGraph randomgraph = filler.RandomFill(world, majoritempool); 
                    randomgraph = filler.RandomFill(randomgraph, minoritempool);

                    SphereSearchOutput output = searcher.SphereSearch(randomgraph);
                    Print_Spheres(output);
                }
            }

            //Loop for forward fill
            if (dotests[1])
            {
                for (int i = 0; i < trials; i++)
                {
                    WorldGraph forwardgraph = filler.ForwardFill(world, majoritempool); //Shuffle major items with logic
                    forwardgraph = filler.RandomFill(forwardgraph, minoritempool); //Shuffle minor items without logic

                    SphereSearchOutput output = searcher.SphereSearch(forwardgraph);
                    Print_Spheres(output);
                }
            }

            //Loop for assumed fill
            if (dotests[2])
            {
                for (int i = 0; i < trials; i++)
                {
                    WorldGraph assumedgraph = filler.AssumedFill(world, majoritempool); //Shuffle major items with logic
                    assumedgraph = filler.RandomFill(assumedgraph, minoritempool); //Shuffle minor items without logic

                    SphereSearchOutput output = searcher.SphereSearch(assumedgraph);
                    Print_Spheres(output);

                    //string jsonworld = JsonConvert.SerializeObject(assumedgraph);
                }
            }
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
        static void Print_Spheres(SphereSearchOutput input)
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
    }
}
