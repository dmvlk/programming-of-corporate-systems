using LibraryManagement.Models;
using System.Windows;
namespace LibraryManagement.Views;

public partial class GenreDialogWindow : Window
{
    public Genre CurrentGenre { get; private set; }

    public GenreDialogWindow(Genre? genreToEdit = null)
    {
        InitializeComponent();
        
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

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Введите название жанра", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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