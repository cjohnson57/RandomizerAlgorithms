using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomizerAlgorithms
{
    //This class implements some miscellaneous functions used in the other classes
    //Most importantly is the shuffle method, which provides all the randomness for the fill algorithms
    class Helpers
    {

        private Random rng;

        public Helpers()
        {
            rng = new Random();
        }

        //Initialize helpers with seed
        public Helpers(int seed)
        {
            rng = new Random(seed);
        }

        //Shuffles list items
        //Should be done every time a list is initialized and popped from
        //Here is where all the RNG comes from
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        //Gets the first item from the list and removes it
        public T Pop<T>(IList<T> list)
        {
            T r = list[0];
            list.RemoveAt(0);
            return r;
        }

        //Gets a random element from a list, not used anywhere
        public T RandomElement<T>(IList<T> list)
        {
            return list[rng.Next(list.Count + 1)];
        }

        //Takes away redundant items from list subfrom which are already in toremove, not used anywhere
        public List<T> SubtractLists<T>(List<T> subfrom, List<T> toremove)
        {
            foreach(T elem in toremove)
            {
                subfrom.Remove(elem);
            }
            return subfrom;
        }

        //Returns a single char that represents a given number in [0, 25]
        //Ex 0 -> A, 1 -> B.... 25 -> Z
        //Used in world generation to generate item names
        private char NumToLetter(int num)
        {
            return (char)(65 + num);
        }

        //Returns a string that represents the given number
        //Ex 0 -> A, 1 -> B.... 25 -> Z, 26 -> AA, 27 -> AB.... 51 -> AZ, 52 -> BA....
        //Used in world generation to generate item names
        public string NumToLetters(int num)
        {
            if(num < 26)
            {
                return NumToLetter(num).ToString();
            }
            else
            {
                int dividedby = num / 26; //Gives the 2nd "digit" of the string
                int remainder = num % 26;
                return NumToLetter(dividedby-1).ToString() + NumToLetter(remainder);
            }
        }
    }
}
