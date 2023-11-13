using Brest_Pekar;
using Brest_Pekar.Models;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Brest_Pekar.ViewModels
{
    class SupplyViewModel : INotifyPropertyChanged
    {
        private Order? _selectedOrder { get; set; }
        private Product? _selectedProduct { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> AllProducts { get; set; }

        public SupplyViewModel()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Orders.Load();
                Orders = new ObservableCollection<Order>(db.Orders.Include(u => u.store ));

                Products = new ObservableCollection<Product>();

                AllProducts = new ObservableCollection<Product>();
                
            }
        }

        public Order SelectedOrder
        {
            get
            {
                return _selectedOrder;
            }
            set
            {
                _selectedOrder = value;

                if (_selectedOrder != null)
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        Products = new ObservableCollection<Product>(db.Products.AsNoTracking().Include(u => u.product_type).Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)));
                        AllProducts = new ObservableCollection<Product>(db.Products.AsNoTracking().Include(u => u.product_type).Where(item => item.product_typeid == _selectedOrder.product_typeid).Where(item => item.supplies.Count() == 0));
                    }
                }

                OnPropertyChanged("Products");
                OnPropertyChanged("AllProducts");
                OnPropertyChanged("SelectedOrder");
            }
        }

        public Product SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }

        private RelayCommand<Product> _addSupplyCommand = null!;
        public RelayCommand<Product> AddSupplyCommand => _addSupplyCommand
            ?? (_addSupplyCommand = new RelayCommand<Product>(obj =>
            {
                if (obj == null)
                {
                    return;
                }

                using (var db = new ApplicationContext())
                {
                    if (_selectedOrder.count <= db.Supplies.Select(u => u.OrderId == _selectedOrder.id).Count()) 
                    {
                        MainWindow.Instance.SendSnackMessage("Заказ заполнен!", PackIconKind.TrainCarCenterbeamFull);
                        return;
                    }

                    if (_selectedOrder.products.Contains(obj))
                    {
                        return;
                    }

                    db.Supplies.Add(new Supply { OrderId = _selectedOrder.id, ProductId = obj.id });
                    db.SaveChanges();
                }
                SelectedOrder = _selectedOrder;
            }
        ));

        private RelayCommand<Product> _removeSupplyCommand = null!;
        public RelayCommand<Product> RemoveSupplyCommand => _removeSupplyCommand
            ?? (_removeSupplyCommand = new RelayCommand<Product>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Supplies.Load();
                    db.Supplies.Remove(db.Supplies.Where(u => u.ProductId == _selectedProduct.Id).First());
                    MessageBox.Show(db.ChangeTracker.ToDebugString());
                    db.SaveChanges();
                }
                SelectedOrder = _selectedOrder;
            }
        ));

        private RelayCommand<Order> _realeseOrderCommand = null!;
        public RelayCommand<Order> RealeseOrderCommand => _realeseOrderCommand
            ?? (_realeseOrderCommand = new RelayCommand<Order>(obj =>
            {
                if (_selectedOrder == null)
                {
                    MainWindow.Instance.SendSnackMessage("Выберите заказ!", PackIconKind.Error);
                    return;
                }

                using (var db = new ApplicationContext())
                {
                    db.Products.Load();
                    db.Orders.Load();

                    foreach (Product p in db.Products.Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)))
                    {
                        db.Products.Remove(p);
                    }
                    db.Orders.Remove(_selectedOrder);

                    //MessageBox.Show(db.ChangeTracker.ToDebugString());
                    db.SaveChanges();

                    db.Orders.Load();
                    Orders = new ObservableCollection<Order>(db.Orders.Include(u => u.store));
                }
                AllProducts.Clear();
                Products.Clear();

                SelectedOrder = null;
            }
        ));

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
