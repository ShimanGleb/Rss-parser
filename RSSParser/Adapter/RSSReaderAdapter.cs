using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

using RSSParser.Model;
using RSSParser.SQLiteManager;

namespace RSSParser.Adapter
{
    public class RSSReaderAdapter : BaseAdapter<Article>
    {       
        private Activity _activity;

        private ConnectionManager _connectionManager;

        private Context _context;

        private List<Article> _items;

        public RSSReaderAdapter(List<Article> items, Context context, ConnectionManager connectionManager, Activity activity) : base()
        {
            _items = items;
            _context = context;
            _connectionManager = connectionManager;
            _activity = activity;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Article this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = (ArticleView)convertView;

            if (view == null)
            {
                LayoutInflater _inflatorservice = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                var layout = _inflatorservice.Inflate(Resource.Layout.ArticleListItem, null);
                
                view = new ArticleView(_items[position], _activity, _connectionManager, layout);                
                return view;
            }

            view.ConfigureView(_items[position]);
           
            return view;       
        }

    }
}