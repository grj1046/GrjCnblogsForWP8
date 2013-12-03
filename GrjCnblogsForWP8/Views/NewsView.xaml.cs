using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using GrjCnblogsForWP8.Model;
using System.IO;

namespace GrjCnblogsForWP8.Views
{
    public partial class NewsView : PhoneApplicationPage
    {
        HttpWebRequest request;
        //最新新闻
        private bool NewsLatestInitialized = false;
        private int NewsLatestCurrentPage = 1;
        //最新新闻没有最大页数
        public ObservableCollection<News> NewsLatestList { get; set; }
        //推荐新闻
        private bool NewsRecommendInitialized = false;
        private int NewsRecommendCurrentPage = 1;
        //最新新闻没有最大页数
        public ObservableCollection<News> NewsRecommendList { get; set; }

        //热门新闻
        private bool NewsHotInitialized = false;
        //CurrentPage StartDate
        private int NewsHotCurrentPage = 1;
        private string NewsHotStartDate = DateTime.Now.ToString("yyyy/MM/dd");
        public ObservableCollection<News> NewsHotList { get; set; }

        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { SystemTray.ProgressIndicator.IsVisible = value; }
        }

        public NewsView()
        {
            InitializeComponent();
            NewsLatestList = new ObservableCollection<News>();
            NewsRecommendList = new ObservableCollection<News>();
            NewsHotList = new ObservableCollection<News>();
            this.llsNewsLatest.ItemsSource = NewsLatestList;
            this.llsNewsRecommend.ItemsSource = NewsRecommendList;
            this.llsNewsHot.ItemsSource = NewsHotList;
            this.llsNewsLatest.Visibility = System.Windows.Visibility.Collapsed;
            this.llsNewsRecommend.Visibility = System.Windows.Visibility.Collapsed;
            this.llsNewsHot.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void pivotPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (request != null && IsLoading)
            {
                IsLoading = false;
                request.Abort();
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    if (NewsLatestInitialized)
                        return;
                    GetNews(string.Format(Constants.NewsLatestArticleUri, NewsLatestCurrentPage));
                    break;
                case 1:
                    if (NewsRecommendInitialized)
                        return;
                    GetNews(string.Format(Constants.NewsRecommendArticleUri, NewsRecommendCurrentPage));
                    break;
                case 2:
                    if (NewsHotInitialized)
                        return;
                    GetNews(string.Format(Constants.NewsHotArticleUri, NewsHotCurrentPage, NewsHotStartDate));
                    break;
                default:
                    break;
            }
        }

