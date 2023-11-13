using Brest_Pekar;
using Brest_Pekar.Models;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Aspose.Words;

namespace Brest_Pekar.ViewModels
{
    class OrderViewModel : INotifyPropertyChanged
    {
        public static OrderViewModel Instance = null!;

        private Order? _selectedOrder { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<ProductType> ProductTypes { get; set; }
        public ObservableCollection<Store> Stores { get; set; }

        private Product? _selectedProduct { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> AllProducts { get; set; }

        public ObservableCollection<RealesedOrder> HistoryOrders { get; set; }


        public OrderViewModel()
        {
            Instance = this;

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Orders.Load();
                Orders = new ObservableCollection<Order>(db.Orders.Include(u => u.store).ThenInclude(j => j.iban).ThenInclude(k => k.bank));

                db.Product_Types.Load();
                ProductTypes = new ObservableCollection<ProductType>(db.Product_Types);

                db.Stores.Load();
                Stores = new ObservableCollection<Store>(db.Stores.Include(u => u.iban).ThenInclude(j => j.bank));

                Products = new ObservableCollection<Product>();

                AllProducts = new ObservableCollection<Product>();

                db.Realesed_Orders.Load();
                HistoryOrders = new ObservableCollection<RealesedOrder>(db.Realesed_Orders.Include(u => u.store).ThenInclude(j => j.iban).ThenInclude(k => k.bank).Include(m => m.product_type));
            }
        }

        public Order? SelectedOrder
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
                        db.Products.Load();
                        Products = new ObservableCollection<Product>(db.Products.Include(u => u.product_type).Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)));
                        AllProducts = new ObservableCollection<Product>(db.Products.Include(u => u.product_type).Where(item => item.product_typeid == _selectedOrder.product_typeid).Where(item => item.supplies.Count() == 0));  
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

                if (_selectedOrder == null) {
                    return;
                }

                using (var db = new ApplicationContext())
                {
                    if (_selectedOrder.count <= db.Products.Include(u => u.product_type).Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)).Count())
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

                    if (_selectedOrder.store.BankAccount == null)
                    {
                        MainWindow.Instance.SendSnackMessage("Не указан основной счёт!", PackIconKind.Error);
                        return;
                    }

                    if (_selectedOrder.count <= 0 ||
                        _selectedOrder.count != db.Products.Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)).Include(p => p.product_type).Count())
                    {
                        MainWindow.Instance.SendSnackMessage("Заказ не заполнен!", PackIconKind.Error);
                        return;
                    }

                    int sum = 0;
                    foreach (Product p in db.Products.Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)).Include(p => p.product_type))
                    {
                        sum += p.product_type!.price;
                    }

                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = "Document"; // Default file name
                    dlg.DefaultExt = ".docx"; // Default file extension
                    dlg.Filter = "Word documents (.docx)|*.docx"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        string filename = dlg.FileName;

                        var doc = new Document();
                        var builder = new DocumentBuilder(doc);

                        // Insert text at the beginning of the document.
                        builder.MoveToDocumentStart();
                        builder.Writeln($"Заказ №{_selectedOrder.id} от {_selectedOrder.date}");
                        builder.Writeln($"От магазина: {_selectedOrder.store!.name} по адресу {_selectedOrder.store.address}\nтел. {_selectedOrder.store.phone}");
                        builder.Writeln($"Cумма: {sum} BYN");
                        builder.Writeln($"Cчёт: {_selectedOrder.store.BankAccount.bank.name} {_selectedOrder.store.BankAccount.iban}");
                        
                        builder.Writeln("");
                        builder.Writeln("К реализации продукцию:");

                        foreach (Product p in db.Products.Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)).Include(p => p.product_type))
                        {
                            builder.Writeln($"№{p.id}\t{p.product_type.name}\t{p.date_of_production}"); 
                        }

                        builder.Writeln("");
                        builder.Writeln($"Дата: {DateTime.Now}");
                        doc.Save(filename);
                    }
                    else
                    {
                        return;
                    }


                    RealesedOrder order = new RealesedOrder();
                    order.product_type = _selectedOrder.product_type;
                    order.store = _selectedOrder.store;
                    order.count = _selectedOrder.count;
                    order.date = DateTime.Now;
                    order.money = sum;

                    db.Attach(order);
                    db.Realesed_Orders.Add(order);
                    HistoryOrders.Add(order);

                    foreach (Product p in db.Products.Where(item => item.supplies.Any(j => j.OrderId == _selectedOrder.id)))
                    {
                        db.Products.Remove(p);
                    } 
                    db.Orders.Remove(_selectedOrder);
                    Orders.Remove(_selectedOrder);

                    db.SaveChanges();

                    //db.Orders.Load();
                    //Orders = new ObservableCollection<Order>(db.Orders.Include(u => u.store).ThenInclude(j => j.iban).ThenInclude(k => k.bank));
                }
                AllProducts.Clear();
                Products.Clear();

                SelectedOrder = null;
            }
        ));

        private RelayCommand<Order> _addOrderCommand = null!;
        public RelayCommand<Order> AddOrderCommand => _addOrderCommand
            ?? (_addOrderCommand = new RelayCommand<Order>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Orders.Load();

                    if (db.Product_Types.Count() == 0)
                    {
                        MainWindow.Instance.SendSnackMessage("Ошибка! Добавьте товар", PackIconKind.Error);
                        return;
                    }

                    if (db.Stores.Count() == 0)
                    {
                        MainWindow.Instance.SendSnackMessage("Ошибка! Добавьте магазин", PackIconKind.Error);
                        return;
                    }

                    Order order = new Order();
                    order.product_type = db.Product_Types.First();
                    order.store = db.Stores.First();
                    order.date = DateTime.Now;

                    db.Orders.Attach(order);
                    db.Orders.Add(order);
                    db.SaveChanges();

                    Orders.Insert(0, order);
                    SelectedOrder = order;
                }
            }
        ));

        private RelayCommand<Order> _removeOrderCommand = null!;
        public RelayCommand<Order> RemoveOrderCommand => _removeOrderCommand
            ?? (_removeOrderCommand = new RelayCommand<Order>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Orders.Load();

                    if (db.Orders.Contains(obj))
                    {
                        db.Orders.Remove(obj);
                        db.SaveChanges();
                    }

                    Orders.Remove(obj);
                }
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
