using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using Brest_Pekar.Models;

namespace Brest_Pekar.ViewModels
{
    class Test
    {
        public DateTime Name;
        public int Count;
        public int Money;
        public int Products;
    }

    class StatisticViewModel : ObservableObject
    {
        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                LabelsRotation = 15,
                Labeler = value => new DateTime((long)value).ToShortDateString(),
                // set the unit width of the axis to "days"
                // since our X axis is of type date time and 
                // the interval between our points is in days
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromDays(1).Ticks
            }
        };

        public ISeries[] Series { get; set; } =
        {   
            new LineSeries<Test>
            {
                DataLabelsSize = 20,
                DataLabelsPaint = new SolidColorPaint(SKColors.Blue),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,

                DataLabelsFormatter = (point) => point.PrimaryValue.ToString("C2"),

                Name ="Income",

                Values = (from order in new ApplicationContext().Realesed_Orders
                        group order by order.date.Date into newOrder
                        select new Test { Name = newOrder.Key, Count = newOrder.Count(), Money = newOrder.Sum(p => p.money)}),

                Stroke = null,

                Mapping = (order, point) =>
                {
                    point.PrimaryValue = order.Money;
                    point.SecondaryValue = order.Name.Ticks;
                }
            },
            new LineSeries<Test>
            {
                DataLabelsSize = 20,
                DataLabelsPaint = new SolidColorPaint(SKColors.Blue),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,

                DataLabelsFormatter = (point) => point.PrimaryValue.ToString() + " ед. прод.",

                Name ="Income",

                Values = (from order in new ApplicationContext().Realesed_Orders
                        group order by order.date.Date into newOrder
                        select new Test { Name = newOrder.Key, Count = newOrder.Count(), Products = newOrder.Sum(p => p.count)}),

                Stroke = null,

                Mapping = (order, point) =>
                {
                    point.PrimaryValue = order.Products;
                    point.SecondaryValue = order.Name.Ticks;
                }
            },
            new LineSeries<Test>
            {
                DataLabelsSize = 20,
                DataLabelsPaint = new SolidColorPaint(SKColors.Blue),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,

                DataLabelsFormatter = (point) => point.PrimaryValue.ToString() + " заказ.",

                Name ="Income",

                Values = (from order in new ApplicationContext().Realesed_Orders
                        group order by order.date.Date into newOrder
                        select new Test { Name = newOrder.Key, Count = newOrder.Count(), Products = newOrder.Sum(p => p.count)}),

                Stroke = null,

                Mapping = (order, point) =>
                {
                    point.PrimaryValue = order.Count;
                    point.SecondaryValue = order.Name.Ticks;
                }
            }
        };

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Статистика",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.IndianRed)
            };
    }
}
