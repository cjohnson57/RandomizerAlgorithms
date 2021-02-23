# RandomizerAlgorithms

## Introduction

This is a project to study algorithms used to randomize key item location in computer games which require certain key items to reach certain locations.

For example, if a computer game has a hammer item which is used to smash rocks, and you want to randomize the location of this item, the hammer can not be placed in a location hidden under a rock.

Further, the location of the hammer can not require an item which is hidden under a rock.

There are three algorithms which can accomplish this:
* Random Fill
* Forward Fill
* Assumed Fill

For in depth descriptions and psuedocode of the algorithms used, see the Documents section.
    
## Files

[Program.cs](Program.cs) is the main function, basically used as the experiment space

[Fill.cs](Fill.cs) defines the 3 fill algorithms: 
* Random Fill
* Forward Fill
* Assumed Fill

[Search.cs](Search.cs) defines the functions used for searching
* GetReachableLocations is the main search algorithm. Given a world graph and item set, returns all locations reachable with given item set. Utilizes BFS.
* GetReachableLocationsAssumed is a slight modification for assumed search where reachability is computed iteratively to check for items that have been removed from the owned items but are still reachable.
* PathsToRegion returns all possible paths (that don't go back on themselves) from the root region to a target region. Utilizes recursive DFS.
* SphereSeach calculates spheres of reachability, where all items in sphere x are reachable with items found in sphere x-1.
* PlaythroughSearch traverses the game world similar to how a player would by going from region to region, collecting all available items, and using a heuristic considering the amount and proximity of item locations in each direction to decide which region to go to next.

[Statistics.cs](Statistics.cs) Used to calculate the aforementioned complexity, interestingness, bias, and other metrics for interestingness
* Complexity: Finds the absolute rule of every location in the world graph and scores it based on the number of items required to reach it, then uses the average of the max 50% of these scores to return a final complexity score. For more information on this scoring see "Complexity and Generation" and "Complexity and Generation Evaluation" under the Documents section.
* Interestingness: Performs PlaythroughSearch, extracts some metrics, and calculates a final interestingness score with the following considerations:
   * Bias: Looks at a randomized world and compares the percentage of item locations in early spheres to the percentage of major items within that sphere to evaluate whether the algorithm introduced some bias by placing items early. Lower bias is better. This is the most objective measure, as it does not utilize PlaythroughSearch or some threshold/scoring system to produce its result. It is also the only measure that was considered by randomizer developers previous to this study.
   * Fun: Represents making consistent progress throughout the game world by discovering item locations. Higher fun is better.
   * Challenge: Represents finding major items not too often but not too rarely, either. Higher challenge is better.
   * Satisfyingness: Represents how many locations are unlocked whenever a key item is discovered, i.e. how immediately useful the item is. Higher satisfyingness is better.
   * Boredom: Represents the player needing to repeatedly visit the same regions to make progress. Lower boredom is better.

For more information on how Interestingness metrics are calculated, see "Bias and PlaythroughSearch" and "Interestingness" under the Documents section.

[Parser.cs](Parser.cs) is used to parse a logical requirement string and use given item set to determine if the conditions are satisfied or not.
* Utilizes a modification of the shunting yard algorithm
* Also has ability to simplify a given rule string by transforming it into a basic boolean expression and utilizing the small script [simplify.py](simplify.py) to reduce it
    * Used in world complexity calculation to simplify all location rules

[Helpers.cs](Helpers.cs) contains some miscellaneous functions that didn't quite fit in the other files. The most significant of these is the shuffle function, which provides randomness.

[WorldGenerator.cs](WorldGenerator.cs) contains code to generate a randomized world graph given an input region count and item list. For more information, see "Complexity and Generation" under the Documents section.

[WorldClasses](WorldClasses) contains several classes which are used to define an input world graph.
* Of particular note is [WorldGraph.cs](WorldClasses/WorldGraph.cs), which contains many functions used to find information about a graph's layout and state.

[WorldGraphs](WorldGraphs) contains some sample world graphs in .json form.
* This includes a test world (empty items besides goal), randomized test world (items filled by Assumed Fill), and the test world with its originally designed item placements for comparison. Also contains 5 randomly generated test worlds of increasing complexity for evaluation.

[ExperimentResults](ExperimentResults) contains classes and files related to experiments performed for this research. For more information, see "Experimental Results" under the Documents section.
* [Result.cs](ExperimentResults/Result.cs) defines the class used to represent a row in the result database.
* [ResultDB.cs](ExperimentResults/ResultDB.cs) is used by EntityFramework to connect to the result database.
* [ResultsDB.zip](ExperimentResults/ResultsDB.zip) is the result database zipped so it can be stored on github. (Note it must be unzipped before it can be used.)
* [ResultQueries.sql](ExperimentResults/ResultQueries.sql) defines SQL queries used to extract statistics from the result database.
* [ResultStats.xlsx](ExperimentResults/ResultStats.xlsx) has tables where the outputs from ResultQueries are stored, as well as graphs of these results.

## Documents

[Pseudocode](https://drive.google.com/file/d/1w8PWoPOP0WWzDxhrliUTG9Q8VwHo0w_A/view?usp=sharing) Some pseudocode definitions of the algorithms used in this code, namely the Fill and Search functions.

[Initial Project Description/Update](https://docs.google.com/presentation/d/1kH7OujADrh_7JZ6zIJkcokt-aar8Mkaei7hMrmHWP9Y/edit?usp=sharing) Short presentation created after finishing the initial algorithm implementation.

[Complexity and Generation](https://docs.google.com/presentation/d/1BCcFDL4GdgRi2Ih_mrm7cEvWyxCpN6MDrw45Zl3AJjk/edit?usp=sharing) Document which describes the initial implementation of the complexity calculation and world generation.

[Complexity and Generation Evaluation](https://docs.google.com/presentation/d/1Sh6Km-P5fIdDRbM25pEOpdnlFkdVT1MeZXqyVRnMhSA/edit?usp=sharing) Goes over some evaluation done to determine what metric with which to calculate the final complexity score. Also describes generation of 5 sample worlds for testing purposes.

[Bias and PlaythroughSearch](https://docs.google.com/presentation/d/1_IezP3AD45V2YAj6AuKbr5FpFIJ9hw1qitbcXZCW-Hk/edit?usp=sharing) describes the calculation of bias from a computed world permutation as well as the implementation of PlaythroughSearch, which attempts to traverse the world as a player of the game would.

[Interestingness](https://docs.google.com/presentation/d/1VVmOIxesQDbI6D2yY7VFo1dLZs9_RkW5aShiv04aJKQ/edit?usp=sharing) describes the evaluation and calculation of metrics used to produce a final interestingness metric for a given permutation of items in a world.

[Experimental Results](https://docs.google.com/presentation/d/1VpflLQdzNcvkVdjA3HhacI251lAlfHv6vBvRO3feWhI/edit?usp=sharing) describes the experimental setup and shows graphs resulting from our experiments, which can also be seen in [the results excel file.](ExperimentResults/ResultStats.xlsx)

## Examples

Sample World (Original Design)

![Sample World Original](https://i.imgur.com/EIXoZHe.png)
    
Sample World (Randomized with Assumed Fill)

![Sample World Randomized](https://i.imgur.com/nZGoMjI.png)

Sphere calculation for the randomized sample world
```
Sphere 0:
Field_Hidden A: Bow
Valley_Hidden A: Bombs
City_Quest: GrapplingHook

Sphere 1:
Field_Hidden B: Sword
River_Hidden A: Key

Sphere 2:
Waterfall_Quest: Sling

Sphere 3:
Waterfall_Chest: GateKey

Sphere 4:
Dungeon_Chest: Magic

Sphere 5:
Lake_Quest: Key

Sphere 6:
Arena_Boss: Goal
```

Bias calculation using these spheres:

| Sphere  | Locations | % Of Locations | Major Items | % Of Major Items | % Difference | Abs. Value of Difference |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | 6 | .2 | 3 | .3 | .1 | .1 |
| 1 | 8 | .267 | 2 | .2 | -.067 | .067 |
| 2 | 1 | .033 | 1 | .1 | .067 | .067 |
| 3 | 5 | .167 | 1 | .1 | -.067 | .067 |
| 4 | 4 | .133 | 1 | .1 | -.033 | .033 |
| 5 | 5 | .167 | 1 | .1 | -.067 | .067 |
| 6 | 1 | .033 | 1 | .1 | .067 | .067 |
| Total | 30 | 1 | 10 | 1 | 0 | <ins>**.467**</ins> |

The result is then normalized by dividing the total of the absolute value of differences by the number of spheres, so the final bias is .467/7 = **.0667**. This is a pretty low value for bias, which is intuitive as looking at the sphere list, most spheres contain only a single item.
