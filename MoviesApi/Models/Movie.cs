﻿using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        public int Year { get; set;}
        public double Rate { get; set; }
        [MaxLength(300)]
        public string Storeline { get; set; }
        public byte[] Poster { get; set; }
        public byte GenreId { get; set; }
        public Genre Genre { get; set;}

    }
}
