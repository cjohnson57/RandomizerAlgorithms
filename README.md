# RandomizerAlgorithms

## Introduction

This is a project to study algorithms used to randomize key item location in computer games which require certain key items to reach certain locations.

For example, if a computer game has a hammer item which is used to smash rocks, and you want to randomize the location of this item, the hammer can not be placed in a location hidden under a rock.

Further, the location of the hammer can not require an item which is hidden under a rock.

There are three algorithms which can accomplish this:
* Random Fill
* Forward Fill
* Assumed Fill

For in depth descriptions and psuedocode of the algorithms used, see the following document:

https://drive.google.com/file/d/1KiCcuJ86YTDVQAVm7HmEQH8xxMhLdQKq/view?usp=sharing

Currently, there is no existing literature on these. The goal of this project is to publish a paper about the algorithms so the previous statement is no longer true.

## Needs to be done

Currently the program is functional. Want to add calculations for following statistics:

* World complexity
    * Plan to take into account complexity of absolute rule/requirement strings of item locations

* Randomized distribution bias
    * Basically, for forward and assumed fill, how much the item distribution is biased toward the early areas of the game by the algorithm
    
* Interestingness
    * This one is bit more nebulous. May include bias, number of items available per sphere
    
After all of these calculations are possible and several world graphs of increasing complexity have been generated, want to perform extensive experimention to determine each of these statistics, as well as failure rate and execution time, for each algorithm on each graph.
    
## Files

[Program.cs](Program.cs) is the main function, basically used as the experiment space

[Fill.cs](Fill.cs) defines the 3 fill algorithms: 
* Random Fill
* Forward Fill
* Assumed Fill

[Search.cs](Search.cs) defines the functions used for searching
* GetReachableLocations given a world graph and item set, returns all locations reachable with given item set. Utilizes BFS
* PathsToRegion returns all possible paths (that don't go back on themselves) from the root region to a target region. Utilizes recursive DFS
* SphereSeach calculates spheres of reachability, where all items in sphere x are reachable with items found in sphere x-1

[Statistics.cs](Statistics.cs) is currently mostly empty but will be used to calculate the aforementioned complexity, bias, and interestingness.
* Complexity: Has some code to find the absolute rule for every item location in the graph and give it a complexity score. Some statistic on list of scores will then be used to calculate final score
    * Ex. Sum of squares

[Parser.cs](Parser.cs) is used to parse a logical requirement string and use given item set to determine if the conditions are satisfied or not.
* Utilizes a modification of the shunting yard algorithm
* Also has ability to simplify a given rule string by transforming it into a basic boolean expression and utilizing the small script [simplify.py](simplify.py) to reduce it
    * Used in world complexity calculation to simplify all location rules

[Helpers.cs](Helpers.cs) contains some miscellaneous functions that didn't quite fit in the other files. The most significant of these is the shuffle function, which provides randomness.

[WorldClasses](WorldClasses) contains several classes which are used to define an input world graph.

[WorldGraphs](WorldGraphs) contains some sample world graphs in .json form.
* Currently these include a test world (empty items besides goal), randomized test world (items filled by Assumed Fill), and the test world with its original item placements for comparison.

## Examples

Sample World (Original Design)

![Sample World Original](https://i.imgur.com/mX2Kh0G.png)
    
Sample World (Randomized with Assumed Fill)

![Sample World Randomized](https://i.imgur.com/sL4CQvi.png)

Sphere calculation for the randomized sample world
```
Sphere 0:
Forest_Chest: Key
Forest_Quest: GrapplingHook
Field_Hidden A: Bow
City_Quest: Magic

Sphere 1:
Lake_Quest: Bombs
Village_Mini-Game: Key

Sphere 2:
River_Hidden A: Sword
River_Chest: Sling
Waterfall_Hidden A: GateKey

Sphere 3:
Arena_Boss: Goal
```
