using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaterialList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Хранить общее количество страниц
        private int totalPageCount = 0;
        // Хранить номер текущей страницы
        private int currentPage = 0;
        // Подключение к базе
        // sql - папка, в которую добавил модель
        // userXEntities - название объекта, указанное во время
        // добавления Модели ADO.NET
        public sql.userXEntities connectDB = new sql.userXEntities();
        public MainWindow()
        {
            InitializeComponent();

            // Вызвать функцию загрузки материалов
            // первой страницы (в момент открытия программы)
            BuildMaterials(0);
        }

        /// <summary>
        /// Сформировать страницу со списком материалов
        /// </summary>
        /// <param name="page">Номер страницы</param>
        void BuildMaterials(int page)
        {
            // Получить весь список материалов отсортированный по имени
            var list = connectDB.Material.OrderBy(m => m.Name).ToList();

            // Очистить StackPanel 
            MaterialList.Children.Clear();

            int pages = page * 3;

            for (int i = pages; i < pages + 3; i++)
            {
                Border border = new Border();
                border.Margin = new Thickness(20, 10, 20, 10);
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Brushes.Black;

                StackPanel stack1 = new StackPanel();
                stack1.Orientation = Orientation.Horizontal;

                Image image = new Image();
                image.Width = 150;
                image.Height = 120;
                image.Margin = new Thickness(10);
                image.HorizontalAlignment = HorizontalAlignment.Left;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (list[i].Image == null)
                {
                    bitmap.UriSource = new Uri("/materials/picture.png", UriKind.Relative);
                }
                else
                {
                    bitmap.UriSource = new Uri(list[i].Image, UriKind.Relative);
                }
                bitmap.EndInit();
                image.Stretch = Stretch.UniformToFill;
                image.Source = bitmap;

                StackPanel stack2 = new StackPanel();
                stack2.Orientation = Orientation.Vertical;
                stack2.Width = 400;
                stack2.Margin = new Thickness(10);

                TextBlock tbType = new TextBlock();
                tbType.Text = list[i].MaterialType.Name + " | " + list[i].Name;
                tbType.TextWrapping = TextWrapping.Wrap;
                tbType.FontSize = 20;

                TextBlock tbCount = new TextBlock();
                tbCount.Text = "Минимальное количество: " + list[i].MinCount + " " + list[i].Unit.Name;
                tbCount.TextWrapping = TextWrapping.Wrap;
                tbCount.FontSize = 16;

                var providers = list[i].Provider.ToList();

                string a = "";
                foreach (var p in providers)
                {
                    a += p.Name + ", ";
                }

                if (a.Length > 2)
                {
                    a = a.Substring(0, a.Length - 2);
                }

                TextBlock tbProvider = new TextBlock();
                tbProvider.Text = "Поставщики: " + a;
                tbProvider.TextWrapping = TextWrapping.Wrap;
                tbProvider.FontSize = 16;

                stack2.Children.Add(tbType);
                stack2.Children.Add(tbCount);
                stack2.Children.Add(tbProvider);

                Label label = new Label();
                label.HorizontalContentAlignment = HorizontalAlignment.Right;
                label.Width = 140;
                label.Margin = new Thickness(10);
                label.FontSize = 18;
                label.Content = "Остаток " + list[i].Count + " " + list[i].Unit.Name;

                border.Child = stack1;
                stack1.Children.Add(image);
                stack1.Children.Add(stack2);
                stack1.Children.Add(label);

                MaterialList.Children.Add(border);
            }

            totalPageCount = list.Count / 3;
            currentPage = page;
            BuildPagination();
        }

        void BuildPagination()
        {
            Pagination.Children.Clear();

            Pagination.Children.Add(createTextBlock("<", false));
            for (int i = totalPageCount; i > 0; i--)
            {
                Pagination.Children.Add(createTextBlock("" + i, i == (currentPage+1)));
            }
            Pagination.Children.Add(createTextBlock(">", false));
        }

        private TextBlock createTextBlock(string text, bool underline)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = 18;
            textBlock.Text = text;
            textBlock.Margin = new Thickness(2, 5, 2, 5);
            if (underline == true)
            {
                textBlock.TextDecorations.Add(TextDecorations.Underline);
            }
            else
            {
                textBlock.Cursor = Cursors.Hand;
                textBlock.MouseLeftButtonDown += onChangePage;
            }
            return textBlock;
        }

        private void onChangePage(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                try
                {
                    if (textBlock.Text == ">")
                    {
                        if (currentPage > 0)
                        {
                            BuildMaterials(--currentPage);
                        }

                    } else
                    {
                        if (textBlock.Text == "<")
                        {
                            if ((currentPage+1) < totalPageCount)
                            {
                                BuildMaterials(++currentPage);
                            }
                        } else
                        {
                            currentPage = int.Parse(textBlock.Text);
                            BuildMaterials(currentPage-1);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
