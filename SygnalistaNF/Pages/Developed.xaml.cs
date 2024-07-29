using System;
using System.IO;
using System.Threading.Tasks;
namespace SygnalistaNF.Pages;

public partial class Developed : ContentPage
{
	public Developed()
	{
		InitializeComponent();
        string fileName = Path.Combine(FileSystem.AppDataDirectory, "number.txt");
        int number = CheckAndCreateFileAsync(fileName);

    }


    private int CheckAndCreateFileAsync(string filePath)
    {
        int number;
        if (!File.Exists(filePath))
        {
            // Jeœli plik nie istnieje, utwórz go i zapisz liczbê
            number = 0;
            File.WriteAllText(filePath, number.ToString());
        }
        else
        {
            // Jeœli plik istnieje, odczytaj liczbê
            string fileContent = File.ReadAllText(filePath);
            if (!int.TryParse(fileContent, out number))
            {
                DisplayAlert("Error", "File exists but does not contain a valid number. Setting default value 0.", "OK");
                number = 0; // lub inna wartoœæ domyœlna
                File.WriteAllText(filePath, number.ToString());
            }
        }
        return number;
    }

}