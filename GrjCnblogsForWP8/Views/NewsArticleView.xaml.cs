using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;

namespace GrjCnblogsForWP8.Views
{
    public partial class NewsArticleView : PhoneApplicationPage
    {
        HttpWebRequest request;
        public string NewsUri { get; set; }
        public string NewsTitle { get; set; }
        public string NewsBody { get; set; }

        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { SystemTray.ProgressIndicator.IsVisible = value; }
        }

        public NewsArticleView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("NewsUri"))
            {
                string newsUri = Uri.UnescapeDataString(this.NavigationContext.QueryString["NewsUri"]);
                this.NewsUri = newsUri;
                GetNewsArticle(newsUri);
            }
        }

        private void GetNewsArticle(string uri)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            request = HttpWebRequest.CreateHttp(uri);
            request.CookieContainer = Constants.CookieContainer;
            request.UserAgent = Constants.UserAgent;
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            try
            {
                request = ar.AsyncState as HttpWebRequest;
                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strHtml = sr.ReadToEnd();
                sr.Close();
                stream.Close();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(strHtml);
                GetNewsTitle(doc.GetElementbyId("news_title"));
                GetNewsBody(doc.GetElementbyId("news_body"));
                strHtml = string.Format(Constants.strHtmlSkeleton, NewsTitle, NewsBody);
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                    this.webBrowser.NavigateToString(strHtml);
                }));
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException webExp = ex as WebException;
                    if (webExp.Status == WebExceptionStatus.RequestCanceled)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IsLoading = false;
                        }));
                    }
                }
            }
        }

        public void GetNewsTitle(HtmlAgilityPack.HtmlNode newsTitleNode)
        {
            if (newsTitleNode != null && newsTitleNode.HasChildNodes)
            {
                var aNode = newsTitleNode.ChildNodes["a"];
                if (aNode != null)
                    this.NewsTitle = aNode.InnerText;
            }
        }

        public void GetNewsBody(HtmlAgilityPack.HtmlNode newsBodyNode)
        {
            if (newsBodyNode != null && newsBodyNode.HasChildNodes)
                this.NewsBody = newsBodyNode.InnerHtml;
        }

        private void appBarRefersh_Click(object sender, EventArgs e)
        {
            GetNewsArticle(this.NewsUri);
        }

        private void appBarCopy_Click(object sender, EventArgs e)
        {
            //TODO:复制至剪贴板功能暂未实现
            MessageBox.Show("复制至剪贴板功能暂未实现");
        }

        private void appBarIng_Click(object sender, EventArgs e)
        {
            //TODO:闪存模块暂未实现
            MessageBox.Show("闪存模块暂未实现");
        }

        private void appBarSmsShare_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.SmsComposeTask sms = new Microsoft.Phone.Tasks.SmsComposeTask();
            sms.Body = "嘿，这篇博客写的不错，分享给你：" + this.NewsTitle + this.NewsUri;
            sms.Show();
        }

        private void appBarEmailShare_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask email = new Microsoft.Phone.Tasks.EmailComposeTask();
            email.Subject = "分享博客：" + this.NewsTitle + "——来自博客园新闻频道";
            email.Body = "嘿，这篇博客写的不错，分享给你：" + this.NewsTitle + this.NewsUri;
            email.Show();
        }

        private void appBarOpenInIE_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri(this.NewsUri, UriKind.Absolute);
            task.Show();
        }
    }
}