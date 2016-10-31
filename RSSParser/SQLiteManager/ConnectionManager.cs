using System.Collections.Generic;
using System.Linq;

using SQLite;

using RSSParser.Model;

namespace RSSParser.SQLiteManager
{
    public class ConnectionManager
    {
        private SQLiteConnection db;

        public string CreateDatabase(string path)
        {

            db = new SQLiteConnection(path);
            try
            {
                db.Query<Article>("Drop table article");
            }
            catch
            {

            }
            db.CreateTable<Article>();
            return "Database created";

        }

        public string AddArticle(Article article)
        {            
            db.Insert(article);
            db.Update(article);
            return "Update: success";
        }
        
        public List<Article> GetArticles()
        {
            List<Article> articles = new List<Article>();

            var items = db.Query<Article>("select * from article");

            articles = db.Table<Article>().ToList();
            articles.Sort((a, b) => b.Date.CompareTo(a.Date));
            return articles;
        }

        public void MarkFavorite(bool favorite, int articleId)
        {
            if (favorite)
            {
                db.Query<Article>("UPDATE article SET Favorite = '1' WHERE id = " + articleId + ";");
            }
            else
            {
                db.Query<Article>("UPDATE article SET Favorite = '0' WHERE id = " + articleId + ";");
            }

            db.Update(new Article());                  
        }

        public bool ConfirmArticlePresence(string title)
        {
            if (db.Query<Article>("SELECT * from article where   Title = '"+ title +"';").ToList().Count > 0)
            {
                return true;
            }

            return false;
        }

        public Article GetLastArticle()
        {
            Article article = new Article();

            var items = db.Query<Article>("select * from article where   ID = (SELECT MIN(ID)  FROM article);");

            article = db.Table<Article>().ToList()[0];

            return article;
        }
    }
}