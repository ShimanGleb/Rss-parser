using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using System.IO;

using Android.Support.V4.Widget;
using static Android.Support.V4.Widget.SwipeRefreshLayout;

using RSSParser.Code;
using RSSParser.Model;
using RSSParser.SQLiteManager;

namespace RSSParser
{
    [Activity(Label = "RSSParser", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnRefreshListener
    {
        private List<Article> _articles = new List<Article>();
        
        private ConnectionManager _manager = new ConnectionManager();

        private SwipeRefreshLayout _refresh = null;

        private string _sourceUri = "http://news.tut.by/rss/all.rss";

        private ListView _listView = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Main);            

            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "article.db");
            
            _manager.CreateDatabase(path);

            Parser.Parse(_sourceUri, _manager);

            _articles = _manager.GetArticles();

            _listView = FindViewById<ListView>(Resource.Id.articleList);
            
            UpdateListAdapter(_articles);

            CheckBox favoriteBox = FindViewById<CheckBox>(Resource.Id.favoriteBox);
            
            favoriteBox.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                if (favoriteBox.Checked)
                {
                    List<Article> favoriteArticles = new List<Article>();

                    for (int i = 0; i < _articles.Count; i++)
                    {
                        if (_articles[i].Favorite) favoriteArticles.Add(_articles[i]);
                    }

                    UpdateListAdapter(favoriteArticles);
                }
                else
                {
                    UpdateListAdapter(_articles);
                }
            };

            _refresh = (SwipeRefreshLayout)FindViewById(Resource.Id.refreshLayout);

            _refresh.SetOnRefreshListener(this);
        }

        public void OnRefresh()
        {
            string lastTitle = _manager.GetLastArticle().Title;

            Parser.UpdateDatabase(_sourceUri, _manager);

            _articles = _manager.GetArticles();

            UpdateListAdapter(_articles);

            _refresh.Refreshing = false;
        }

        public void UpdateListAdapter(List<Article> articles)
        {            
            _listView.Adapter = new Adapter.RSSReaderAdapter(articles, Application.Context, _manager, this);
        }
    }
}

