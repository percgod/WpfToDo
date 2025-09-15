using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace TodoApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();
        private const string FilePath = "tasks.xml";

        public MainWindow()
        {
            InitializeComponent();
            TasksListBox.ItemsSource = tasks;
            LoadTasks();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskTextBox.Text))
            {
                tasks.Add(new TaskItem { Text = TaskTextBox.Text });
                TaskTextBox.Clear();
                TaskTextBox.Focus();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem != null)
            {
                tasks.Remove((TaskItem)TasksListBox.SelectedItem);
                TasksListBox.SelectedItem = null;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (tasks.Count > 0)
            {
                var result = MessageBox.Show("Очистить весь список?", "Подтверждение",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    tasks.Clear();
                }
            }
        }

        private void LoadTasks()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<TaskItem>));
                    using (var reader = new StreamReader(FilePath))
                    {
                        var loadedTasks = (ObservableCollection<TaskItem>)serializer.Deserialize(reader);
                        tasks.Clear();
                        foreach (var task in loadedTasks)
                        {
                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки задач: " + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<TaskItem>));
                using (var writer = new StreamWriter(FilePath))
                {
                    serializer.Serialize(writer, tasks);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения задач: " + ex.Message);
            }
        }
    }

    public class TaskItem
    {
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
    }
}