using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
namespace LibraryManagement.Views;

public partial class AuthorsWindow : Window
{
    private readonly ApplicationDbContext _context;

    public AuthorsWindow(ApplicationDbContext context)
    {
        InitializeComponent();
        _context = context;
        LoadAuthors();
    }
    private void LoadAuthors()
    {
        _context.Authors.Load();
        AuthorsDataGrid.ItemsSource = _context.Authors.Local.ToObservableCollection();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AuthorDialogWindow(_context);
        if (dialog.ShowDialog() == true)
        {
            _context.Authors.Add(dialog.CurrentAuthor);
            _context.SaveChanges();
            LoadAuthors();
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedAuthor = AuthorsDataGrid.SelectedItem as Author;
        if (selectedAuthor == null)
        {
            MessageBox.Show("Выберите автора для редактирования", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new AuthorDialogWindow(_context, selectedAuthor);
        if (dialog.ShowDialog() == true)
        {
            _context.Entry(selectedAuthor).CurrentValues.SetValues(dialog.CurrentAuthor);
            _context.SaveChanges();
            LoadAuthors();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedAuthor = AuthorsDataGrid.SelectedItem as Author;
        if (selectedAuthor == null)
        {
            MessageBox.Show("Выберите автора для удаления", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        if (_context.Books.Any(b => b.AuthorId == selectedAuthor.Id))
        {
            MessageBox.Show("Нельзя удалить автора, у которого есть книги", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var result = MessageBox.Show($"Удалить автора {selectedAuthor.FirstName} {selectedAuthor.LastName}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _context.Authors.Remove(selectedAuthor);
            _context.SaveChanges();
            LoadAuthors();
        }
    }
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}