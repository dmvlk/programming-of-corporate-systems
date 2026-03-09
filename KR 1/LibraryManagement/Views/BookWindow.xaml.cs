using LibraryManagement.Models;
using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace LibraryManagement.Views;

public partial class BookWindow : Window
{
    private readonly ApplicationDbContext _context;
    public Book CurrentBook { get; private set; }
    private ObservableCollection<Author> _selectedAuthors;
    private ObservableCollection<Genre> _selectedGenres;

    public BookWindow(ApplicationDbContext context, Book? bookToEdit = null)
    {
        InitializeComponent();
        _context = context;
        
        _selectedAuthors = new ObservableCollection<Author>();
        _selectedGenres = new ObservableCollection<Genre>();
        
        SelectedAuthorsListBox.ItemsSource = _selectedAuthors;
        SelectedGenresListBox.ItemsSource = _selectedGenres;
        
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
        
        if (CurrentBook.Authors != null)
        {
            foreach (var author in CurrentBook.Authors)
                _selectedAuthors.Add(author);
        }
        
        if (CurrentBook.Genres != null)
        {
            foreach (var genre in CurrentBook.Genres)
                _selectedGenres.Add(genre);
        }
    }

    private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        if (AuthorComboBox.SelectedItem is Author selectedAuthor)
        {
            if (!_selectedAuthors.Any(a => a.Id == selectedAuthor.Id))
                _selectedAuthors.Add(selectedAuthor);
            AuthorComboBox.SelectedItem = null;
        }
        else
        {
            MessageBox.Show("Выберите автора из списка", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void RemoveAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedAuthorsListBox.SelectedItem is Author selectedAuthor)
        {
            _selectedAuthors.Remove(selectedAuthor);
        }
        else
        {
            MessageBox.Show("Выберите автора в списке ниже для удаления", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void CreateAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AuthorDialogWindow(_context);
        if (dialog.ShowDialog() == true)
        {
            _context.Authors.Add(dialog.CurrentAuthor);
            _context.SaveChanges();
            
            _context.Authors.Load();
            AuthorComboBox.ItemsSource = _context.Authors.Local.ToObservableCollection();
            
            _selectedAuthors.Add(dialog.CurrentAuthor);
        }
    }

    private void AddGenreButton_Click(object sender, RoutedEventArgs e)
    {
        if (GenreComboBox.SelectedItem is Genre selectedGenre)
        {
            if (!_selectedGenres.Any(g => g.Id == selectedGenre.Id))
                _selectedGenres.Add(selectedGenre);
            GenreComboBox.SelectedItem = null;
        }
        else
        {
            MessageBox.Show("Выберите жанр из списка", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void RemoveGenreButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedGenresListBox.SelectedItem is Genre selectedGenre)
        {
            _selectedGenres.Remove(selectedGenre);
        }
        else
        {
            MessageBox.Show("Выберите жанр в списке ниже для удаления", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void CreateGenreButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new GenreDialogWindow();
        if (dialog.ShowDialog() == true)
        {
            _context.Genres.Add(dialog.CurrentGenre);
            _context.SaveChanges();
            
            _context.Genres.Load();
            GenreComboBox.ItemsSource = _context.Genres.Local.ToObservableCollection();
            
            _selectedGenres.Add(dialog.CurrentGenre);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Введите название книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_selectedAuthors.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы одного автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_selectedGenres.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы один жанр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        CurrentBook.Title = TitleTextBox.Text.Trim();
        CurrentBook.ISBN = ISBNTextBox.Text.Trim();
        CurrentBook.PublishYear = year;
        CurrentBook.Publisher = PublisherTextBox.Text.Trim();
        CurrentBook.QuantityInStock = quantity;
        
        CurrentBook.Authors.Clear();
        foreach (Author author in _selectedAuthors)
            CurrentBook.Authors.Add(author);
        
        CurrentBook.Genres.Clear();
        foreach (Genre genre in _selectedGenres)
            CurrentBook.Genres.Add(genre);

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}