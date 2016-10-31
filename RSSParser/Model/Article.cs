using System;

using SQLite;

namespace RSSParser.Model
{
    [Table("article")]
    public class Article
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public bool Favorite { get; set; }

        public string ImageUri { get; set; }

        public string Link { get; set; }

        public string Title { get; set; }
    }
}