using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooleanParser;

namespace RandomizerAlgorithms
{
    //This class is used to parse a requirements string to determine if the current set of owned items satisfies the requirements or not
    class Parser
    {
        //Converts requirements into a logical statement and uses an algorithm to process it
        public bool RequirementsMet(string requirements, List<Item> owneditems)
        {
            if (requirements.ToUpper() == "NONE") //If no requirements then answer is true
            {
                return true;
            }
            else //Must parse the logical string and check if key item nouns are in the owneditems list
            {
                //This loop converts the statement from a sequence of items to a sequence of true or false
                //Ex, statement = Bow and Sword
                //Bow is owned, sword is not owned
                //Statement becomes -> true and false
                string[] keywordlist = { "(", ")", "and", "or" };
                string[] words = requirements.Split(" "); //Split string into words
                string convertedrequirements = ""; //Used to reconstruct words back into sentence the BooleanParser can process
                for(int i = 0; i < words.Length; i++)
                {
                    string word = words[i];
                    if(!keywordlist.Contains(word)) //Not a keyword, so is an item
                    {
                        if(word.Length > 7 && word.Substring(0, 4) == "Has(") // Has(item,x) indicates that x number of item is required. Ex Has(Key,2) indicates 2 keys are required. Minimum length 8: Has(x,y)
                        {
                            int startindex = word.IndexOf("(");
                            int endindex = word.IndexOf(",");
                            int numindex = word.IndexOf(")");
                            string item = word.Substring(startindex + 1, endindex - startindex - 1); //Parse item name from string
                            int num = int.Parse(word.Substring(endindex + 1, numindex - endindex - 1)); //Parse required number from string
                            if (owneditems.Where(x => x.Name == item).Count() >= num) //Has at least required number of item, set to true
                            {
                                words[i] = "true";
                            }
                            else //Does not own item or does not have enough of item, set to false
                            {
                                words[i] = "false";
                            }
                        }
                        else
                        {
                            if (owneditems.Where(x => x.Name == word).Count() > 0) //Has item, set to true
                            {
                                words[i] = "true";
                            }
                            else //Does not own item, set to false
                            {
                                words[i] = "false";
                            }
                        }
                    }
                    convertedrequirements += words[i]; //Add word to convertedrequirements
                    //If last word, don't add a space
                    //If a left paranthesis, don't add a space
                    //If this is not the last word and the next word is a right parenthesis, don't add a space
                    if(i != words.Length - 1 && word != "(" && (i < words.Length - 1 && words[i+1] != ")")) 
                    {
                        convertedrequirements += " "; //Add a space if necessary
                    }
                }
                convertedrequirements = convertedrequirements.ToUpper();
                return (new BooleanParser.Parser(convertedrequirements)).Parse(); //BooleanParser library parses the statement. So string "true and false" will return false.
            }
        }

    }
}
