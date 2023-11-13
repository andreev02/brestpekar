using Brest_Pekar;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Brest_Pekar.Models;
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

namespace Brest_Pekar.ViewModels
{
    class ProductViewModel : INotifyPropertyChanged
    {
        private Product? _selectedProduct { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<ProductType> ProductTypes { get; set; }

        public ProductViewModel()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Products.Load();
                Products = new ObservableCollection<Product>(db.Products.Include(p => p.product_type));

                db.Product_Types.Load();
                ProductTypes = new ObservableCollection<ProductType>(db.Product_Types);
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

        private RelayCommand<Product> _addProductCommand = null!;
        public RelayCommand<Product> AddProductCommand => _addProductCommand
            ?? (_addProductCommand = new RelayCommand<Product>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Products.Load();

                    if (db.Product_Types.Count() == 0)
                    {
                        MainWindow.Instance.SendSnackMessage("Ошибка! Добавьте товар", PackIconKind.Error);
                        return;
                    }

                    Product product = new Product();
                    product.product_type = db.Product_Types.First();
                    product.date_of_production = DateTime.Now;

                    db.Products.Attach(product);
                    db.Products.Add(product);
                    db.SaveChanges();

                    Products.Insert(0, product);
                    SelectedProduct = product;
                }
            }
        ));

        private RelayCommand<Product> _removeProductCommand = null!;
        public RelayCommand<Product> RemoveProductCommand => _removeProductCommand
            ?? (_removeProductCommand = new RelayCommand<Product>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Products.Load();

                    if (db.Products.Contains(obj))
                    {
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }

                    Products.Remove(obj);
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
