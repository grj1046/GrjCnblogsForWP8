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
    public partial class IngView : PhoneApplicationPage
    {
        HttpWebRequest request;
        private int pageSize = 30;
        //全部
        private bool IngAllInitialized = false;
        private int IngAllCurrentPage = 1;
        //MaxPage = 167
        public ObservableCollection<Ing> IngAllList { get; set; }

        //回复我
        private bool IngReplyToMeInitialized = false;
        private int IngReplyToMeCurrentPage = 1;
        //private int IngReplyToMeMaxPage = 1;//可以拿到maxPage
        public ObservableCollection<IngReplyToMe> IngReplyToMeList { get; set; }

        //提到我
        private bool IngMentionedMeInitialized = false;
        private int IngMentionedMeCurrentPage = 1;
        //private int IngMentionedMeMaxPage = 1;
        public ObservableCollection<Ing> IngMentionedMeList { get; set; }

        //新回应
        private bool IngNewReplyInitialized = false;
        private int IngNewReplyCurrentPage = 1;
        //新回应没有MaxPage
        public ObservableCollection<Ing> IngNewReplyList { get; set; }

        //我回应
        private bool IngMyReplyInitialized = false;
        private int IngMyReplyCurrentPage = 1;
        //private int Ing<yReplyMaxPage = 1;//有 最大值 但是无法获取
        public ObservableCollection<Ing> IngMyReplyList { get; set; }

        //我的
        private bool IngMyInitialized = false;
        private int IngMyCurrentPage = 1;
        //private int IngMyMaxPage = 1;//有MaxPage，可以获取
        private ObservableCollection<Ing> IngMyList { get; set; }

        //关注
        private bool IngRecentInitialized = false;
        private int IngRecentCurrentPage = 1;
        //private int IngRecentMaxPage = 1;//最大页数，可以获取到。
        private ObservableCollection<Ing> IngRecentList { get; set; }

        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { SystemTray.ProgressIndicator.IsVisible = value; }
        }

        public IngView()
        {
            InitializeComponent();
            IngAllList = new ObservableCollection<Ing>();
            IngReplyToMeList = new ObservableCollection<IngReplyToMe>();
            IngMentionedMeList = new ObservableCollection<Ing>();
            IngNewReplyList = new ObservableCollection<Ing>();
            IngMyReplyList = new ObservableCollection<Ing>();
            IngMyList = new ObservableCollection<Ing>();
            IngRecentList = new ObservableCollection<Ing>();
            this.llsIngAll.ItemsSource = IngAllList;
            this.llsIngReplyToMe.ItemsSource = IngReplyToMeList;
            this.llsIngMentionedMe.ItemsSource = IngMentionedMeList;
            this.llsIngNewReply.ItemsSource = IngNewReplyList;
            this.llsIngMyReply.ItemsSource = IngMyReplyList;
            this.llsIngMy.ItemsSource = IngMyList;
            this.llsIngRecent.ItemsSource = IngRecentList;
            this.llsIngAll.Visibility = System.Windows.Visibility.Collapsed;
            this.llsIngReplyToMe.Visibility = System.Windows.Visibility.Collapsed;
            this.llsIngMentionedMe.Visibility = System.Windows.Visibility.Collapsed;
            this.llsIngNewReply.Visibility = System.Windows.Visibility.Collapsed;
            this.llsIngMyReply.Visibility = System.Windows.Visibility.Collapsed;
            this.llsIngMy.Visibility = System.Windows.Visibility.Collapsed;
            this.llsIngRecent.Visibility = System.Windows.Visibility.Collapsed;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back && e.Uri == new Uri("/Views/IngView.xaml", UriKind.Relative))
            {
                IngAllCurrentPage = 1;
                GetIng(string.Format(Constants.IngAllUri, IngAllCurrentPage, pageSize, DateTime.Now.Second));
                return;
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 1:
                    if (IngReplyToMeInitialized)
                        return;
                    GetIng(string.Format(Constants.IngReplyToMe, IngReplyToMeCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 2:
                    if (IngMentionedMeInitialized)
                        return;
                    GetIng(string.Format(string.Format(Constants.IngMentionedMe, IngMentionedMeCurrentPage, pageSize, DateTime.Now.Second)));
                    break;
                case 3:
                    if (IngNewReplyInitialized)
                        return;
                    GetIng(string.Format(Constants.IngNewReply, IngNewReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 4:
                    if (IngMyReplyInitialized)
                        return;
                    GetIng(string.Format(Constants.IngMyReply, IngMyReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 5:
                    if (IngMyInitialized)
                        return;
                    GetIng(string.Format(Constants.IngMy, IngMyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 6:
                    if (IngRecentInitialized)
                        return;
                    GetIng(string.Format(Constants.IngRecent, IngRecentCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                default:
                    break;
            }
        }

        private void appMenuLock_Click(object sender, EventArgs e)
        {
            ApplicationBarMenuItem appbarMenu = sender as ApplicationBarMenuItem;
            if (appbarMenu == null)
                return;
            if (this.pivotPanel.IsLocked)
            {
                this.pivotPanel.IsLocked = false;
                appbarMenu.Text = "锁定当前页";
            }
            else
            {
                this.pivotPanel.IsLocked = true;
                appbarMenu.Text = "解锁当前页";
            }
        }
        private void appBarPublishIng_Click(object sender, EventArgs e)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            this.NavigationService.Navigate(new Uri("/Views/PublishIng.xaml", UriKind.Relative));
        }

        private void pivotPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    if (IngAllInitialized)
                        return;
                    GetIng(string.Format(Constants.IngAllUri, IngAllCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 1:
                    if (IngReplyToMeInitialized)
                        return;
                    GetIng(string.Format(Constants.IngReplyToMe, IngReplyToMeCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 2:
                    if (IngMentionedMeInitialized)
                        return;
                    GetIng(string.Format(Constants.IngMentionedMe, IngMentionedMeCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 3:
                    if (IngNewReplyInitialized)
                        return;
                    GetIng(string.Format(Constants.IngNewReply, IngNewReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 4:
                    if (IngMyReplyInitialized)
                        return;
                    GetIng(string.Format(Constants.IngMyReply, IngMyReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 5:
                    if (IngMyInitialized)
                        return;
                    GetIng(string.Format(Constants.IngMy, IngMyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 6:
                    if (IngRecentInitialized)
                        return;
                    GetIng(string.Format(Constants.IngRecent, IngRecentCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                default:
                    break;
            }
        }

        private void appBarRefresh_Click(object sender, EventArgs e)
        {
            //IngAllList.First().IngReply.Add(new IngReply(IngAllList.First()) { ReplyBody = "ReplyBody" });
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    IngAllCurrentPage = 1;
                    GetIng(string.Format(Constants.IngAllUri, IngAllCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 1:
                    IngReplyToMeCurrentPage = 1;
                    GetIng(string.Format(Constants.IngReplyToMe, IngAllCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 2:
                    IngMentionedMeCurrentPage = 1;
                    GetIng(string.Format(Constants.IngMentionedMe, IngMentionedMeCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 3:
                    IngNewReplyCurrentPage = 1;
                    GetIng(string.Format(Constants.IngNewReply, IngNewReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 4:
                    IngMyReplyCurrentPage = 1;
                    GetIng(string.Format(Constants.IngMyReply, IngMyReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 5:
                    IngMyCurrentPage = 1;
                    GetIng(string.Format(Constants.IngMy, IngMyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 6:
                    IngRecentCurrentPage = 1;
                    GetIng(string.Format(Constants.IngRecent, IngRecentCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                default:
                    break;
            }
        }

        private void nextPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            switch (this.pivotPanel.SelectedIndex)
            {
                case 0:
                    GetIng(string.Format(Constants.IngAllUri, ++IngAllCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 1:
                    GetIng(string.Format(Constants.IngReplyToMe, ++IngReplyToMeCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 2:
                    GetIng(string.Format(Constants.IngMentionedMe, ++IngAllCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 3:
                    GetIng(string.Format(Constants.IngNewReply, ++IngNewReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 4:
                    GetIng(string.Format(Constants.IngMyReply, ++IngMyReplyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 5:
                    GetIng(string.Format(Constants.IngMy, ++IngMyCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                case 6:
                    GetIng(string.Format(Constants.IngRecent, ++IngRecentCurrentPage, pageSize, DateTime.Now.Second));
                    break;
                default:
                    break;
            }
        }

        private void content_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Constants.CookieContainer == null || Constants.CookieContainer.Count == 0)
            {
                var dialogResult = MessageBox.Show("您尚未登录，请先登录。", "提示", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                    this.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                return;
            }
            Border border = sender as Border;
            if (border == null)
                return;
            if (border.DataContext is Ing)
            {
                Ing ing = border.DataContext as Ing;
                //MessageBox.Show(ing.IngDetailUri);
                string uri = "/Views/IngDetailView.xaml?IngDetailUri=" + ing.IngDetailUri;
                this.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else if (border.DataContext is IngReplyToMe)
            {
                IngReplyToMe ingReplyToMe = border.DataContext as IngReplyToMe;
                MessageBox.Show(ingReplyToMe.IngDetailUri);
            }
        }

        private void item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            if (border == null)
                return;
            IngReply ingReply = border.DataContext as IngReply;
            if (ingReply == null)
                return;
            this.NavigationService.Navigate(new Uri("/Views/ReplyIngView.xaml", UriKind.Relative));
            //MessageBox.Show(ingReply.Id);
            e.Handled = true;
        }

        private void GetIng(string uri)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            IsLoading = true;
            request = HttpWebRequest.CreateHttp(uri);
            request.Method = "GET";
            request.UserAgent = Constants.UserAgent;
            request.CookieContainer = Constants.CookieContainer;
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
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //当前处于未登录状态，请先<a href='javascript:void(0)' onclick='login();'>登录</a>。
                    if (strHtml.Contains("未登录状态"))
                    {
                        var dialogResult = MessageBox.Show("当前处于未登录状态，请先登录", "提示", MessageBoxButton.OKCancel);
                        if (dialogResult == MessageBoxResult.OK)
                        {
                            this.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                        }
                        IsLoading = false;
                        return;
                    }
                    switch (this.pivotPanel.SelectedIndex)
                    {
                        case 0:
                            GetIngAll(GetIngList(strHtml));
                            break;
                        case 1:
                            GetIngReplyToMe(GetIngReplyToMeList(strHtml));
                            break;
                        case 2:
                            GetIngMentionedMe(GetIngList(strHtml));
                            break;
                        case 3:
                            GetIngNewReply(GetIngList(strHtml));
                            break;
                        case 4:
                            GetIngMyReply(GetIngList(strHtml));
                            break;
                        case 5:
                            GetIngMy(GetIngList(strHtml));
                            break;
                        case 6:
                            GetIngRecent(GetIngList(strHtml));
                            break;
                        default:
                            break;
                    }
                }));
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() => IsLoading = false));
                if (ex is WebException)
                {
                    WebException webExp = ex as WebException;
                    if (webExp.Status == WebExceptionStatus.RequestCanceled) { }
                    //TODO:网络异常
                    if (webExp.Status == WebExceptionStatus.UnknownError) { /*网络异常*/}
                }
            }
        }

        private void GetIngAll(IEnumerable<Ing> ingList)
        {
            if (!IngAllInitialized)
            {
                IngAllList.Clear();
                this.llsIngAll.Visibility = System.Windows.Visibility.Visible;
                IngAllInitialized = true;
            }
            foreach (var item in ingList)
            {
                IngAllList.Add(item);
            }
            IsLoading = false;
        }

        private void GetIngReplyToMe(IEnumerable<IngReplyToMe> ingList)
        {
            if (!IngReplyToMeInitialized)
            {
                IngReplyToMeList.Clear();
                this.llsIngReplyToMe.Visibility = System.Windows.Visibility.Visible;
                IngReplyToMeInitialized = true;
            }
            foreach (var ingReplyToMe in ingList)
            {
                IngReplyToMeList.Add(ingReplyToMe);
            }
            IsLoading = false;
        }

        private void GetIngMentionedMe(IEnumerable<Ing> ingList)
        {
            if (!IngMentionedMeInitialized)
            {
                IngMentionedMeList.Clear();
                this.llsIngMentionedMe.Visibility = System.Windows.Visibility.Visible;
                IngMentionedMeInitialized = true;
            }
            foreach (var item in ingList)
            {
                IngMentionedMeList.Add(item);
            }
            IsLoading = false;
        }

        private void GetIngNewReply(IEnumerable<Ing> ingList)
        {
            if (!IngNewReplyInitialized)
            {
                IngNewReplyList.Clear();
                this.llsIngNewReply.Visibility = System.Windows.Visibility.Visible;
                IngNewReplyInitialized = true;
            }
            foreach (var item in ingList)
            {
                IngNewReplyList.Add(item);
            }
            IsLoading = false;
        }

        private void GetIngMyReply(IEnumerable<Ing> ingList)
        {
            if (!IngMyReplyInitialized)
            {
                IngMyReplyList.Clear();
                this.llsIngMyReply.Visibility = System.Windows.Visibility.Visible;
                IngMyReplyInitialized = true;
            }
            foreach (var item in ingList)
            {
                IngMyReplyList.Add(item);
            }
            IsLoading = false;
        }

        private void GetIngMy(IEnumerable<Ing> ingList)
        {
            if (!IngMyInitialized)
            {
                IngMyList.Clear();
                this.llsIngMy.Visibility = System.Windows.Visibility.Visible;
                IngMyInitialized = true;
            }
            foreach (var item in ingList)
            {
                IngMyList.Add(item);
            }
            IsLoading = false;
        }

        private void GetIngRecent(IEnumerable<Ing> ingList)
        {
            if (!IngRecentInitialized)
            {
                IngRecentList.Clear();
                this.llsIngRecent.Visibility = System.Windows.Visibility.Visible;
                IngRecentInitialized = true;
            }
            foreach (var item in ingList)
            {
                IngRecentList.Add(item);
            }
            IsLoading = false;
        }

        private IEnumerable<IngReplyToMe> GetIngReplyToMeList(string strHtml)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(strHtml);
            var feedList = doc.DocumentNode.ChildNodes["div"];
            if (feedList != null && feedList.HasChildNodes)
            {
                var commentList = feedList.ChildNodes["ul"];
                if (commentList != null && commentList.HasChildNodes)
                {
                    foreach (var liNode in commentList.ChildNodes)
                    {
                        if (liNode.Name != "li")
                            continue;
                        IngReplyToMe ingReplyToMe = new IngReplyToMe();
                        foreach (var childNode in liNode.ChildNodes)
                        {
                            if (childNode.Name == "div" && childNode.Attributes["class"] != null && childNode.Attributes["class"].Value == "feed_avatar")
                            {
                                var authorHomeUri = childNode.ChildNodes["a"].Attributes["href"].Value;
                                var avatarImgUri = childNode.ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;
                                ingReplyToMe.ReplyAuthorHomeUri = "http://home.cnblogs.com" + authorHomeUri;
                                ingReplyToMe.BloggerAvatarImgUri = avatarImgUri;
                            }
                            else if (childNode.Name == "div" && childNode.Attributes["id"] != null && childNode.Attributes["id"].Value.StartsWith("feed_content_"))
                            {
                                var commentId = childNode.Attributes["id"].Value.Remove(0, "feed_content_".Length);
                                ingReplyToMe.CommentId = commentId;
                                if (childNode.HasChildNodes)
                                {
                                    foreach (var feedBody in childNode.ChildNodes)
                                    {
                                        if (feedBody.Name == "div" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "comment-body-topline")
                                        {
                                            foreach (var toplineNode in feedBody.ChildNodes)
                                            {
                                                if (toplineNode.Name == "a" && toplineNode.Attributes["id"] != null && toplineNode.Attributes["id"].Value.StartsWith("comment_author_"))
                                                {
                                                    var authorName = toplineNode.InnerText;
                                                    ingReplyToMe.ReplyAuthorName = authorName;
                                                }
                                                else if (toplineNode.Name == "a" && toplineNode.Attributes["class"] != null && toplineNode.Attributes["class"].Value.StartsWith("comment-body"))
                                                {
                                                    var ingDetailUri = toplineNode.Attributes["href"].Value;
                                                    var commentBody = toplineNode.InnerText;
                                                    ingReplyToMe.IngDetailUri = "http://home.cnblogs.com" + ingDetailUri;
                                                    ingReplyToMe.CommentBody = commentBody;
                                                }
                                            }
                                        }
                                        else if (feedBody.Name == "span" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_body")
                                        {
                                            var ingBody = feedBody.InnerText;
                                            HtmlAgilityPack.HtmlNode htmlNode = feedBody.ChildNodes.FirstOrDefault();
                                            if (htmlNode != null && htmlNode.Attributes["class"] != null && htmlNode.Attributes["class"].Value != "ing_time")
                                            {
                                                ingReplyToMe.ReplyBodyUrls = new List<string>();
                                                while (htmlNode != null
                                                    && htmlNode.Name == "a"
                                                    && htmlNode.Attributes["class"] != null
                                                    && htmlNode.Attributes["class"].Value == "gray")
                                                {
                                                    ingReplyToMe.ReplyBodyUrls.Add(htmlNode.Attributes["href"].Value);
                                                    htmlNode = htmlNode.NextSibling;
                                                    while (htmlNode != null && htmlNode.Name == "#text")
                                                        htmlNode = htmlNode.NextSibling;
                                                }
                                            }
                                            ingReplyToMe.ReplyBody = ingBody;
                                        }
                                        else if (feedBody.Name == "a" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_time")
                                        {
                                            var ingTime = feedBody.Attributes["title"].Value;
                                            ingReplyToMe.ReplyTime = ingTime;
                                        }
                                    }
                                }
                            }
                            else if (childNode.Name == "span" && childNode.Attributes["id"] != null && childNode.Attributes["id"].Value == "max_comment_id")
                            {
                                var maxCommentId = childNode.InnerText;
                                ingReplyToMe.MaxCommentId = maxCommentId;
                            }
                        }
                        yield return ingReplyToMe;
                    }
                }
            }
        }

        private IEnumerable<Ing> GetIngList(string strHtml)
        {
            if (!string.IsNullOrEmpty(strHtml))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(strHtml);
                foreach (var ingNode in doc.DocumentNode.ChildNodes)
                {
                    if (ingNode.Name == "li" && ingNode.HasChildNodes)
                    {
                        Ing ing = new Ing();
                        foreach (var entryNode in ingNode.ChildNodes["div"].ChildNodes)
                        {
                            if (entryNode.Name == "div" && entryNode.Attributes["class"].Value == "feed_avatar")
                            {
                                var blogUri = "http://home.cnblogs.com" + entryNode.ChildNodes["a"].Attributes["href"].Value;
                                var bloggerAvatarImgUri = entryNode.ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;
                                ing.BlogUri = blogUri;
                                ing.BloggerAvatarImgUri = bloggerAvatarImgUri;
                            }
                            else if (entryNode.Name == "div" && entryNode.Attributes["class"].Value == "feed_body")
                            {
                                foreach (var feedBody in entryNode.ChildNodes)
                                {
                                    //http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=all&PageIndex=1&PageSize=30
                                    //删除闪存 <a class="recycle" onclick="return DelIng(486880)" href="javascript:void(0);" title="删除这个闪存"><img alt="删除" src="http://static.cnblogs.com/images/icon_trash.gif"></a>
                                    if (feedBody.Name == "a" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing-author")
                                    {
                                        var author = feedBody.Attributes["href"].Value;
                                        var authorName = feedBody.InnerText;
                                        ing.Author = "http://home.cnblogs.com" + author;
                                        ing.AuthorName = authorName;
                                    }
                                    else if (feedBody.Name == "span" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_body")
                                    {
                                        //var ingBody = feedBody.InnerHtml;
                                        //将Html中的a标签筛选出来更改成HyperLink，或者不在此处处理Html，在详细页处理Html。
                                        var ingBody = feedBody.InnerText;
                                        var ingId = feedBody.Attributes["id"].Value.Remove(0, "ing_body_".Length);
                                        ing.IngBody = ingBody;
                                        ing.IngId = ingId;
                                    }
                                    else if (feedBody.Name == "a" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "recycle")
                                    {
                                        ing.CanDelete = true;
                                    }
                                    else if (feedBody.Name == "a" && feedBody.Attributes["class"].Value == "ing_time")
                                    {
                                        var ingDetailUri = "http://home.cnblogs.com" + feedBody.Attributes["href"].Value;
                                        var ingPublishTime = feedBody.Attributes["title"].Value;
                                        ingPublishTime = ingPublishTime.Substring(0, ingPublishTime.IndexOf('，'));
                                        ing.PublishTime = ingPublishTime;
                                        ing.IngDetailUri = ingDetailUri;
                                    }
                                    else if (feedBody.Name == "span" && feedBody.Attributes["id"] != null && feedBody.Attributes["id"].Value == "max_ing_id")
                                    {
                                        var maxIngId = feedBody.InnerText;
                                        ing.MaxIngId = maxIngId;
                                    }
                                    else if (feedBody.Name == "img" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_icon_lucky")
                                    {
                                        ing.IsIngLucky = "★";
                                    }
                                    else if (feedBody.Name == "img" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_icon_newbie")
                                    {
                                        ing.IsNewbie = "❀";//"♠";
                                    }
                                    else if (feedBody.Name == "a" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_reply")
                                    {
                                        var ingReplyCount = feedBody.InnerText;
                                        if (ingReplyCount == "回应")
                                            ingReplyCount = " 0回应";
                                        else
                                            ingReplyCount = " " + ingReplyCount;
                                        ing.IngReplyCount = ingReplyCount;
                                    }
                                    else if (feedBody.Name == "div" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "ing_comments")
                                    {
                                        var ulNode = feedBody.ChildNodes["div"].ChildNodes["ul"];
                                        ing.IngReply = new ObservableCollection<IngReply>();
                                        foreach (var liNode in ulNode.ChildNodes)
                                        {
                                            if (liNode.InnerText == "&nbsp;" || liNode.Attributes["id"] == null)
                                                break;
                                            IngReply ingReply = new IngReply(ing);
                                            var id = liNode.Attributes["id"].Value.Remove(0, "comment_".Length);
                                            ingReply.Id = id;
                                            foreach (var replyNode in liNode.ChildNodes)
                                            {
                                                if (replyNode.Name == "a")
                                                {
                                                    if (replyNode.Attributes["id"] != null && replyNode.Attributes["id"].Value.StartsWith("comment_author_"))
                                                    {
                                                        var commentAuthorHomeUri = replyNode.Attributes["href"].Value;
                                                        ingReply.CommentAuthorName = replyNode.InnerText;
                                                        ingReply.CommentAuthorHomeUri = "http://home.cnblogs.com" + commentAuthorHomeUri;
                                                    }
                                                    else if (replyNode.Attributes["class"] != null && replyNode.Attributes["class"].Value == "recycle")
                                                    {
                                                        ingReply.CanDelete = true;
                                                    }
                                                    else if (replyNode.Attributes["class"] != null && replyNode.Attributes["class"].Value == "ing_comment_time")
                                                    {
                                                        var replyTime = replyNode.Attributes["title"].Value;
                                                        ingReply.ReplyTime = replyTime;
                                                    }
                                                    else if (replyNode.Name == "a" && replyNode.Attributes["class"] != null && replyNode.Attributes["class"].Value == "ing_reply")
                                                    {
                                                        if (replyNode.Attributes["onclick"] != null)
                                                        {
                                                            var parentReplyNode = replyNode.Attributes["onclick"].Value;
                                                            var firstIndex = parentReplyNode.IndexOf(',') + 1;
                                                            var lastIndex = parentReplyNode.LastIndexOf(',');
                                                            parentReplyNode = parentReplyNode.Substring(firstIndex, lastIndex - firstIndex);
                                                            ingReply.ParentReplyId = parentReplyNode;
                                                        }
                                                    }
                                                }
                                                else if (replyNode.Name == "#text" && replyNode.InnerText.StartsWith("："))
                                                {
                                                    var replyBody = replyNode.InnerText;
                                                    HtmlAgilityPack.HtmlNode htmlNode = replyNode.NextSibling;
                                                    if (htmlNode != null && htmlNode.Attributes["class"] != null && htmlNode.Attributes["class"].Value != "recycle")
                                                    {
                                                        ingReply.IngALink = new List<string>();
                                                        while (htmlNode != null
                                                            && htmlNode.Name == "a"
                                                            && htmlNode.Attributes["class"] != null
                                                            && htmlNode.Attributes["class"].Value == "gray")
                                                        {
                                                            replyBody += " " + htmlNode.InnerText;
                                                            ingReply.IngALink.Add(htmlNode.Attributes["href"].Value);
                                                            htmlNode = htmlNode.NextSibling;
                                                            while (htmlNode != null && htmlNode.Name == "#text")
                                                                htmlNode = htmlNode.NextSibling;
                                                        }
                                                    }
                                                    ingReply.ReplyBody = replyBody;
                                                }
                                            }
                                            ing.IngReply.Add(ingReply);
                                        }
                                    }
                                    else if (feedBody.Name == "span" && feedBody.Attributes["class"] != null && feedBody.Attributes["class"].Value == "gray")
                                    {
                                        var selfImg = feedBody.ChildNodes["img"];
                                        if (selfImg != null && selfImg.Attributes["title"] != null && selfImg.Attributes["title"].Value == "私有闪存")
                                            ing.IsPrivate = "私";
                                    }
                                    else if (feedBody.Name == "script")
                                    {
                                        //need to get ingReply with uri; with ingId
                                        //487984 为Ing id
                                        //<script type="text/javascript">$(function(){ GetIngComments(487984,true);});</script>
                                        ing.NeedGetIngReply = true;
                                        ing.Script = "<script type=\"text/javascript\">$(function(){ GetIngComments(" + ing.IngId + ",true);});</script>";
                                    }
                                }
                            }
                        }
                        yield return ing;
                    }
                }
            }
        }
    }
}