using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace apportionment
{
    class Program
    {
        const int totalSeats = 435;

        static Dictionary<string,int> pops;
        static Dictionary<string,int> seats = new Dictionary<string,int>();
        static List<string> forFile = new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            int count = 1; //1-indexed for human readability of output

            //Get the population data from file
            pops = File.ReadLines("./statePop.csv").ToDictionary(x=>x.Split(',')[0],x=>Convert.ToInt32(x.Split(',')[1]));
            IEnumerable<string> stateList = pops.Keys.ToList();

            //Give each state one seat
            foreach (string s in pops.Keys)
            {
                seats.Add(s,1);
                log(count,s);
                count++;
            }

            while (count <= totalSeats) //still 1-indexed
            {
                stateList = stateList.OrderByDescending(x=>priority(x));
                seats[stateList.First()] += 1;
                log(count,stateList.First());
                count++;
            }
            //Write final seats
            File.WriteAllLines("./seats.csv",seats.OrderByDescending(x=>x.Value).Select(x=>$"{x.Key},{x.Value}"));
            Console.WriteLine("----------------------------------------------");
            forFile.Add("");
            //Compute what would have been the next 10 allocations
            for (int i = 0; i < 10; i++)
            {
                stateList = stateList.OrderByDescending(x=>priority(x));
                seats[stateList.First()] += 1;
                log(count,stateList.First());
                count++;
            }

            //Write output
            File.WriteAllLines("./allocationOrder.csv",forFile);

        }
        ///Returns the priority for a state
        static double priority(string state)
        {
            return pops[state]/Math.Sqrt(seats[state]*(seats[state]+1));
        }

        static void log(int num, string state)
        {
            forFile.Add($"{num},{state}");
            Console.WriteLine($"Seat {num}:\t{state}");
        }
    }
}
