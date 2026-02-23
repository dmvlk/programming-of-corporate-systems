using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
namespace LibraryManagement.Views;

public partial class GenresWindow : Window
{
    private readonly ApplicationDbContext _context;

    public GenresWindow(ApplicationDbContext context)
    {
        InitializeComponent();
        _context = context;
        LoadGenres();
    }

    private void LoadGenres()
    {
        _context.Genres.Load();
        GenresDataGrid.ItemsSource = _context.Genres.Local.ToObservableCollection();
    }
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new GenreDialogWindow();
        if (dialog.ShowDialog() == true)
        {
            _context.Genres.Add(dialog.CurrentGenre);
            _context.SaveChanges();
            LoadGenres();
        }
    }
    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedGenre = GenresDataGrid.SelectedItem as Genre;
        if (selectedGenre == null)
        {
            MessageBox.Show("Выберите жанр для редактирования", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new GenreDialogWindow(selectedGenre);
        if (dialog.ShowDialog() == true)
        {
            _context.Entry(selectedGenre).CurrentValues.SetValues(dialog.CurrentGenre);
            _context.SaveChanges();
            LoadGenres();
        }
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedGenre = GenresDataGrid.SelectedItem as Genre;
        if (selectedGenre == null)
        {
            MessageBox.Show("Выберите жанр для удаления", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        if (_context.Books.Any(b => b.GenreId == selectedGenre.Id))
        {
            MessageBox.Show("Нельзя удалить жанр, к которому привязаны книги", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Удалить жанр '{selectedGenre.Name}'?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _context.Genres.Remove(selectedGenre);
            _context.SaveChanges();
            LoadGenres();
        }
    }
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}