using System;
using Microsoft.Maui.Controls;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SygnalistaNF.Pages;

namespace SygnalistaNF
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private List<string> _attachmentPaths = new List<string>();
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Developed());
        }


        private async Task<int> IncrementNumberInFileAsync(string filePath)
        {
            int number;
            if (!File.Exists(filePath))
            {
                // Jeśli plik nie istnieje, utwórz go i zapisz liczbę początkową
                number = 0;
                File.WriteAllText(filePath, number.ToString());
            }
            else
            {
                // Jeśli plik istnieje, odczytaj liczbę
                string fileContent = File.ReadAllText(filePath);
                if (!int.TryParse(fileContent, out number))
                {
                    await DisplayAlert("Error", "File exists but does not contain a valid number. Setting default value 0.", "OK");
                    number = 0;  
                }
            }

            // Dodaj 1 do liczby i zapisz zaktualizowaną wartość
            number++;
            File.WriteAllText(filePath, number.ToString());
            return number;
        }

        private async void OnAddAttachmentButtonClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Wybierz zdjęcie",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                foreach (var file in result)
                {
                    _attachmentPaths.Add(file.FullPath);
                    await DisplayAlert("Plik wybrany", $"Wybrany plik: {file.FileName}", "OK");
                }
            }
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            // Obsługa kliknięcia przycisku
            string messageText = TextInput.Text;
            string selectedName = NamePicker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedName))
            {
                await DisplayAlert("Błąd", "Proszę wybrać imię z listy.", "OK");
                return;
            }
            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = true;
            IsEnabled = false;



            // Tworzenie wiadomości e-mail
            var message = new MimeMessage();
             message.From.Add(new MailboxAddress("Syngalista", "sygnalista.gal.anonim@gmail.com"));
             message.To.Add(new MailboxAddress("Odbiorca", "daniel.drapiewski@infopower.pl"));
             message.Subject = "Donos";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Cześć, {selectedName} {messageText}"
            };
            foreach (var attachmentPath in _attachmentPaths)
            {
                if (File.Exists(attachmentPath))
                {
                    bodyBuilder.Attachments.Add(attachmentPath);
                }
                else
                {
                    await DisplayAlert("Błąd", $"Załącznik nie istnieje: {attachmentPath}", "OK");
                }
            }
            message.Body = bodyBuilder.ToMessageBody();
            // Konfiguracja klienta SMTP using (var client = new SmtpClient())
            using (var client = new SmtpClient())
            {
                // Configure client to ignore SSL certificate validation errors
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("sygnalista.gal.anonim@gmail.com", "bccm oogr uzct jtyi");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    string fileName = Path.Combine(FileSystem.AppDataDirectory, "number.txt");
                    int updatedNumber = await IncrementNumberInFileAsync(fileName);
                     await DisplayAlert("Wiadomość", "E-mail został wysłany!", "OK");
                    await DisplayAlert("Donos", $"Twoja liczba donosów wynosi: {updatedNumber}.", "OK");
                    // Czyszczenie pola tekstowego
                    TextInput.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    DisplayAlert("Błąd", $"Wysłanie e-maila nie powiodło się: {ex.Message}", "OK");
                }
                finally 
                {
                    ActivityIndicator.IsRunning = false;
                    ActivityIndicator.IsVisible = false;
                    IsEnabled = true;
                }
            }

           
        }

    }
}
