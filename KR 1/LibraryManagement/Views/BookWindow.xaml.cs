using LibraryManagement.Models;
using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
namespace LibraryManagement.Views;

public partial class BookWindow : Window
{
    private readonly ApplicationDbContext _context;
    public Book CurrentBook { get; private set; }

    public BookWindow(ApplicationDbContext context, Book? bookToEdit = null)
    {
        InitializeComponent();
        _context = context;
        
        LoadComboBoxes();
        
        if (bookToEdit != null)
        {
            CurrentBook = bookToEdit;
            Title = "Редактирование книги";
            LoadBookData();
        }
        else
        {
            CurrentBook = new Book();
            Title = "Добавление книги";
        }
    }

    private void LoadComboBoxes()
    {
        _context.Authors.Load();
        AuthorComboBox.ItemsSource = _context.Authors.Local.ToObservableCollection();
        
        _context.Genres.Load();
        GenreComboBox.ItemsSource = _context.Genres.Local.ToObservableCollection();
    }

    private void LoadBookData()
    {
        TitleTextBox.Text = CurrentBook.Title;
        ISBNTextBox.Text = CurrentBook.ISBN;
        YearTextBox.Text = CurrentBook.PublishYear.ToString();
        PublisherTextBox.Text = CurrentBook.Publisher;
        QuantityTextBox.Text = CurrentBook.QuantityInStock.ToString();
        
        AuthorComboBox.SelectedValue = CurrentBook.AuthorId;
        GenreComboBox.SelectedValue = CurrentBook.GenreId;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Введите название книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (AuthorComboBox.SelectedItem == null)
        {
            MessageBox.Show("Выберите автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (GenreComboBox.SelectedItem == null)
        {
            MessageBox.Show("Выберите жанр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(YearTextBox.Text, out int year) || year < 1000 || year > 2100)
        {
            MessageBox.Show("Введите корректный год (1000-2100)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
        {
            MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        CurrentBook.Title = TitleTextBox.Text;
        CurrentBook.ISBN = ISBNTextBox.Text;
        CurrentBook.PublishYear = year;
        CurrentBook.Publisher = PublisherTextBox.Text;
        CurrentBook.QuantityInStock = quantity;
        CurrentBook.AuthorId = (int)AuthorComboBox.SelectedValue;
        CurrentBook.GenreId = (int)GenreComboBox.SelectedValue;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AuthorDialogWindow(_context);
        if (dialog.ShowDialog() == true)
        {
            _context.Authors.Add(dialog.CurrentAuthor);
            _context.SaveChanges();
            
            _context.Authors.Load();
            AuthorComboBox.ItemsSource = _context.Authors.Local.ToObservableCollection();
            
            AuthorComboBox.SelectedValue = dialog.CurrentAuthor.Id;
        }
    }

    private void AddGenreButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new GenreDialogWindow();
        if (dialog.ShowDialog() == true)
        {
            _context.Genres.Add(dialog.CurrentGenre);
            _context.SaveChanges();
            
            _context.Genres.Load();
            GenreComboBox.ItemsSource = _context.Genres.Local.ToObservableCollection();
            
            GenreComboBox.SelectedValue = dialog.CurrentGenre.Id;
            
        }
    }
}