        private void NextPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    GetNews(string.Format(Constants.NewsLatestArticleUri, ++NewsLatestCurrentPage));
                    break;
                case 1:
                    GetNews(string.Format(Constants.NewsRecommendArticleUri, ++NewsLatestCurrentPage));
                    break;
                case 2:
                    GetNews(string.Format(Constants.NewsHotArticleUri, ++NewsHotCurrentPage, NewsHotStartDate));
                    break;
                default:
                    break;
            }
        }

        private void appBarRefresh_Click(object sender, EventArgs e)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    NewsLatestList.Clear();
                    NewsLatestCurrentPage = 1;
                    GetNews(string.Format(Constants.NewsLatestArticleUri, NewsLatestCurrentPage));
                    break;
                case 1:
                    NewsRecommendList.Clear();
                    NewsRecommendCurrentPage = 1;
                    GetNews(string.Format(Constants.NewsRecommendArticleUri, NewsRecommendCurrentPage));
                    break;
                case 2:
                    NewsHotList.Clear();
                    NewsHotCurrentPage = 1;
                    GetNews(string.Format(Constants.NewsHotArticleUri, NewsHotCurrentPage, NewsHotStartDate));
                    break;
                default:
                    break;
            }
        }

        private void lpStartDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker lp = sender as ListPicker;
            string str = lp.SelectedItem as string;
            DateTime dt = DateTime.Now;
            switch (str)
            {
                case "今日":
                    dt = DateTime.Now;
                    break;
                case "昨日":
                    dt = DateTime.Now.AddDays(-1);
                    break;
                case "本周":
                    int daySpan = -1 * Convert.ToInt32(DateTime.Now.DayOfWeek) + 1;
                    dt = DateTime.Now.AddDays(daySpan);
                    break;
                case "本月":
                    dt = DateTime.Now.AddDays(-1 * dt.Date.Day + 1);
                    break;
                default:
                    break;
            }
            string tempDate = dt.ToString("yyyy/MM/dd");
            if (NewsHotStartDate == tempDate)
                return;
            NewsHotStartDate = tempDate;
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            NewsHotCurrentPage = 1;
            NewsHotList.Clear();
            GetNews(string.Format(Constants.NewsHotArticleUri, NewsHotStartDate, NewsHotCurrentPage));
        }

        private void Content_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            if (border == null)
                return;
            News news = border.DataContext as News;
            if (news == null)
                return;
            string newsUri = news.Uri;
            Uri uri = new Uri("/Views/NewsArticleView.xaml?NewsUri=" + newsUri, UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void GetNews(string uri)
        {
            if (request != null && IsLoading)
                request.Abort();
            IsLoading = true;
            request = HttpWebRequest.CreateHttp(uri);
            request.Method = "GET";
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
                var newsListNode = doc.GetElementbyId("news_list");
                if (newsListNode == null || !newsListNode.HasChildNodes)
                    return;
                var newsList = GetNewsList(newsListNode);
                if (newsList == null || newsList.Count() == 0)
                {
                    //TODO:当前页无内容
                }
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    switch (this.pivotPanel.SelectedIndex)
                    {
                        case 0:
                            GetNewsLatest(newsList);
                            break;
                        case 1:
                            GetNewsRecommend(newsList);
                            break;
                        case 2:
                            GetNewsHot(newsList);
                            break;
                        default:
                            break;
                    }
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
                    //如果webExp.Message 为NotFound，则需要将currentPage减1
                }
            }
        }

        private void GetNewsLatest(IEnumerable<News> newsList)
        {
            if (newsList == null)
                return;
            foreach (var news in newsList)
            {
                NewsLatestList.Add(news);
            }
            if (!NewsLatestInitialized)
            {
                this.llsNewsLatest.Visibility = System.Windows.Visibility.Visible;
                NewsLatestInitialized = true;
            }
            IsLoading = false;
        }

        private void GetNewsRecommend(IEnumerable<News> newsList)
        {
            if (newsList == null)
                return;
            foreach (var news in newsList)
            {
                NewsRecommendList.Add(news);
            }
            if (!NewsRecommendInitialized)
            {
                this.llsNewsRecommend.Visibility = System.Windows.Visibility.Visible;
                NewsRecommendInitialized = true;
            }
            IsLoading = false;
        }

        private void GetNewsHot(IEnumerable<News> newsList)
        {
            if (newsList == null)
                return;
            foreach (var news in newsList)
            {
                NewsHotList.Add(news);
            }
            if (!NewsHotInitialized)
            {
                this.llsNewsHot.Visibility = System.Windows.Visibility.Visible;
                NewsHotInitialized = true;
            }
            IsLoading = false;
        }

        private IEnumerable<News> GetNewsList(HtmlAgilityPack.HtmlNode newsListNode)
        {
            foreach (var newsBlock in newsListNode.ChildNodes)
            {
                if (newsBlock.Name == "div" && newsBlock.Attributes["class"] != null && newsBlock.Attributes["class"].Value.Contains("news_block"))
                {
                    News news = new News();
                    string newsId = newsBlock.Attributes["id"].Value.Remove(0, 6);
                    news.NewsId = newsId;
                    foreach (var item in newsBlock.ChildNodes)
                    {
                        if (item.Name == "div" && item.Attributes["class"] != null)
                        {
                            if (item.Attributes["class"].Value == "action")
                            {
                                string digg = item.ChildNodes["div"].ChildNodes["span"].InnerHtml;
                                news.Digg = digg;
                            }
                            else if (item.Attributes["class"].Value == "content")
                            {
                                foreach (var itemContent in item.ChildNodes)
                                {
                                    if (itemContent.Name == "h2")
                                    {
                                        string newsUri = itemContent.ChildNodes["a"].Attributes["href"].Value;
                                        string newsTitle = itemContent.ChildNodes["a"].InnerText;
                                        news.Uri = Uri.EscapeDataString("http://news.cnblogs.com" + newsUri);
                                        news.Title = newsTitle.Replace("\r", "").Replace("\n", "").Trim();
                                    }
                                    else if (itemContent.Name == "div")
                                    {
                                        if (itemContent.Attributes["class"].Value == "entry_summary")
                                        {
                                            if (itemContent.ChildNodes["a"] != null)
                                            {
                                                var topicTitleNode = itemContent.ChildNodes["a"].Attributes["title"];
                                                if (topicTitleNode != null)
                                                {
                                                    string topicTitle = itemContent.ChildNodes["a"].Attributes["title"].Value;
                                                    news.TopicTitle = topicTitle;
                                                }
                                                string topicUri = itemContent.ChildNodes["a"].Attributes["href"].Value;
                                                string topicImg = itemContent.ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;
                                                string summary = itemContent.ChildNodes["a"].NextSibling.InnerHtml;

                                                news.TopicUri = "http://news.cnblogs.com" + topicUri;
                                                news.TopicImg = "http://news.cnblogs.com" + topicImg;
                                                news.Summary = summary.Replace("\r", "").Replace("\n", "").Trim();
                                            }
                                            else
                                            {
                                                news.Summary = itemContent.InnerText.Replace("\r", "").Replace("\n", "").Trim();
                                            }
                                        }
                                        else if (itemContent.Attributes["class"].Value == "entry_footer")
                                        {
                                            foreach (var itemFooter in itemContent.ChildNodes)
                                            {
                                                if (itemFooter.Name == "a")
                                                {
                                                    string deliver = itemFooter.InnerText;
                                                    news.Deliver = deliver.Replace("\r", "").Replace("\n", "").Trim();
                                                }
                                                else if (itemFooter.Name == "span")
                                                {
                                                    if (itemFooter.Attributes["class"].Value == "comment")
                                                    {
                                                        string commentNum = itemFooter.ChildNodes["a"].InnerText;
                                                        news.CommentNum = commentNum;
                                                    }
                                                    else if (itemFooter.Attributes["class"].Value == "view")
                                                    {
                                                        string views = itemFooter.InnerText;
                                                        news.Views = views;
                                                        if (itemFooter.NextSibling.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                                                            news.PublishTime = itemFooter.NextSibling.InnerText.Replace("\r", "").Replace("\n", "").Trim();
                                                    }
                                                    else if (itemFooter.Attributes["class"].Value == "tag")
                                                    {
                                                        string tagUri = itemFooter.ChildNodes["a"].Attributes["href"].Value;
                                                        string tagTitle = itemFooter.ChildNodes["a"].InnerText;
                                                        news.newsTagTitle = tagTitle;
                                                        news.newsTagUri = "http://news.cnblogs.com" + tagUri;
                                                        if (itemFooter.NextSibling.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                                                            news.PublishTime = itemFooter.NextSibling.InnerText.Replace("\r", "").Replace("\n", "").Trim();
                                                    }
                                                    else if (itemFooter.Attributes["class"].Value == "gray")
                                                    {
                                                        news.PublishTime = itemFooter.InnerText;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    yield return news;
                }
            }
        }
    }
}