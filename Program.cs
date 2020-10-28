using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace MediaLibrary
{
    class Program
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {

            logger.Info("Program started");

            string scrubbedFile = FileScrubber.ScrubMovies("movies.csv");
            logger.Info(scrubbedFile);

            string movieFilePath = Directory.GetCurrentDirectory() + "\\movies.scrubbed.csv";

            MovieFile movieFile = new MovieFile(movieFilePath);
            string choice = "";
            do
            {
                Console.WriteLine("1) Add Movie");
                Console.WriteLine("2) Display All Movies");
                Console.WriteLine("3) Search Movies");
                Console.WriteLine("Enter to quit");

                choice = Console.ReadLine();
                logger.Info("User choice: {Choice}", choice);
                
                if (choice == "1")
                {
                    Movie movie = new Movie();

                    Console.WriteLine("Enter movie title");
                    movie.title = Console.ReadLine();

                    if (movieFile.isUniqueTitle(movie.title)){
                        Console.WriteLine("Movie title is unique\n");
  
                        string input;
                        do
                        {
                            Console.WriteLine("Enter genre (or done to continue)");
                            input = Console.ReadLine();
                            if (input != "done" && input.Length > 0)
                            {
                                movie.genres.Add(input);
                            }
                        }while (input != "done");

                        if (movie.genres.Count == 0)
                        {
                            movie.genres.Add("(no genres listed)");
                        }
                        
                        Console.WriteLine("\nEnter movie director");
                        movie.director = Console.ReadLine();

                        Console.WriteLine("\nEnter running time (h:m:s)");
                        string runTime = Console.ReadLine();
                        movie.runningTime = TimeSpan.Parse(runTime);

                        movieFile.AddMovie(movie);
                    }
                } else if (choice == "2")
                {
                    foreach(Movie m in movieFile.Movies)
                    {
                        Console.WriteLine(m.Display());
                    }
                } else if (choice == "3")
                {
                    Console.WriteLine("Enter movie title");
                    string searchInput = Console.ReadLine();
                    var titleSearch = movieFile.Movies.Where(m => m.title.Contains(searchInput)).Select(m => m.title);
                    Console.WriteLine($"There are {titleSearch.Count()} movies that match your search:");
                    foreach(string m in titleSearch)
                    {
                        Console.WriteLine($"  {m}");
                    }
                    
                }
            } while (choice == "1" || choice == "2" || choice == "3");

            logger.Info("Program ended");
        }
    }
}