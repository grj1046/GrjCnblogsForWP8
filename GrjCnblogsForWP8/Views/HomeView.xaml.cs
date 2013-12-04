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
    public partial class HomeView : PhoneApplicationPage
    {
        HttpWebRequest request;
        //Home
        private bool homePageInitialized = false;
        private int homeMaxPage = 200;
        private int homeCurrentPage = 1;
        public ObservableCollection<Article> HomeArticleList { get; set; }

        //Pick
        private bool pickPageInitialized = false;
        private int pickMaxPage = 68;
        private int pickCurrentPage = 1;
        public ObservableCollection<Article> PickArticleList { get; set; }

        //candidate
        private bool candidatePageInitialized = false;
        private int candidateMaxPage = 200;
        private int candidateCurrentPage = 1;
        public ObservableCollection<Article> CandidateArticleList { get; set; }

        //news
        private bool newsPageInitialized = false;
        //private int newsNaxPage = int.MaxValue;//新闻频道未设置MaxPage
        private int newsCurrentPage = 1;
        public ObservableCollection<Article> NewsArticleList { get; set; }

        //myFollowing
        private bool myFollowingPageInitialized = false;
        private int myFollowingMaxPage = 17;
        private int myFollowingCurrentPage = 1;
        public ObservableCollection<Article> MyFollowingArticleList { get; set; }

        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { if (SystemTray.ProgressIndicator != null) SystemTray.ProgressIndicator.IsVisible = value; }
        }

        public HomeView()
        {
            InitializeComponent();
            HomeArticleList = new ObservableCollection<Article>();
            PickArticleList = new ObservableCollection<Article>();
            CandidateArticleList = new ObservableCollection<Article>();
            NewsArticleList = new ObservableCollection<Article>();
            MyFollowingArticleList = new ObservableCollection<Article>();
            this.llsHomePage.ItemsSource = HomeArticleList;
            this.llsPickPage.ItemsSource = PickArticleList;
            this.llsCandidatePage.ItemsSource = CandidateArticleList;
            this.llsNewsPage.ItemsSource = NewsArticleList;
            this.llsMyFollowingPage.ItemsSource = MyFollowingArticleList;
            this.llsHomePage.Visibility = System.Windows.Visibility.Collapsed;
            this.llsPickPage.Visibility = System.Windows.Visibility.Collapsed;
            this.llsCandidatePage.Visibility = System.Windows.Visibility.Collapsed;
            this.llsNewsPage.Visibility = System.Windows.Visibility.Collapsed;
            this.llsMyFollowingPage.Visibility = System.Windows.Visibility.Collapsed;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.pivotPanel.SelectedIndex == 4)
            {
                if (myFollowingPageInitialized)
                    return;
                if (Constants.CookieContainer == null || Constants.CookieContainer.Count == 0)
                {
                    MessageBoxResult dialogResult = MessageBox.Show("登录后才能访问，是否登录？", "提示", MessageBoxButton.OKCancel);
                    if (dialogResult == MessageBoxResult.OK)
                    {
                        this.NavigationService.Navigate(new Uri("/Views/Login.xaml", UriKind.Relative));
                    }
                    return;
                }
                GetArticleList(string.Format(Constants.MyFollowingArticleUri, myFollowingCurrentPage));
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            IsLoading = false;
            if (request != null)
                request.Abort();
        }

        private void pivotPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoading && request != null)
                request.Abort();
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    if (homePageInitialized)
                        return;
                    GetArticleList(String.Format(Constants.HomeArticleUri, homeCurrentPage));
                    break;
                case 1:
                    if (pickPageInitialized)
                        return;
                    GetArticleList(String.Format(Constants.PickArticleUri, pickCurrentPage));
                    break;
                case 2:
                    if (candidatePageInitialized)
                        return;
                    GetArticleList(string.Format(Constants.CandidateArticleUri, candidateCurrentPage));
                    break;
                case 3:
                    if (newsPageInitialized)
                        return;
                    GetArticleList(string.Format(Constants.NewsArticleUri, newsCurrentPage));
                    break;
                case 4:
                    if (myFollowingPageInitialized)
                        return;

                    if (Constants.CookieContainer == null || Constants.CookieContainer.Count == 0)
                    {
                        MessageBoxResult dialogResult = MessageBox.Show("登录后才能访问，是否登录？", "提示", MessageBoxButton.OKCancel);
                        if (dialogResult == MessageBoxResult.OK)
                        {
                            this.NavigationService.Navigate(new Uri("/Views/Login.xaml", UriKind.Relative));
                        }
                        return;
                    }
                    GetArticleList(string.Format(Constants.MyFollowingArticleUri, myFollowingCurrentPage));
                    break;
                default:
                    break;
            }
        }

        private void llsHeader_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            Article article = border.DataContext as Article;
            if (article == null)
                return;
            if (string.IsNullOrEmpty(article.ArticleUri))
                return;
            string articleUri = Uri.EscapeDataString(article.ArticleUri);
            Uri uri = new Uri(string.Format("/Views/ArticleView.xaml?ArticleUri={0}", articleUri), UriKind.Relative);
            this.NavigationService.Navigate(uri);
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
            this.NavigationService.Navigate(new Uri(string.Format("/Views/ArticleView.xaml?BlogerName={0}&ArticleUri={1}", blogerName, articleUri), UriKind.Relative));
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
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    if (homeCurrentPage >= homeMaxPage || IsLoading)
                        return;
                    GetArticleList(string.Format(Constants.HomeArticleUri, ++homeCurrentPage));
                    break;
                case 1:
                    if (pickCurrentPage >= pickMaxPage || IsLoading)
                        return;
                    GetArticleList(string.Format(Constants.PickArticleUri, ++pickCurrentPage));
                    break;
                case 2:
                    if (candidateCurrentPage >= candidateMaxPage || IsLoading)
                        return;
                    GetArticleList(string.Format(Constants.CandidateArticleUri, ++candidateCurrentPage));
                    break;
                case 3:
                    if (IsLoading)
                        return;
                    GetArticleList(string.Format(Constants.NewsArticleUri, ++newsCurrentPage));
                    break;
                case 4:
                    if (myFollowingCurrentPage >= myFollowingMaxPage || IsLoading)
                        return;
                    GetArticleList(string.Format(Constants.MyFollowingArticleUri, ++myFollowingCurrentPage));
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
                    //this.llsHomePage.ListHeader = null;
                    homeCurrentPage = 1;
                    GetArticleList(string.Format(Constants.HomeArticleUri, homeCurrentPage));
                    break;
                case 1:
                    //this.llsPickPage.ListHeader = null;
                    pickCurrentPage = 1;
                    GetArticleList(string.Format(Constants.PickArticleUri, pickCurrentPage));
                    break;
                case 2:
                    //this.llsCandidatePage.ListHeader = null;
                    candidateCurrentPage = 1;
                    GetArticleList(string.Format(Constants.CandidateArticleUri, candidateCurrentPage));
                    break;
                case 3:
                    //this.llsNewsPage.ListHeader = null;
                    newsCurrentPage = 1;
                    GetArticleList(string.Format(Constants.NewsArticleUri, newsCurrentPage));
                    break;
                case 4:
                    //this.llsMyFollowingPage.ListHeader = null;
                    myFollowingCurrentPage = 1;
                    GetArticleList(string.Format(Constants.MyFollowingArticleUri, myFollowingCurrentPage));
                    break;
                default:
                    break;
            }
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

                //获取编辑推荐至推荐新闻
                var headerContent = GetListHeader(doc.GetElementbyId("headline_block"));

                var postList = doc.GetElementbyId("post_list");
                if (postList == null || !postList.HasChildNodes)
                    return;
                var articleList = this.GetArticleList(postList);

                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    switch (this.pivotPanel.SelectedIndex)
                    {
                        case 0:
                            GetHomePageArticle(articleList, headerContent);
                            break;
                        case 1:
                            GetPickPageArticle(articleList, headerContent);
                            break;
                        case 2:
                            GetCandidatePageArticle(articleList, headerContent);
                            break;
                        case 3:
                            GetNewsPageArticle(articleList, headerContent);
                            break;
                        case 4:
                            GetMyFollowingArticle(articleList, headerContent);
                            break;
                        default:
                            break;
                    }
                }));
            }
            catch (WebException webExp)
            {
                //点击取消的时候未将CurrentPage减1
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                }));
                if (webExp.Status == WebExceptionStatus.RequestCanceled)
                {
                    //用户取消了请求
                }
            }
        }
        //主页区文章
        private void GetHomePageArticle(IEnumerable<Article> articleList, HeaderContent headerContent)
        {
            if (!homePageInitialized)
            {
                HomeArticleList.Clear();
                homePageInitialized = true;
                this.llsHomePage.Visibility = System.Windows.Visibility.Visible;
            }
            this.llsHomePage.ListHeader = headerContent;
            foreach (Article article in articleList)
            {
                HomeArticleList.Add(article);
            }
            IsLoading = false;
        }
        //精华区文章
        private void GetPickPageArticle(IEnumerable<Article> articleList, HeaderContent headerContent)
        {
            if (!pickPageInitialized)
            {
                PickArticleList.Clear();
                pickPageInitialized = true;
                this.llsPickPage.Visibility = System.Windows.Visibility.Visible;
            }
            //获取编辑推荐至推荐新闻
            this.llsPickPage.ListHeader = headerContent;

            foreach (Article article in articleList)
            {
                PickArticleList.Add(article);
            }
            IsLoading = false;
        }
        //候选区文章
        private void GetCandidatePageArticle(IEnumerable<Article> articleList, HeaderContent headerContent)
        {
            if (!candidatePageInitialized)
            {
                CandidateArticleList.Clear();
                candidatePageInitialized = true;
                this.llsCandidatePage.Visibility = System.Windows.Visibility.Visible;
            }
            //获取编辑推荐至推荐新闻
            this.llsCandidatePage.ListHeader = headerContent;

            foreach (Article article in articleList)
            {
                CandidateArticleList.Add(article);
            }
            IsLoading = false;
        }
        //新闻区文章
        private void GetNewsPageArticle(IEnumerable<Article> articleList, HeaderContent headerContent)
        {
            if (!newsPageInitialized)
            {
                NewsArticleList.Clear();
                newsPageInitialized = true;
                this.llsNewsPage.Visibility = System.Windows.Visibility.Visible;
            }
            //获取编辑推荐至推荐新闻
            this.llsNewsPage.ListHeader = headerContent;

            foreach (Article article in articleList)
            {
                NewsArticleList.Add(article);
            }
            IsLoading = false;
        }
        //我关注的
        private void GetMyFollowingArticle(IEnumerable<Article> articleList, HeaderContent headerContent)
        {
            if (!myFollowingPageInitialized)
            {
                MyFollowingArticleList.Clear();
                myFollowingPageInitialized = true;
                this.llsMyFollowingPage.Visibility = System.Windows.Visibility.Visible;
            }
            //获取编辑推荐至推荐新闻
            this.llsMyFollowingPage.ListHeader = headerContent;

            foreach (Article article in articleList)
            {
                MyFollowingArticleList.Add(article);
            }
            IsLoading = false;
        }

        /// <summary>
        /// 获取编辑推荐等等
        /// </summary>
        /// <param name="headlineBlock"></param>
        /// <returns></returns>
        private HeaderContent GetListHeader(HtmlAgilityPack.HtmlNode headlineBlock)
        {
            if (headlineBlock == null)
                return new HeaderContent();
            //headlineBlock.ChildNodes["ul"].ChildNodes[0].ChildNodes["a"].ChildNodes["span"].InnerText 获取不到评论数

            Article EditorRecommend = new Article();
            Article MaximumRecommend = new Article();
            Article MaximumComment = new Article();
            Article TopNews = new Article();
            Article RecommendNews = new Article();

            foreach (var item in headlineBlock.ChildNodes["ul"].ChildNodes)
            {
                string title = item.ChildNodes["a"].InnerText;
                string articleUri = item.ChildNodes["a"].Attributes["href"].Value;
                if (title.Contains("编辑推荐"))
                {
                    EditorRecommend.Title = title;
                    EditorRecommend.ArticleUri = articleUri;
                }
                else if (title.Contains("最多推荐"))
                {
                    MaximumRecommend.Title = title;
                    MaximumRecommend.ArticleUri = articleUri;
                }
                else if (title.Contains("最多评论"))
                {
                    MaximumComment.Title = title;
                    MaximumComment.ArticleUri = articleUri;
                }
                else if (title.Contains("新闻头条"))
                {
                    TopNews.Title = title;
                    TopNews.ArticleUri = articleUri;
                }
                else if (title.Contains("推荐新闻"))
                {
                    RecommendNews.Title = title;
                    RecommendNews.ArticleUri = articleUri;
                }
            }

            return new HeaderContent()
            {
                EditorRecommend = EditorRecommend,
                MaximumRecommend = MaximumRecommend,
                MaximumComment = MaximumComment,
                TopNews = TopNews,
                RecommendNews = RecommendNews
            };
        }
        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="postList"></param>
        /// <returns></returns>
        private IEnumerable<Article> GetArticleList(HtmlAgilityPack.HtmlNode postList)
        {
            foreach (var item in postList.ChildNodes)
            {
                if (item.Name == "div" && item.Attributes["class"] != null && item.Attributes["class"].Value == "post_item")
                {
                    Article article = new Article();
                    foreach (var itemNode in item.ChildNodes)
                    {
                        if (itemNode.Name == "div" && itemNode.Attributes["class"] != null)
                        {
                            //&& itemNode.Attributes["class"].Value == "digg"
                            var classValue = itemNode.Attributes["class"].Value;
                            if (classValue == "digg")
                            {
                                var digg = itemNode.ChildNodes["div"].ChildNodes["span"].InnerText;
                                article.Digg = digg;
                            }
                            else if (classValue == "post_item_body")
                            {
                                //h3
                                var title = itemNode.ChildNodes["h3"].ChildNodes["a"].InnerText;
                                var articleLink = itemNode.ChildNodes["h3"].ChildNodes["a"].Attributes["href"].Value;
                                article.Title = title;
                                article.ArticleUri = articleLink;
                                //article.FaceImage = "";
                                if (itemNode.ChildNodes["p"].ChildNodes["a"] != null)
                                {
                                    var faceImg = itemNode.ChildNodes["p"].ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;
                                    string strSummary = string.Empty;
                                    var summaryNode = itemNode.ChildNodes["p"].ChildNodes["a"].NextSibling;
                                    if (summaryNode != null)
                                        strSummary = itemNode.ChildNodes["p"].ChildNodes["a"].NextSibling.InnerText;
                                    article.FaceImage = faceImg;
                                    article.Summary = strSummary.Replace("\r\n", "").TrimEnd();
                                }
                                else
                                {
                                    var summary = itemNode.ChildNodes["p"].InnerText;
                                    article.Summary = summary.Replace("\r\n", "").TrimEnd();
                                }

                                var blogerName = itemNode.ChildNodes["div"].ChildNodes["a"].InnerText;
                                var blogAddress = itemNode.ChildNodes["div"].ChildNodes["a"].Attributes["href"].Value;
                                var publishTime = itemNode.ChildNodes["div"].ChildNodes["a"].NextSibling.InnerText;
                                var articleComment = itemNode.ChildNodes["div"].LastChild.PreviousSibling.ChildNodes["a"].InnerText;
                                var articleView = itemNode.ChildNodes["div"].LastChild.ChildNodes["a"].InnerText;
                                article.BlogerName = blogerName.Replace("\r\n", "").Trim();
                                article.BlogAddress = blogAddress.Replace("\r\n", "").Trim();
                                article.PublishTime = publishTime.Replace("\r\n", "").Trim();
                                article.ArticleComment = articleComment.Replace("\r\n", "").Trim();
                                article.ArticleView = articleView.Replace("\r\n", "").Trim();
                            }
                        }
                    }
                    yield return article;
                }
            }
        }
    }
}