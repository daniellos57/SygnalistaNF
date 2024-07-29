using System;
using Microsoft.Maui.Controls;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace SygnalistaNF
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
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





            // Tworzenie wiadomości e-mail
             var message = new MimeMessage();
             message.From.Add(new MailboxAddress("Syngalista", "PRIVATE"));
             message.To.Add(new MailboxAddress("Odbiorca", "PRIVATE"));
             message.Subject = "Donos";
             message.Body = new TextPart("plain")
             {
                 Text = $"Cześć, {selectedName} {messageText}"
             };

            // Konfiguracja klienta SMTP using (var client = new SmtpClient())
            using (var client = new SmtpClient())
            {
                // Configure client to ignore SSL certificate validation errors
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("PRIVATE");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                     await DisplayAlert("Wiadomość", "E-mail został wysłany!", "OK");

                    // Czyszczenie pola tekstowego
                    TextInput.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    DisplayAlert("Błąd", $"Wysłanie e-maila nie powiodło się: {ex.Message}", "OK");
                }
            }
        

        }

    }
}
