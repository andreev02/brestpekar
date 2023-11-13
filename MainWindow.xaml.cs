using Brest_Pekar.Models;
using Brest_Pekar.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Brest_Pekar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; } = null!;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            //new ApplicationContext().Database.EnsureDeleted();
        }

        private void theme_button_Click(object sender, RoutedEventArgs e)
        {             
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(theme_button.IsChecked == true ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);

            if (theme_button.IsChecked == true)
            {
                LiveCharts.Configure(config => config.AddDarkTheme());
            }
            else
            {
                LiveCharts.Configure(config => config.AddLightTheme());
            }

            SendSnackMessage("Тема изменена", PackIconKind.None);
        }

        private async void tabcontrol_Selected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null && tab.IsSelected)
            {
                ProgressCircle.Visibility = Visibility.Visible;
                await Task.Delay(1000);
                if (tab == tabitem_productTypes)
                {
                    ProductTypeViewModel model = null!;
                    await Task.Run(() => model = new ProductTypeViewModel());
                    try { tabcontrol.DataContext = model; } catch { }
                }
                else if (tab == tabitem_products)
                {
                    ProductViewModel model = null!;
                    await Task.Run(() => model = new ProductViewModel());
                    try { tabcontrol.DataContext = model; } catch { }
                }
                else if (tab == tabitem_stores)
                {
                    StoreViewModel model = null!;
                    await Task.Run(() => model = new StoreViewModel());
                    try {  tabcontrol.DataContext = model; grid_stores.ScrollIntoView(StoreViewModel.Instance.Stores[0]); } catch { }
                }
                else if (tab == tabitem_orders)
                {
                    OrderViewModel model = null!;
                    await Task.Run(() => model = new OrderViewModel());
                    try { tabcontrol.DataContext = model; } catch { }
                }
                else if (tab == tabitem_statistic)
                {
                    StatisticViewModel model = null!;
                    await Task.Run(() => model = new StatisticViewModel());
                    try { tabcontrol.DataContext = model; } catch { }
                }
                ProgressCircle.Visibility = Visibility.Hidden;
            }
        }

        public void SendSnackMessage(string message, PackIconKind kind)
        {
            SnackbarInfo.MessageQueue!.Clear();

            if (kind == PackIconKind.None)
            {
                SnackPackIcon.Width = 0;
            }
            else
            {
                SnackPackIcon.Width = 24;
            }
            SnackPackIcon.Kind = kind;
            SnackbarText.Text = message;

            SnackbarInfo.Message = SnackbarMessage;
            SnackbarInfo.MessageQueue!.Enqueue(SnackbarInfo.Message.Content);
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            /*MessageBox.Show("test1");
            var datagrid = sender as DataGrid;
            datagrid!.SelectedIndex = -1;*/
        }

        private void PopupBox_Closed(object sender, RoutedEventArgs e)
        {
            //(sender as PopupBox).op
        }

        private void PekarLink_Click(object sender, RoutedEventArgs e)
        {
            var ps = new ProcessStartInfo("http://bhp.by/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void BGKLPLink_Click(object sender, RoutedEventArgs e)
        {
            var ps = new ProcessStartInfo("http://bgklp.by/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void webView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            //53.142251, 26.035902
            //53.142251, 26.035902
            //MessageBox.Show(webView.Source.ToString());
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Store store = StoreViewModel.Instance.SelectedStore;
            if (store == null)
            {
                SendSnackMessage("Выберите магазин в таблице!", PackIconKind.Error);
                return;
            }

            string name = await webView.ExecuteScriptAsync("document.getElementsByClassName(\"DUwDvf fontHeadlineLarge\")[0].textContent");
            string address = await webView.ExecuteScriptAsync("document.getElementsByClassName(\"Io6YTe fontBodyMedium \")[0].textContent");

            if (name.Equals("null"))
            {
                SendSnackMessage("Выберите магазин на карте!", PackIconKind.Error);
                return;
            }

            bool isPhoneFind = false;
            string phone = "";
            for (int i = 0; i < 5; i++)
            {
                phone = await webView.ExecuteScriptAsync($"document.getElementsByClassName(\"Io6YTe fontBodyMedium \")[{i}].textContent");
                if (phone[1] == '8')
                {
                    isPhoneFind = true;
                    break;
                }
            }

            name = name.Substring(1, name.Length - 2);
            address = address.Substring(1, address.Length - 2);
            phone = phone.Substring(1, phone.Length - 2);

            if (isPhoneFind == false)
            {
                phone = "-";
            }

            store.Name = name;
            store.Address = address;
            store.Phone = phone;
        }

        private void DataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void grid_stores_Selected(object sender, RoutedEventArgs e)
        {
            if (StoreViewModel.Instance.SelectedStore == null) return;
            if (StoreViewModel.Instance.SelectedStore.address.Length <= 1) return;
            webView.Source = new Uri($"https://www.google.com/maps/place/{StoreViewModel.Instance.SelectedStore.address} {StoreViewModel.Instance.SelectedStore.name}");
            /*await webView.ExecuteScriptAsync("document.getElementById(\"searchboxinput\").value = \"Test\"");
            string test = await webView.ExecuteScriptAsync("document.getElementById(\"searchboxinput\").fireEvent(\"input\")");
            MessageBox.Show(test);*/
            await Task.Delay(1000);
            await webView.ExecuteScriptAsync("document.getElementById(\"searchbox-searchbutton\").click()");
        }
        private async void grid_orders_Selected(object sender, RoutedEventArgs e)
        {
            if (OrderViewModel.Instance.SelectedOrder == null) return;
            if (OrderViewModel.Instance.SelectedOrder.store == null) return;
            if (OrderViewModel.Instance.SelectedOrder.store.address.Length <= 1) return;
            webView2.Source = new Uri($"https://www.google.com/maps/dir/Барановичский хлебозавод филиал ОАО Брестхлебпром, улица Текстильная, Барановичи/{OrderViewModel.Instance.SelectedOrder.store.address} {OrderViewModel.Instance.SelectedOrder.store.name}");
            /*await webView.ExecuteScriptAsync("document.getElementById(\"searchboxinput\").value = \"Test\"");
            string test = await webView.ExecuteScriptAsync("document.getElementById(\"searchboxinput\").fireEvent(\"input\")");
            MessageBox.Show(test);*/
            await Task.Delay(1000);
            await webView2.ExecuteScriptAsync("document.getElementById(\"searchbox-searchbutton\").click()");
        }
    }
}