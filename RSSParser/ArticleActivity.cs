using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;

namespace RSSParser
{
    [Activity(Label = "RSSParser", Icon = "@drawable/icon")]
    public class ArticleActivity : Activity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Article);

            string sourceUri = "";

            sourceUri = Intent.GetStringExtra("sourceUri");

            WebView webView = FindViewById<WebView>(Resource.Id.articleView);

            webView.Settings.JavaScriptEnabled = true;

            webView.SetWebViewClient(new WebViewClient());

            webView.LoadUrl(sourceUri);
        }
    }
}