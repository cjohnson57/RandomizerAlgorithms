using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

namespace RandomizerAlgorithms
{
    //This class is used to parse a requirements string to determine if the current set of owned items satisfies the requirements or not
    class Parser
    {
        Helpers helper = new Helpers();

        //Converts requirements into a logical statement and uses an algorithm to process it
        public bool RequirementsMet(string requirements, List<Item> owneditems)
        {
            if (requirements.ToUpper() == "NONE" || requirements.ToUpper() == "TRUE") //If no requirements then answer is true
            {
                return true;
            }
            else //Must parse the logical string and check if key item nouns are in the owneditems list
            {
                string function = ShuntingYard(requirements, owneditems);
                return FunctionInterpreter(function);
            }
        }

        //Implementation of the shunting yard algorithm specifically for boolean statements and setting item names to true or false
        private string ShuntingYard(string requirements, List<Item> owneditems)
        {
            Stack<string> tokens = new Stack<string>();
            string function = "";
            int index = 0;
            int length = requirements.Length;
            while (index < length) //Index step size is variable based on token
            {
                if (requirements[index] == ' ') //Just a space, move to next index
                {
                    index++;
                }
                else if (requirements[index] == '(') //Handle left parenthesis
                {
                    tokens.Push("(");
                    index++;
                }
                else if (requirements[index] == ')') //Handle right parenthesis
                {
                    while (tokens.Peek() != "(")
                    {
                        function += tokens.Pop();
                    }
                    tokens.Pop();
                    index++;
                }
                //Instead of checking for arithmetic operands, check for logical operands
                else if ((length - index) > 2 && requirements.Substring(index, 3).ToUpper() == "AND")
                {
                    string result = "AND";
                    while (tokens.Count() > 0 && precedence(tokens.Peek()) >= precedence(result))
                    {
                        function += tokens.Pop();
                    }
                    tokens.Push(result);
                    index += 3; //Increment index to go after and
                }
                else if ((length - index) > 1 && requirements.Substring(index, 2).ToUpper() == "OR")
                {
                    string result = "OR";
                    while (tokens.Count() > 0 && precedence(tokens.Peek()) >= precedence(result))
                    {
                        function += tokens.Pop();
                    }
                    tokens.Push(result);
                    index += 2; //Increment index to go after or
                }
                //Also check for hard coded logical expressions... Mainly used for Simplification function
                else if ((length - index) > 3 && requirements.Substring(index, 4).ToUpper() == "TRUE")
                {
                    function += "TRUE"; //Add expression to function
                    index += 4; //Increment index to go after true
                }
                else if ((length - index) > 4 && requirements.Substring(index, 5).ToUpper() == "FALSE")
                {
                    function += "FALSE"; //Add expression to function
                    index += 5; //Increment index to go after false
                }
                //Check for items, either requiring a certain count or just the item
                else if ((length - index) > 7 && requirements.Substring(index, 4).ToUpper() == "HAS(") // Has(item,x) indicates that x number of item is required. Ex Has(Key,2) indicates 2 keys are required. Minimum length 8: Has(x,y)
                {
                    //Calculations are just to extract item and num from Has(Item, Num) and then check if itempool contains num count of item
                    int startindex = requirements.IndexOf("(", index); //Should be index + 3
                    int endindex = requirements.IndexOf(",", index);
                    int numindex = requirements.IndexOf(")", index);
                    string item = requirements.Substring(startindex + 1, endindex - startindex - 1); //Parse item name from string
                    int num = int.Parse(requirements.Substring(endindex + 1, numindex - endindex - 1)); //Parse required number from string
                    string result = (owneditems.Where(x => x.Name == item).Count() >= num).ToString(); //true or false string
                    function += result; // Append result to function
                    index = numindex + 1; //Set index to after HAS statement
                }
                else //Not a logical statement, parenthesis, or multi-item statement, at this point can only assume it is 
                {
                    int endindex = ItemEndIndex(requirements, index);
                    string item = requirements.Substring(index, endindex - index);
                    string result = (owneditems.Where(x => x.Name == item).Count() > 0).ToString();
                    function += result; //Append result to function
                    index = endindex; //Set index to after item name
                }
            }
            //After going through string, pop any remaining tokens in stack to end of function string
            while(tokens.Count > 0)
            {
                function += tokens.Pop();
            }
            return function;
        }

