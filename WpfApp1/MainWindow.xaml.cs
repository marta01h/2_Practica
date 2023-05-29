using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;

public class Note
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}

// Класс для сериализации и десериализации данных
public class DataManager
{
    public void SerializeNotes(List<Note> notes, string filePath)
    {
        string json = JsonConvert.SerializeObject(notes);
        File.WriteAllText(filePath, json);
    }

    public List<Note> DeserializeNotes(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Note>>(json);
        }
        return new List<Note>();
    }
}

// Главное окно WPF
public partial class MainWindow : Window
{
    private string dataFilePath = "notes.json";
    private List<Note> allNotes;
    private List<Note> displayedNotes;

    public MainWindow()
    {
        InitializeComponent();
        LoadNotes();
        datePicker.SelectedDate = DateTime.Today;
    }

    private void LoadNotes()
    {
        DataManager dataManager = new DataManager();
        allNotes = dataManager.DeserializeNotes(dataFilePath);
    }

    private void SaveNotes()
    {
        DataManager dataManager = new DataManager();
        dataManager.SerializeNotes(allNotes, dataFilePath);
    }

    private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Today;
        displayedNotes = allNotes.Where(note => note.Date.Date == selectedDate.Date).ToList();
        notesListBox.ItemsSource = displayedNotes;
    }

    private void notesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Note selectedNote = notesListBox.SelectedItem as Note;
        if (selectedNote != null)
        {
            titleTextBox.Text = selectedNote.Title;
            descriptionTextBox.Text = selectedNote.Description;
        }
    }

    private void addNoteButton_Click(object sender, RoutedEventArgs e)
    {
        string title = titleTextBox.Text.Trim();
        string description = descriptionTextBox.Text.Trim();
        DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Today;

        if (!string.IsNullOrEmpty(title) && selectedDate != null)
        {
            Note newNote = new Note
            {
                Title = title,
                Description = description,
                Date = selectedDate
            };

            allNotes.Add(newNote);
            SaveNotes();

            displayedNotes.Add(newNote);
            notesListBox.ItemsSource = displayedNotes;
            notesListBox.SelectedItem = newNote;

            ClearFields();
        }
        else
        {
            MessageBox.Show("Введите текст заметки и выбирете дату");
        }
    }

    private void updateNoteButton_Click(object sender, RoutedEventArgs e)
    {
        Note selectedNote = notesListBox.SelectedItem as Note;
        if (selectedNote != null)
        {
            selectedNote.Title = titleTextBox.Text.Trim();
            selectedNote.Description = descriptionTextBox.Text.Trim();
            SaveNotes();

            notesListBox.Items.Refresh();
            ClearFields();
        }
    }

    private void deleteNoteButton_Click(object sender, RoutedEventArgs e)
    {
        Note selectedNote = notesListBox.SelectedItem as Note;
        if (selectedNote != null)
        {
            allNotes.Remove(selectedNote);
            SaveNotes();

            displayedNotes.Remove(selectedNote);
            notesListBox.ItemsSource = displayedNotes;

            ClearFields();
        }
    }

    private void ClearFields()
    {
        titleTextBox.Text = "";
        descriptionTextBox.Text = "";
    }
}
