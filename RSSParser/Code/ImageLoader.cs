using Android.Graphics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RSSParser.Code
{
    public static class ImageLoader
    {
        private static string GetFilepath(string path, string filename)
        {
            string filepath = string.Format("{0}/{1}", path,
                    filename.Replace('/', '_').Replace('.', '_'));

            return filepath;
        }

        private static async void SaveImage(Bitmap image, string path, string fileName)
        {
            if (!File.Exists(GetFilepath(path, fileName)))
            {
                await Task.Run(()=>
                {
                    byte[] bitmapData;
                    using (var stream = new MemoryStream())
                    {
                        image.Compress(Bitmap.CompressFormat.Png, 0, stream);
                        bitmapData = stream.ToArray();
                    }

                    File.WriteAllBytes(GetFilepath(path, fileName), bitmapData);
                });
                
            }
        }

        public static Bitmap GetImage(string path, string fileName)
        {
            Bitmap image = null;

            byte[] imageBytes;

            if (File.Exists(GetFilepath(path,fileName)))
            {
                imageBytes = File.ReadAllBytes(GetFilepath(path, fileName));
            }
            else
            {
                WebClient webClient = new WebClient();

                imageBytes = webClient.DownloadData(fileName);
            }

            image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

            SaveImage(image, path, fileName);

            return image;
        }
    }
}