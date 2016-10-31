using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

using AndroidSwipeLayout;

using RSSParser.Code;
using RSSParser.SQLiteManager;

namespace RSSParser.Model
{
    public class ArticleView : FrameLayout
    {
        private Activity _activity;

        private Article _article;

        private SwipeLayout _swipeLayout;

        private ImageView _favoriteView;

        public ArticleView(Article article, Activity activity, ConnectionManager connectionManager, View layout) : base(activity)
        {          
            _activity = activity;

            AddView(layout);
            ConfigureView(article);
            ConfigureSwipe(connectionManager);
            SetFavoriteViewEvent(connectionManager);
        }

        private void SetFavoriteViewEvent(ConnectionManager connectionManager)
        {
            _favoriteView.Click += delegate
            {
                if (_article.Favorite)
                {
                    _article.Favorite = false;
                    connectionManager.MarkFavorite(false, _article.ID);
                    _favoriteView.SetImageResource(Resource.Drawable.EmptyStar);
                }
                else
                {
                    _article.Favorite = true;
                    connectionManager.MarkFavorite(true, _article.ID);
                    _favoriteView.SetImageResource(Resource.Drawable.Star);
                }
            };
        }

        private void ConfigureSwipe(ConnectionManager connectionManager)
        {
            _swipeLayout = FindViewById<SwipeLayout>(Resource.Id.swipeLayout);
            
            _swipeLayout.SetShowMode(SwipeLayout.ShowMode.PullOut);

            var addFavoriteView = FindViewById(Resource.Id.leftBottomWrapper);
            _swipeLayout.AddDrag(SwipeLayout.DragEdge.Left, addFavoriteView);

            var removeFavoriteView = FindViewById(Resource.Id.rightBottomWrapper);
            _swipeLayout.AddDrag(SwipeLayout.DragEdge.Right, removeFavoriteView);

            View backgroundView = FindViewById<Button>(Resource.Id.addToFavorite);

            _swipeLayout.Opened += (sender, e) => {
                if (_swipeLayout.CurrentBottomView.FindViewById<Button>(Resource.Id.addToFavorite) != null)
                {
                    _favoriteView.SetImageResource(Resource.Drawable.Star);
                    backgroundView = FindViewById<Button>(Resource.Id.addToFavorite);
                    connectionManager.MarkFavorite(true, _article.ID);
                    _article.Favorite = true;
                }
                else
                {
                    _favoriteView.SetImageResource(Resource.Drawable.EmptyStar);
                    backgroundView = FindViewById<Button>(Resource.Id.removeFromFavorite);
                    connectionManager.MarkFavorite(false, _article.ID);
                    _article.Favorite = false;
                }

                backgroundView.SetBackgroundColor(Color.Green);

                _swipeLayout.Close();
            };

            _swipeLayout.Closed += (sender, e) =>
            {
                backgroundView.SetBackgroundColor(Color.Transparent);
            };

            _swipeLayout.Hover += (sender, e) =>
            {
                _swipeLayout.Close();
            };

            _swipeLayout.Click += delegate {
                string swipeStatus = _swipeLayout.OpenStatus.ToString();
                if (!swipeStatus.Equals("Open") && !swipeStatus.Equals("Middle"))
                {
                    Intent intent = new Intent(Context, typeof(ArticleActivity));
                    intent.PutExtra("sourceUri", _article.Link);
                    _activity.StartActivity(intent);
                }
                _swipeLayout.Close();
            };

        }

        private void ConfigureFavoriteView()
        {
            _favoriteView = FindViewById<ImageView>(Resource.Id.articleFavorite);
            
            if (_article.Favorite)
            {
                _favoriteView.SetImageResource(Resource.Drawable.Star);
            }
            else
            {
                _favoriteView.SetImageResource(Resource.Drawable.EmptyStar);
            }            
        }


        private void SetImage()
        {
            if (!_article.ImageUri.Equals("None"))
            {
                Bitmap image = ImageLoader.GetImage(_activity.CacheDir.Path, _article.ImageUri);

                FindViewById<ImageView>(Resource.Id.articleImage).SetImageBitmap(image);
            }
        }


        public void ConfigureView(Article article)
        {
            _article = article;

            TextView titleView = FindViewById<TextView>(Resource.Id.articleTitle);            
            titleView.Text = _article.Title;

            TextView descriptionView = FindViewById<TextView>(Resource.Id.articleDescription);
            descriptionView.Text = _article.Description;

            SetImage();

            ConfigureFavoriteView();            
        }
    }
}