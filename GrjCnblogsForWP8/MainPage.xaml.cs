using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GrjCnblogsForWP8.Resources;
using System.IO;
using System.Windows.Resources;
using System.Xml;
using System.Threading.Tasks;
using GrjCnblogsForWP8.Models;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO.IsolatedStorage;

namespace GrjCnblogsForWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
            LoadLoginCookies();
            //this.OrientationChanged += MainPage_OrientationChanged;
        }

        //void MainPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        //{
        //    //当横向的时候把AppBar隐藏起来
        //    if (this.Orientation == PageOrientation.Portrait || this.Orientation == PageOrientation.PortraitUp)
        //        this.ApplicationBar.IsVisible = true;
        //    else
        //        this.ApplicationBar.IsVisible = false;
        //}

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //Windows.ApplicationModel.Core.CoreApplication.Exit();
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void LoadLoginCookies()
        {
            string loginFileName = "loginCookies";
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.FileExists(loginFileName))
                    return;
                using (IsolatedStorageFileStream fileStream = isf.OpenFile(loginFileName, FileMode.Open, FileAccess.Read))
                {
                    XmlReader xr = XmlReader.Create(fileStream);
                    CookieCollection cookies = new CookieCollection();
                    while (xr.Read())
                    {
                        switch (xr.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xr.Name == "Cookie")
                                {
                                    string cookieName = xr.GetAttribute("CookieName");
                                    string cookieValue = xr.GetAttribute("CookieValue");
                                    string cookieDomain = xr.GetAttribute("CookieDomain");
                                    string cookieExpired = xr.GetAttribute("CookieExpired");
                                    string cookieExpires = xr.GetAttribute("CookieExpires");
                                    string cookieHttpOnly = xr.GetAttribute("CookieHttpOnly");
                                    string cookiePath = xr.GetAttribute("CookiePath");

                                    Cookie cookie = new Cookie(cookieName, cookieValue, cookiePath, cookieDomain);
                                    cookie.Expired = Convert.ToBoolean(cookieExpired);
                                    cookie.Expires = Convert.ToDateTime(cookieExpires);
                                    cookie.HttpOnly = Convert.ToBoolean(cookieHttpOnly);
                                    cookies.Add(cookie);
                                    Constants.CookieContainer.Add(new Uri("http://www.cnblogs.com/"), cookies);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void HomeNav_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            NavItem nav = fe.DataContext as NavItem;
            if (string.IsNullOrEmpty(nav.Uri))
                return;
            this.NavigationService.Navigate(new Uri(nav.Uri, UriKind.Relative));
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}