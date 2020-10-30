using System;
using System.Collections.Generic;
using System.Collections;           //needed for arraylists
using System.Linq;  
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Larger_Project
{
    /// <summary>
    /// Creates a class with 3 fields for use as the value in a dictionary, x-y coordinates, and a boolean value denoting 'life'.
    /// </summary>
    public class Coordinates
    {
        public bool Mortality;
        public int X;
        public int Y;

        //methods
        /*public override string ToString()       // Allows the class to be written to console. useful in early testing, now defunct
        {
            return this.X.ToString() + " " + this.Y.ToString() ;        
        }*/
        //constructors(default and non-default)
        public Coordinates()
        {
            bool Mortality;
            int X, Y;
        }
        public Coordinates(int x, int y, bool mortality)
        {
            X = x;
            Y = y;
            bool Mortality = mortality;
        }
    }
    /// <summary>
    /// Class containing useful functions for manipulating the custom dictionary.
    /// </summary>
    class MyYHashtable
    {
        public Dictionary<int, Coordinates> Iteration;  //contains a normal <int> key and a unique value consisting of a bool, and 2 <int>s

        public Object this[int index]                   //standard indexer
        {
            get { return Iteration[index-1].Mortality; }
            set { Iteration[index] = (Coordinates)value; }
        }
        /*public override string ToString()               //allows the class to be written to console. Now defunct.
        {
            return Iteration.Keys.ToString() + " " + Iteration.Values.ToString();
        }*/
        public void Add(int key, int x, int y, bool mortality)  //provides an easy manner for loading values into the dictionary.
        {
            Coordinates coord = new Coordinates();
            coord.X = x;
            coord.Y = y;
            coord.Mortality = mortality;
            this.Iteration.Add(key, coord);
        }
        public MyYHashtable Clone(MyYHashtable master)      //provides functionality for duplicating this class object, rather than copying a reference value. Could use some work.
        {
            MyYHashtable slave = new MyYHashtable();
            foreach(int i in master.Iteration.Keys) //actually make a new class object and loads each key-value pair individually. Could be improved, but it works for now
            {
                slave.Add(i, master.Iteration[i].X, master.Iteration[i].Y, master.Iteration[i].Mortality);
            }
            return slave;
        }
        public bool Toggle(MyYHashtable dict,int i)         //toggles the bool value for determining the 'life' of a coordinate point.
        {
            dict.Iteration[i].Mortality = !dict.Iteration[i].Mortality;
            return dict.Iteration[i].Mortality;
        }
        //constructors(default and non-default)
        public MyYHashtable(Dictionary<int,Coordinates> gridToCopy)
        {
            Iteration = new Dictionary<int, Coordinates>();
        }
        public MyYHashtable()
        {
            Iteration = new Dictionary<int, Coordinates>();
        }
    }
    class PartyMode
    {
        private static bool party = false;   //literally just exists for partymode, the alternative logical gambit was too horrible to consider.
        public static bool Party
        {
            get
            {
                return party;
            }
            set
            {
                party = value;
            }
        }
    }
    class Program
    {
        //enum exists to provide functionality to an if-else statement. Really just here to check a box for grading purposes.
        public enum FunctionChoice { Compose = 1, Build = 2, Random = 3, party = 5}
        static void Main(string[] args)
        {
            Console.SetWindowSize(52, 52);                          //sets the console window to 1 cell larger than my maximum values. If I could adjust cell SIZE, I would. standby
            MyYHashtable MasterGrid = new MyYHashtable();           //initializes my custom dictionary essentially as a grid; ignore the name, too tedious to change now. big lessons**
            MasterGrid = BuildMasterGrid(MasterGrid);               //populates that grid with coordinates and dead cell values.

            bool condition = true;                                  //a few variables to drive the main function
            string seedName;
            while (condition)
            {
                Console.WriteLine("1 - Compose a seed for textfile archiving.\n2 - Build a seed from file.\n3 - Create random seed file"); //choose which function to perform
                string response = Console.ReadLine();         //determines response
                Int32.TryParse(response, out int answer);
                if (answer == (int)FunctionChoice.Compose)  //compose seed
                {
                    Console.WriteLine("Input name of seed");
                    seedName = Console.ReadLine();                          //sets new seed file's name
                    GenerateFileFromConsole(seedName);                      //calls method to generate that file
                    Console.WriteLine("File Created, reload and test");     //no functionality to return to loop, should add                                                        **
                }
                else if (answer == (int)FunctionChoice.Build) //build seed
                {
                    seedName = DetermineSeed();                             //redundant comment dictating the line of code determines a which seed 
                    Console.Clear();
                    MasterGrid = GenerateSeedFromFile(MasterGrid, seedName);//calls method to alter bool values at specific coordinates
                    DrawIteration(MasterGrid);                              //all purpose drawing method, called
                    StepThrough(MasterGrid);
                }
                else if (answer == (int)FunctionChoice.Random)     //Create random seed file
                {

                    Console.WriteLine("Enter name for new random file. Format: 'name'");
                    seedName = Console.ReadLine();
                    CreateRandomSeed(seedName);
                }
                else if (answer == (int)FunctionChoice.party || response == ((FunctionChoice)5).ToString())
                {
                    Console.WriteLine("Party... or party hardy?");
                    Console.ReadLine();
                    PartyMode.Party = !PartyMode.Party;
                }
                Console.Clear();
                Console.WriteLine("Press 'X' to terminate.\nPress 'R' to return to main options menu.");
                if (Console.ReadLine().ToLower() != 'r'.ToString())         //keeps looping to use program, or exits
                    condition = false;
            }
        }
        /// <summary>
        /// Returns primary grid to all 'dead' values
        /// </summary>
        /// <param name="old"></param>
        public static void ReturnToDefault(MyYHashtable old)
        {
            foreach (int i in old.Iteration.Keys)
                if (old.Iteration[i].Mortality == true)
                    old.Toggle(old, i);
        }
        /// <summary>
        /// Iterates through a pre-built grid
        /// </summary>
        /// <param name="grid"></param>
        public static void StepThrough(MyYHashtable grid)
        {
            bool condition = true;
            while(condition)                                        //can cycle through the iterations as long as desired
            {
                if (Console.ReadLine() == "x".ToString())           // 'x' to escape
                    condition = false;
                grid = CalculateIteration(grid);                 //calculates the next iteration.
            }
        }
        /// <summary>
        /// Creates random seed and stores to a user-generated filename
        /// </summary>
        /// <param name="old"></param>
        /// <param name="seedName"></param>
        /// <returns></returns>
        public static void CreateRandomSeed(string seedName)
        {
            //obvious improvement: set parameters for random conditions
            
            StreamWriter filename = new StreamWriter(seedName + ".txt");
            Random randGen = new Random();
            for(int i = 1; i <=2500; i++)                   //arbitrary condition for life, 3/5 chance at spawning initially
                if (randGen.Next(1, 5) >= 3)
                {
                    filename.WriteLine(i);                          //places index into file for use later
                }

            filename.Close();
            StreamWriter fileSeed = new StreamWriter("SeedList.txt",true);
            fileSeed.WriteLine(seedName);
            fileSeed.Close();
        }
        /// <summary>
        /// Asks user which seed they want to populate onto the screen.
        /// </summary>
        /// <returns></returns>
        public static string DetermineSeed()
        {
            Console.WriteLine("Which seed to start?");
            StreamReader fileRead = new StreamReader("SeedList.txt");
            int i = 0;
            List<string> arr = new List<string>();
            while(!fileRead.EndOfStream)
            {
                arr.Add(fileRead.ReadLine());
                Console.WriteLine(i+" " + arr[i]);
                i++;
            }
            fileRead.Close();
            return (arr[(Convert.ToInt32(Console.ReadLine()))]);    //this mess literally just returns the selected enum value as a string to be the file name
        }
        /// <summary>
        /// Selects a txtfile and populates the master grid/dictionary with values
        /// </summary>
        /// <param name="old"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static MyYHashtable GenerateSeedFromFile(MyYHashtable old, string seed)
        {
            StreamReader file = new StreamReader(seed + ".txt"); //starts to read selected file
            List<int> arr1 = new List<int>();
            while (!file.EndOfStream)
                arr1.Add(Convert.ToInt32(file.ReadLine()));    //reads the file and finds KEYS to change the bool value 
            ReturnToDefault(old);
            foreach (int i in arr1)
            {
                old.Toggle(old, i);                         //cycles through that array and modifies the grid accordingly
            }

            file.Close();                                     //close file
            return old;                                      
        }
        /// <summary>
        /// This method creates a .txt file given input from the console. Adds that filename to a master list of filenames 
        /// </summary>
        /// <param name="seed"></param>
        public static void GenerateFileFromConsole(string seed)
        {
            StreamWriter file = new StreamWriter(seed + ".txt");        //user input name for the file
            bool condition = true;
            Console.WriteLine("Add int values. Hit 'x' to conclude");
            while(condition)                                            //input can be as long as user likes, important to note: index location, not actual cell coordinate.
            {
                try
                {
                    file.WriteLine(Convert.ToInt32(Console.ReadLine()));    //writes to file each key
                }
                catch(FormatException)
                {
                    condition = false;                                      //if not an int, escapes
                }
            }
            file.Close();                                                   //file close
            StreamWriter seedListFile = new StreamWriter("SeedList.txt",true);    //adds the generated filename to a list of seed file names
            seedListFile.WriteLine(seed);
            seedListFile.Close();
        }
        /// <summary>
        /// Code for composing the seed for a PentaDecathlonSeed. Now defunct
        /// </summary>
        /// <param name="old"></param>
        /// <returns></returns>
        public static MyYHashtable PentaDecathlonSeed(MyYHashtable old)
        {
            MyYHashtable new1 = new MyYHashtable();             //brute force method of creating a certain seed, used in testing and left in for demonstration purposes.
            new1 = new1.Clone(old);
            int[] arr1 = new int[] {124,125,126,175,225,274,275,276,374,375,376,424,425,426,524,525,526,575,625,674,675,676};
            foreach (int i in arr1)
            {
                new1.Toggle(new1, i);
            }
            return new1;
        }
        /// <summary>
        /// Constructs Mastergrid of all cell values/coordinates initalized to 'dead'
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static MyYHashtable BuildMasterGrid(MyYHashtable grid)
        {
            int i = 1;
            for (int x = 1; x <= 50; x++)               //literally creates the 2500 dead cells, can easily be modified to be larger/smaller in future versions
            {
                for (int y = 1; y <= 50; y++, i++)
                {
                    grid.Add(i, x, y, false);
                }
            }
            return grid;
        }
        /// <summary>
        /// Draws a single cell onto the console
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DrawOne(int y, int x)
        {
            int colour = 15;            //initialized to white, allowing standard use in the event partmode isnt active(rare)
            if (PartyMode.Party)        //if global variable party is active, give random number 0-15
            {
                Random randGen = new Random();
                colour = randGen.Next(0, 16);
            }
            Console.SetCursorPosition(x,y);
            Console.BackgroundColor = (ConsoleColor)colour;     //0-15 correlates with a specific console colour.
            Console.WriteLine(" ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        /// <summary>
        /// Returns to black the spaces no longer occupied with life.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        public static void UnDraw(int y, int x)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ");
        }
        /// <summary>
        /// Guides drawing the entire new iteration
        /// </summary>
        /// <param name="lastIteration"></param>
        public static void DrawIteration(MyYHashtable newIteration)
        {
            foreach (var i in newIteration.Iteration.Keys)
            {
                if (newIteration.Iteration[i].Mortality)
                    DrawOne(newIteration.Iteration[i].X, newIteration.Iteration[i].Y);//separate from DrawOne due to early testing requirements; not strictly needed, but organizes code
            }
        }
        /// <summary>
        /// Calculates the new iteration from the previous.
        /// </summary>
        /// <param name="lastIteration"></param>
        /// <returns></returns>
        public static MyYHashtable CalculateIteration(MyYHashtable masterGrid)
        {
                                                                //most complicated bit of logic, though significantly reduced from how it began
            MyYHashtable slaveGrid = new MyYHashtable();        //creates new table to compare master against such that all changes occur immediately upon evolution of iteration
            slaveGrid = slaveGrid.Clone(masterGrid);                                                     
            int[] arr1 = new int[] { -51, -50, -49, -1, 1, 49, 50, 51 };    //array of concrete values correlating to the distance of the 8 blocks surrounding any individual cell
                                                                            //this is poor form, as it will not change with an increased/decreased field size. Future update worthy.
            foreach (int i in masterGrid.Iteration.Keys)//for each key
            {
                int count = 0;
                foreach (int x in arr1)                 //for each foreign cell in proximity to current selected
                {
                    try
                    {
                        if (masterGrid.Iteration[(i + x)].Mortality == true)        //assess if foreign cell is alive
                            count++;                                                //increment if so
                    }
                    catch (KeyNotFoundException) { }   //if out of bounds(for our grid), it cannot be alive/dead, thus is ignored. This causes an issue with bounding around the edges**
                }
                if (count == 3 && masterGrid.Iteration[i].Mortality == false)       //if selected cell is dead, and has 3 living neighbors
                    slaveGrid.Toggle(slaveGrid, i);                                 //bloom to life
                else if ((count <= 1 || count >= 4) && masterGrid.Iteration[i].Mortality == true)   //if selected cell is alive and has too many/few neighbors
                {
                    slaveGrid.Toggle(slaveGrid, i);                                                 //cell dies
                    UnDraw(slaveGrid.Iteration[i].X, slaveGrid.Iteration[i].Y);                     //and is undrawn
                }
            }
            DrawIteration(slaveGrid);                                                               //draws new iteration
            return slaveGrid;                                                                       //replaces the master with the slave table with all changes made properly
        }

    }
}