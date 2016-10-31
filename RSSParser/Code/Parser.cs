using System;
using System.Net;

using RSSParser.Model;
using RSSParser.SQLiteManager;

namespace RSSParser.Code
{
    public static class Parser
    {
        public static void Parse(string sourceUri, ConnectionManager manager)
        {
            WebClient webClient = new WebClient();

            string[] data = webClient.DownloadString(new Uri(sourceUri)).Split('\n');

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i].Split('\r')[0];
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Equals("\t<item>"))
                {
                    Article article = new Article();

                    article.Title = data[i + 1].Split('\t')[2];

                    article.Title = article.Title.Substring(7, article.Title.Length - 15);

                    article.Link = data[i + 2].Split('\t')[2];

                    article.Link = article.Link.Substring(6, article.Link.Length - 13);

                    try
                    {
                        article.Description = data[i + 3].Split('&')[2];

                        article.Description = article.Description.Substring(5, article.Description.Length - 5);

                        if (article.Description.Split(';')[0].Equals("quot"))
                        {
                            article.Description = data[i + 3].Split('&')[4];

                            article.Description = article.Description.Substring(5, article.Description.Length - 5);
                        }
                    }
                    catch
                    {
                        article.Description = data[i + 3].Split('>')[1].Split('<')[0];

                        article.ImageUri = "None";
                    }

                    if (article.ImageUri != "None")
                    {
                        string imageSource = data[i + 3].Split('"')[1];

                        article.ImageUri = imageSource;
                    }

                    string[] date = data[i + 10].Split('>');

                    int offset = 0;
                    while (!date[0].Equals("\t\t<pubDate"))
                    {
                        date = data[i + 11 + offset].Split('>');
                        offset++;
                    }

                    date = date[1].Split('<');

                    article.Date = DateTime.Parse(date[0]);

                    manager.AddArticle(article);

                    i += 9;
                }
            }            
        }

        public static void UpdateDatabase(string sourceUri, ConnectionManager manager)
        {
            WebClient webClient = new WebClient();

            string[] data = webClient.DownloadString(new Uri(sourceUri)).Split('\n');

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i].Split('\r')[0];
            }

            for (int i = 0; i < data.Length / 10; i++)
            {
                if (data[i].Equals("\t<item>"))
                {
                    Article article = new Article();

                    article.Title = data[i + 1].Split('\t')[2];

                    article.Title = article.Title.Substring(7, article.Title.Length - 15);

                    if (manager.ConfirmArticlePresence(article.Title))
                    {
                        return;
                    }

                    article.Link = data[i + 2].Split('\t')[2];

                    article.Link = article.Link.Substring(6, article.Link.Length - 13);

                    try
                    {
                        article.Description = data[i + 3].Split('&')[2];

                        article.Description = article.Description.Substring(5, article.Description.Length - 5);

                        if (article.Description.Split(';')[0].Equals("quot"))
                        {
                            article.Description = data[i + 3].Split('&')[4];

                            article.Description = article.Description.Substring(5, article.Description.Length - 5);
                        }
                    }
                    catch
                    {
                        article.Description = data[i + 3].Split('>')[1].Split('<')[0];

                        article.ImageUri = "None";
                    }

                    if (article.ImageUri != "None")
                    {
                        string imageSource = data[i + 3].Split('"')[1];

                        article.ImageUri = imageSource;
                    }

                    string[] date = data[i + 10].Split('>');

                    int offset = 0;
                    while (!date[0].Equals("\t\t<pubDate"))
                    {
                        date = data[i + 11 + offset].Split('>');
                        offset++;
                    }

                    date = date[1].Split('<');

                    article.Date = DateTime.Parse(date[0]);

                    manager.AddArticle(article);

                    i += 9;
                }
            }
        }
    }
}