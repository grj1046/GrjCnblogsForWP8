using GrjCnblogsForWP8.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;

namespace GrjCnblogsForWP8.ViewModels
{
    public class IndexViewModel
    {
        public ObservableCollection<NavItem> Navigations { get; set; }

        public IndexViewModel()
        {
            LoadNavigations();
        }

        private void LoadNavigations()
        {
            //await Task.Run(() =>
            //{
            //ViewModel中不让使用Task
            //});
            this.Navigations = new ObservableCollection<NavItem>();
            Uri uri = new Uri("/GrjCnblogsForWP8;component/SiteMap.xml", UriKind.Relative);
            StreamResourceInfo stream = Application.GetResourceStream(uri);

            XElement root = XElement.Load(stream.Stream);

            var items = from view in root.Elements("View")
                        select new NavItem()
                        {
                            Title = view.Element("Title").Value,
                            Uri = view.Element("Uri").Value,
                        };
            foreach (var view in items)
            {
                this.Navigations.Add(view);
            }
        }
    }
}
