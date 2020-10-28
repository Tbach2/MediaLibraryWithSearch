using System;
using NLog.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MediaLibrary
{
    class MovieFile
    {
        public string listing { get; set; }
        public List<Movie> Movies { get; set; }
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();

        public MovieFile(string movieFileListing)
        {
            listing = movieFileListing;
            Movies = new List<Movie>();

            try
            {
                StreamReader sr = new StreamReader(listing);
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    Movie movie = new Movie();
                    string line = sr.ReadLine();
                    int idx = line.IndexOf('"');
                    if (idx == -1)
                    {
                        string[] movieDetails = line.Split(',');
                        movie.mediaId = UInt64.Parse(movieDetails[0]);
                        movie.title = movieDetails[1];
                        movie.genres = movieDetails[2].Split('|').ToList();
                        movie.director = movieDetails[3];
                        movie.runningTime = TimeSpan.Parse(movieDetails[4]);
            
                    }
                    else
                    {
                        movie.mediaId = UInt64.Parse(line.Substring(0, idx - 1));

                        line = line.Substring(idx + 1);

                        idx = line.IndexOf('"');

                        movie.title = line.Substring(0, idx);

                        line = line.Substring(idx + 2);
                        movie.genres = line.Split('|').ToList();
                    }
                    Movies.Add(movie);
                }
                sr.Close();
                logger.Info("Movies in file {Count}", Movies.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public bool isUniqueTitle(string title)
        {
            if (Movies.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate movie title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddMovie(Movie movie)
        {
            try
            {
                movie.mediaId = Movies.Max(m => m.mediaId) + 1;
                StreamWriter sw = new StreamWriter(listing, true);
                sw.WriteLine($"{movie.mediaId},{movie.title},{string.Join("|", movie.genres)},{movie.director},{movie.runningTime}");
                sw.Close();
                Movies.Add(movie);
                logger.Info("Movie id {Id} added", movie.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}