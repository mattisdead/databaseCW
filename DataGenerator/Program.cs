using System;
using static System.Console;
using System.IO;
using Npgsql;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                WriteLine("What data would you like to generate?");
                string command = ReadLine();

                if (command == "students" || command == "teachers")
                {
                    string allNames = File.ReadAllText("../data/names.txt");
                    string allSurnames = File.ReadAllText("../data/surnames.txt");

                    string[] arrNames = allNames.Split("\r\n");
                    string[] arrSurnames = allSurnames.Split("\r\n");

                    Random r = new Random();

                    string name = arrNames[r.Next(0, arrNames.Length)];
                    string surname = arrSurnames[r.Next(0, arrSurnames.Length)];

                    WriteLine("Meet " + name + " " + surname + "!");
                }
                else if (command == "marks")
                {
                    Random r = new Random();

                    int perc = r.Next(0, 101);
                    int mark = -1;

                    if (perc < 10)
                    {
                        mark = r.Next(0, 60);
                    }
                    else if (perc < 90)
                    {
                        mark = r.Next(60, 90);
                    }
                    else
                    {
                        mark = r.Next(90, 101);
                    }

                    WriteLine(mark);
                }
                else if (command == "subjects")
                {
                    string allSubjects = File.ReadAllText("../data/subjects.txt");
                    string[] arrSubjects = allSubjects.Split("\r\n");

                    Random r = new Random();

                    string subject = arrSubjects[r.Next(0, arrSubjects.Length)];

                    WriteLine(subject);
                }
            }
        }
    }
}
