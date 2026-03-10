using LibraryManagement.Data;
using LibraryManagement.Models;
using System.Linq;
using System.Windows;
namespace LibraryManagement.Views;

public partial class GenreDialogWindow : Window
{
    private readonly ApplicationDbContext _context;
    public Genre CurrentGenre { get; private set; }

    public GenreDialogWindow(ApplicationDbContext context, Genre? genreToEdit = null)
    {
        InitializeComponent();
        _context = context;
        
        if (genreToEdit != null)
        {
            CurrentGenre = new Genre
            {
                Id = genreToEdit.Id,
                Name = genreToEdit.Name,
                Description = genreToEdit.Description
            };
            Title = "Редактирование жанра";
            LoadGenreData();
        }
        else
        {
            CurrentGenre = new Genre();
            Title = "Добавление жанра";
        }
    }

    private void LoadGenreData()
    {
        NameTextBox.Text = CurrentGenre.Name;
        DescriptionTextBox.Text = CurrentGenre.Description;
    }
    private bool CheckGenreUniqueness()
    {
        var currentId = CurrentGenre?.Id ?? 0;
        
        var existingGenre = _context.Genres
            .FirstOrDefault(g => 
                g.Name == NameTextBox.Text.Trim() &&
                g.Id != currentId);
        
        if (existingGenre != null)
        {
            var message = $"Жанр с таким названием уже существует:\n\n" +
                        $"Название: {existingGenre.Name}\n" +
                        $"Описание: {existingGenre.Description ?? "нет"}\n\n" +
                        $"Воспользуйтесь поиском жанров.";
            
            MessageBox.Show(message, "Жанр уже существует", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        
        return true;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Введите название жанра", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!CheckGenreUniqueness())
        {
            return;
        }

        CurrentGenre.Name = NameTextBox.Text.Trim();
        CurrentGenre.Description = DescriptionTextBox.Text?.Trim();

        DialogResult = true;

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}