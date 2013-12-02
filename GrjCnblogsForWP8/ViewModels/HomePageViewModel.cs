using GrjCnblogsForWP8.Model;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GrjCnblogsForWP8.ViewModels
{
    public class HomePageViewModel
    {

        HttpWebRequest request;
        //Home
        private bool homePageInitialized = false;
        private int homeMaxPage = 200;
        private int homeCurrentPage = 1;
        public ObservableCollection<Article> ArticleList { get; set; }

        public bool IsLoading { get; set; }
        public bool IsGettingNextPage { get; set; }


        private void llsHeader_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            Article article = border.DataContext as Article;
            if (article == null)
                return;
            if (string.IsNullOrEmpty(article.ArticleUri))
                return;
            string articleUri = Uri.EscapeDataString(article.ArticleUri);
            Uri uri = new Uri(string.Format("/Home/ArticleView.xaml?ArticleUri={0}", articleUri), UriKind.Relative);
            var root = App.Current.RootVisual as PhoneApplicationPage;
            root.NavigationService.Navigate(uri);
        }

        private void content_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border item = sender as Border;
            if (item == null)
                return;
            Article article = item.DataContext as Article;
            if (article == null)
                return;
            string blogerName = Uri.EscapeDataString(article.BlogerName);
            string articleUri = Uri.EscapeDataString(article.ArticleUri);
            //this.NavigationService.Navigate(new Uri(string.Format("/Home/ArticleView.xaml?BlogerName={0}&ArticleUri={1}", blogerName, articleUri), UriKind.Relative));
        }

        private void miBlog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("个人博客");
        }

        private void miComment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("查看评论");
        }

        private void NextPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (homeCurrentPage >= homeMaxPage || IsLoading)
                return;
            GetArticleList(string.Format(Constants.HomeArticleUri, ++homeCurrentPage));
        }

        private void GetArticleList(string uri)
        {
            if (IsLoading)
                return;
            IsLoading = true;
            request = HttpWebRequest.CreateHttp(uri);
            request.Method = "GET";
            request.UserAgent = Constants.UserAgent;
            request.CookieContainer = Constants.CookieContainer;
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            request = ar.AsyncState as HttpWebRequest;
            try
            {
                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string strHtml = sr.ReadToEnd();
                sr.Close();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(strHtml);
            }
            catch (WebException)
            {
                //点击取消的时候未将CurrentPage减1
                //Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    IsLoading = false;
                //}));
                //if (webExp.Status == WebExceptionStatus.RequestCanceled)
                //{
                //    //用户取消了请求
                //}
            }
        }
    }
}
