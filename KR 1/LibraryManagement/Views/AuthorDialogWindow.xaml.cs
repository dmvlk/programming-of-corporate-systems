using LibraryManagement.Data;
using LibraryManagement.Models;
using System;
using System.Linq;
using System.Windows;

namespace LibraryManagement.Views;

public partial class AuthorDialogWindow : Window
{
    private readonly ApplicationDbContext _context;
    public Author CurrentAuthor { get; private set; }

    public AuthorDialogWindow(ApplicationDbContext context, Author? authorToEdit = null)
    {
        InitializeComponent();
        _context = context;
        
        if (authorToEdit != null)
        {
            CurrentAuthor = new Author
            {
                Id = authorToEdit.Id,
                FirstName = authorToEdit.FirstName,
                LastName = authorToEdit.LastName,
                MiddleName = authorToEdit.MiddleName,
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
        LastNameTextBox.Text = CurrentAuthor.LastName;
        FirstNameTextBox.Text = CurrentAuthor.FirstName;
        MiddleNameTextBox.Text = CurrentAuthor.MiddleName;
        BirthDatePicker.SelectedDate = CurrentAuthor.BirthDate;
        CountryTextBox.Text = CurrentAuthor.Country;
    }

    private bool CheckAuthorUniqueness()
    {
        var currentId = CurrentAuthor?.Id ?? 0;
        
        var existingAuthor = _context.Authors
            .FirstOrDefault(a => 
                a.FirstName == FirstNameTextBox.Text.Trim() &&
                a.LastName == LastNameTextBox.Text.Trim() &&
                a.MiddleName == (string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ? null : MiddleNameTextBox.Text.Trim()) &&
                a.BirthDate == BirthDatePicker.SelectedDate &&
                a.Country == CountryTextBox.Text.Trim() &&
                a.Id != currentId);
        
        if (existingAuthor != null)
        {
            var message = $"Такой автор уже существует:\n\n" +
                        $"Имя: {existingAuthor.FullName}\n" +
                        $"Дата рождения: {existingAuthor.BirthDate:dd.MM.yyyy}\n" +
                        $"Страна: {existingAuthor.Country}\n\n" +
                        $"Рекомендуется воспользоваться поиском авторов.";
            
            MessageBox.Show(message, "Автор уже существует", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        
        return true;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Введите фамилию автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            MessageBox.Show("Введите имя автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (BirthDatePicker.SelectedDate == null)
        {
            MessageBox.Show("Выберите дату рождения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!CheckAuthorUniqueness())
        {
            return;
        }

        CurrentAuthor.LastName = LastNameTextBox.Text.Trim();
        CurrentAuthor.FirstName = FirstNameTextBox.Text.Trim();
        CurrentAuthor.MiddleName = string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ? null : MiddleNameTextBox.Text.Trim();
        CurrentAuthor.BirthDate = BirthDatePicker.SelectedDate.Value;
        CurrentAuthor.Country = CountryTextBox.Text?.Trim() ?? "";

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}