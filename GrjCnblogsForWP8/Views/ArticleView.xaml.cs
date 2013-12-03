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
    public partial class ArticleView : PhoneApplicationPage
    {
        public string BlogerName { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleUri { get; set; }
        public HttpWebRequest request { get; set; }
        private bool isInitialized = false;

        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { SystemTray.ProgressIndicator.IsVisible = value; }
        }

        public ArticleView()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (isInitialized)
                return;
            if (this.NavigationContext.QueryString.ContainsKey("BlogerName"))
            {
                this.BlogerName = Uri.UnescapeDataString(this.NavigationContext.QueryString["BlogerName"]);
            }
            if (this.NavigationContext.QueryString.ContainsKey("ArticleUri"))
            {
                string articleUri = Uri.UnescapeDataString(this.NavigationContext.QueryString["ArticleUri"]);
                this.ArticleUri = articleUri;
                GetArticle(this.ArticleUri);
            }
        }
        
        private void webBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //Type type = sender.GetType();
            //string value = e.Value;
            //this.webBrowser.SaveToString();
        }

        private void appBarRefersh_Click(object sender, EventArgs e)
        {
            GetArticle(this.ArticleUri);
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
            sms.Body = "嘿，这篇博客写的不错，分享给你：" + this.ArticleTitle + this.ArticleUri;
            sms.Show();
        }

        private void appBarEmailShare_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask email = new Microsoft.Phone.Tasks.EmailComposeTask();
            email.Subject = "分享博客：" + this.ArticleTitle;
            if (!string.IsNullOrEmpty(this.BlogerName))
                email.Subject += " —— " + this.BlogerName;
            email.Body = "嘿，这篇博客写的不错，分享给你：" + this.ArticleTitle + this.ArticleUri;
            email.Show();
        }

        private void appBarOpenInIE_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri(this.ArticleUri, UriKind.Absolute);
            task.Show();
        }

        private void GetArticle(string uri)
        {
            if (request != null && IsLoading)
                request.Abort();
            IsLoading = true;
            request = HttpWebRequest.CreateHttp(uri);
            request.Method = "GET";
            request.CookieContainer = Constants.CookieContainer;
            request.UserAgent = Constants.UserAgent;
            request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = ar.AsyncState as HttpWebRequest;
                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strHtml = sr.ReadToEnd();
                sr.Close();
                stream.Close();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(strHtml);
                var postBody = doc.GetElementbyId("cnblogs_post_body");
                if (postBody == null)
                    postBody = doc.GetElementbyId("news_body");
                var postTitle = doc.GetElementbyId("cb_post_title_url");
                if (postTitle == null)
                    postTitle = doc.GetElementbyId("ctl01_lnkTitle");
                if (postTitle == null)
                    postTitle = doc.GetElementbyId("news_title").ChildNodes["a"];
                this.ArticleTitle = postTitle.InnerHtml;
                //strHtml = "<html><head><meta http-equiv=\"Content-type\" content=\"text/html;charset=utf-8\" /><meta id=\"viewport\" name=\"viewport\" content=\"width=device-width,initial-scale=1.0,minimum-scale=0.5,maximum-scale=2.0,user-scalable=yes\"/> </head><body style=\"margin-bottom: 50px;\">" + postBody.InnerHtml + "</body></html>";
                strHtml = string.Format(Constants.strHtmlSkeleton, this.ArticleTitle, postBody.InnerHtml);
                MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(strHtml));
                sr = new StreamReader(memoryStream, System.Text.Encoding.Unicode);
                strHtml = sr.ReadToEnd();
                isInitialized = true;
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //this.txtArticleTitle.Text = this.ArticleTitle;
                    this.webBrowser.NavigateToString(strHtml);
                    IsLoading = false;
                }));
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    if ((ex as WebException).Status == WebExceptionStatus.RequestCanceled)
                    {
                        //请求被取消
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                }));
            }
        }
    }
}