        //Interpret output of shunting yard algorithm to produce final result
        public bool FunctionInterpreter(string function)
        {
            int index = 0;
            int length = function.Length;
            Stack<bool> st = new Stack<bool>();
            while (index < length) //Index step size is variable based on token
            {
                //First two possibilities for operators
                if ((length - index) > 2 && function.Substring(index, 3).ToUpper() == "AND")
                {
                    bool operand2 = st.Pop();
                    bool operand1 = st.Pop();
                    st.Push(operand1 && operand2);
                    index += 3;
                }
                else if ((length - index) > 1 && function.Substring(index, 2).ToUpper() == "OR")
                {
                    bool operand2 = st.Pop();
                    bool operand1 = st.Pop();
                    st.Push(operand1 || operand2);
                    index += 2;
                }
                //Next two possibilities for values
                else if ((length - index) > 3 && function.Substring(index, 4).ToUpper() == "TRUE")
                {
                    st.Push(true);
                    index += 4;
                }
                else if ((length - index) > 4 && function.Substring(index, 5).ToUpper() == "FALSE")
                {
                    st.Push(false);
                    index += 5;
                }
                else if (function[index] == ' ') //Just a space, move to next index
                {
                    index++;
                }
            }
            return st.Pop();
        }

        //Take a boolean expression with items as variables and simplify according to boolean simplification rules
        public string Simplify(string requirements)
        {
            if(requirements == "true") //Very common case
            {
                return "1";
            }
            //Run python script on requirements string to simplify
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c python ../../../simplify.py \"" + requirements + "\"";
            if(p.StartInfo.Arguments.Length > 2030) //Max command line size reached, have to supply file instead
            {
                File.WriteAllText("temprequirements.txt", requirements);
                p.StartInfo.Arguments = "/c python ../../../simplify.py temprequirements.txt";
            }
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output.Trim();
        }

        //Calculates a score for an individual rule based on its composition
        public double CalcRuleScore(string rule)
        {
            double score = 1; //Base score for each location is 1
            if (!(rule == "1")) //If false, can reach from beginning of game, just use base scoer
            {
                int index = 0;
                int length = rule.Length;
                while (index < length) //Index step size is variable based on token
                {
                    if (rule[index] == '(' || rule[index] == ')') //Just a parenthesis, move to next index
                    {
                        index++;
                    }
                    //Instead of checking for arithmetic operands, check for logical operands
                    else if (rule[index] == '&')
                    {
                        score += .5; //Add .5 to score for each AND since they add complexity
                        index++;
                    }
                    else if (rule[index] == '|')
                    {
                        score -= .5; //Subtract .5 from score for each OR since they reduce complexity
                        index++;
                    }
                    //Check for items, either requiring a certain count or just the item
                    else if ((length - index) > 7 && rule.Substring(index, 4).ToUpper() == "HAS(") // Has(item,x) indicates that x number of item is required. Ex Has(Key,2) indicates 2 keys are required. Minimum length 8: Has(x,y)
                    {
                        int startindex = rule.IndexOf("(", index); //Should be index + 3
                        int endindex = rule.IndexOf(",", index);
                        int numindex = rule.IndexOf(")", index);
                        int num = int.Parse(rule.Substring(endindex + 1, numindex - endindex - 1)); //Parse required number from string

                        score += num; //Add 1 to complexity for each requirement. So Has(Key,2) adds 2 complexity
                        index = numindex + 1; //Set index to after HAS statement
                    }
                    else //Not a logical statement, parenthesis, or multi-item statement, at this point can only assume it is single var
                    {
                        int endindex = ItemEndIndex(rule, index);
                        score++; //Add 1 to score for each item
                        index = endindex; //Set index to after item name
                    }
                }
            }
            return score;
        }
        
        //Gets the end index for an item
        // Can either be space, ex. Sword and x"
        // Right parenthesis, ex. Sword)"
        // Or the end of the string, ex. Sword"
        private int ItemEndIndex(string requirements, int index)
        {
            //Get index of things that could be the end of the item name
            List<int> indexlist = new List<int>();
            indexlist.Add(requirements.IndexOf(' ', index));
            indexlist.Add(requirements.IndexOf(')', index));
            indexlist.Add(requirements.IndexOf('&', index));
            indexlist.Add(requirements.IndexOf('|', index));
            if(indexlist.Where(x => x > -1).Count() == 0) 
            {
                return requirements.Length; //If none of the possibilities are ahead, this is the last term in the string and has no parenthesis, go to end of string

            }
            return indexlist.Where(x => x > -1).Min(); //Else get the minimum of the existing indices
        }

        //Used to determine precedence of operators
        //And has highest precedence, followed by or, followed by everything else
        private int precedence(string oper)
        {
            oper = oper.ToUpper();
            if (oper == "AND")
            {
                return 3;
            }
            else if (oper == "OR")
            {
                return 2;
            }
            return -1;
        }
    }
}
