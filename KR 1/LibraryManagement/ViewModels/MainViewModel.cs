using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using LibraryManagement.Views;
namespace LibraryManagement.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ApplicationDbContext _context;
    private ObservableCollection<Book> _books = new();
    private ObservableCollection<Author> _authors = new();
    private ObservableCollection<Genre> _genres = new();
    private string _searchText = string.Empty;
    private Author? _selectedAuthorFilter;
    private Genre? _selectedGenreFilter;
    private Book? _selectedBook;

    public ObservableCollection<Book> Books
    {
        get => _books;
        set => SetProperty(ref _books, value);
    }

    public ObservableCollection<Author> Authors
    {
        get => _authors;
        set => SetProperty(ref _authors, value);
    }

    public ObservableCollection<Genre> Genres
    {
        get => _genres;
        set => SetProperty(ref _genres, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                FilterBooks();
        }
    }

    public Author? SelectedAuthorFilter
    {
        get => _selectedAuthorFilter;
        set
        {
            if (SetProperty(ref _selectedAuthorFilter, value))
                FilterBooks();
        }
    }

    public Genre? SelectedGenreFilter
    {
        get => _selectedGenreFilter;
        set
        {
            if (SetProperty(ref _selectedGenreFilter, value))
                FilterBooks();
        }
    }

    public Book? SelectedBook
    {
        get => _selectedBook;
        set => SetProperty(ref _selectedBook, value);
    }

    public ICommand AddBookCommand { get; }
    public ICommand EditBookCommand { get; }
    public ICommand DeleteBookCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ManageAuthorsCommand { get; }
    public ICommand ManageGenresCommand { get; }

    public MainViewModel()
    {
        _context = new ApplicationDbContext();
        _context.Database.EnsureCreated();

        LoadData();

        AddBookCommand = new RelayCommand(AddBook);
        EditBookCommand = new RelayCommand(EditBook, () => SelectedBook != null);
        DeleteBookCommand = new RelayCommand(DeleteBook, () => SelectedBook != null);
        RefreshCommand = new RelayCommand(LoadData);
        ManageAuthorsCommand = new RelayCommand(ManageAuthors);
        ManageGenresCommand = new RelayCommand(ManageGenres);
    }

    private void LoadData()
    {
        _context.Authors.Load();
        _context.Genres.Load();
        _context.Books.Include(b => b.Author).Include(b => b.Genre).Load();

        Authors = new ObservableCollection<Author>(_context.Authors.Local);
        Authors.Insert(0, new Author { Id = 0, LastName = "Все авторы" });
        
        Genres = new ObservableCollection<Genre>(_context.Genres.Local);
        Genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });

        SelectedAuthorFilter = Authors[0];
        SelectedGenreFilter = Genres[0];
        SearchText = string.Empty;
        FilterBooks();
    }

    private void FilterBooks()
    {
        var query = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            query = query.Where(b => b.Title.Contains(SearchText));

        if (SelectedAuthorFilter != null && SelectedAuthorFilter.Id != 0)
            query = query.Where(b => b.AuthorId == SelectedAuthorFilter.Id);

        if (SelectedGenreFilter != null && SelectedGenreFilter.Id != 0)
            query = query.Where(b => b.GenreId == SelectedGenreFilter.Id);

        Books = new ObservableCollection<Book>(query.ToList());
    }

    private void AddBook()
    {
        var bookWindow = new BookWindow(_context);
        if (bookWindow.ShowDialog() == true)
        {
            if (bookWindow.CurrentBook.Id == 0)
            {
                _context.Books.Add(bookWindow.CurrentBook);
            }
            _context.SaveChanges();
            LoadData();
        }
    }

    private void EditBook()
    {
        if (SelectedBook == null) return;
        var bookWindow = new BookWindow(_context, SelectedBook);
        if (bookWindow.ShowDialog() == true)
        {
            _context.Entry(SelectedBook).CurrentValues.SetValues(bookWindow.CurrentBook);
            _context.SaveChanges();
            LoadData();
        }
        
    }

    private void DeleteBook()
    {
        if (SelectedBook == null) return;
        
        var result = MessageBox.Show(
            $"Удалить книгу '{SelectedBook.Title}'?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _context.Books.Remove(SelectedBook);
            _context.SaveChanges();
            LoadData();
        }
    }

    private void ManageAuthors()
    {
        var authorsWindow = new AuthorsWindow(_context);
        authorsWindow.ShowDialog();
        LoadData();
    }



    private void ManageGenres()
    {
        var genresWindow = new GenresWindow(_context);
        genresWindow.ShowDialog();
        LoadData();
    }
}