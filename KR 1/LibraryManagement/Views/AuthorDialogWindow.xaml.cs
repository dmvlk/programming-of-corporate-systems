using LibraryManagement.Data;
using LibraryManagement.Models;
using System;
using System.Windows;
namespace LibraryManagement.Views;

public partial class AuthorDialogWindow : Window
{
    public Author CurrentAuthor { get; private set; }

    public AuthorDialogWindow(ApplicationDbContext context, Author? authorToEdit = null)
    {
        InitializeComponent();
        
        if (authorToEdit != null)
        {
            CurrentAuthor = new Author
            {
                Id = authorToEdit.Id,
                FirstName = authorToEdit.FirstName,
                LastName = authorToEdit.LastName,
                BirthDate = authorToEdit.BirthDate,
                Country = authorToEdit.Country
            };
            Title = "Редактирование автора";
            LoadAuthorData();
        }
        else
        {
            CurrentAuthor = new Author();
            Title = "Добавление автора";
            BirthDatePicker.SelectedDate = DateTime.Today.AddYears(-30);
        }
    }

    private void LoadAuthorData()
    {
        FirstNameTextBox.Text = CurrentAuthor.FirstName;
        LastNameTextBox.Text = CurrentAuthor.LastName;
        BirthDatePicker.SelectedDate = CurrentAuthor.BirthDate;
        CountryTextBox.Text = CurrentAuthor.Country;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            MessageBox.Show("Введите имя автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Введите фамилию автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (BirthDatePicker.SelectedDate == null)
        {
            MessageBox.Show("Выберите дату рождения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        CurrentAuthor.FirstName = FirstNameTextBox.Text;
        CurrentAuthor.LastName = LastNameTextBox.Text;
        CurrentAuthor.BirthDate = BirthDatePicker.SelectedDate.Value;
        CurrentAuthor.Country = CountryTextBox.Text;

        DialogResult = true;

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close() ;
    }
}