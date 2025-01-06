using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMonetaryManager;


public partial class MainWindow : Window
{
    private const string FilePath = "SaveFile.txt";
    private ArrayList newElements = new ArrayList();
    
    
    public MainWindow()
    {
        InitializeComponent();
        using FileStream fileStream = OpenOrCreateFile(FilePath);
        fileStream.Seek(0, SeekOrigin.Begin);
        using StreamReader reader = new StreamReader(fileStream);
        string content = reader.ReadToEnd();
        string[] lines = content.Split("\n");
        foreach (string line in lines)
        {
            string[] parts = line.Trim().Split("/");
            if (parts[0] == "+")
            {
                AddIncome(int.Parse(parts[1]), parts[2]);
            }
            else if (parts[0] == "-")
            {
                AddExpanse(int.Parse(parts[1]), parts[2]);
            }
        }
    }

    private int BALANCE = 0;

    private void SaveLine(string line)
    {
        using FileStream fileStream = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.None);
        using StreamWriter writer = new StreamWriter(fileStream);
        writer.WriteLine(line);
        writer.Flush();
    }

    
    private void AddIncome(int income, string description)
    {
        MonetaryList.Items.Add(
            new TextBlock
            {
                Text = $"{description} - {income} Ft",
                Foreground = Brushes.Green,
            });
        BALANCE += income;
        Balance.Text = BALANCE.ToString();
    }
    
    private void AddExpanse(int expanse, string description)
    {
        MonetaryList.Items.Add(
            new TextBlock
            {
                Text = $"{description} - {expanse} Ft",
                Foreground = Brushes.Red,
            });
        BALANCE -= expanse;
        Balance.Text = BALANCE.ToString();
    }

    private bool ValidateInput()
    {
        try
        {
            int.Parse(Amount.Text);
            if (Description.Text.Length is > 4 and < 31)
            {
                return true;
            }
            else
            {
                MessageBox.Show("A leírásnak legalábbb 3 maximum 30 karakterből kell állnia..");
                return false;
            }
        }
        catch (Exception ignored)
        {
            MessageBox.Show("Összeg nem értelmezhető.");
            return false;
        }
    }

    private void IncomeButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ValidateInput())
        {
            AddIncome(int.Parse(Amount.Text),Description.Text);
            newElements.Add($"+/{int.Parse(Amount.Text)}/{Description.Text}");

        }

    }

    private void ExpanseButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ValidateInput())
        {
            AddExpanse(int.Parse(Amount.Text),Description.Text);
            newElements.Add($"-/{int.Parse(Amount.Text)}/{Description.Text}");
        }
    }
    
    private static FileStream OpenOrCreateFile(string filePath)
    {
        try
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                using FileStream file = File.Create(filePath);
                Console.WriteLine($"File created: {filePath}");
            }
            
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            Console.WriteLine($"File opened for reading and writing: {filePath}");
            return fileStream;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (string newElement in newElements)
        {
            SaveLine(newElement);
        }
        newElements.Clear();
    }
}