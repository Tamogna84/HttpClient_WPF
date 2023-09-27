using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpClient_WPF
{    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void DownloadPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var timer = new Stopwatch(); // таймер работы
                timer.Start();
                using var client = new HttpClient();
                string address = "https://" + textBoxAddress.Text;
                using HttpResponseMessage response = await client.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    textBoxResult.Text = result;

                    // Добавляем вывод Заголовков ответа в отдельный TextBox
                    var headers = response.Headers;
                    string headersText = string.Join(Environment.NewLine, headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));
                    textBoxHeaders.Text = headersText;

                    // Останавливаем таймер
                    timer.Stop();
                    TimeSpan elapsedTime = timer.Elapsed;
                    var statusCode = response.StatusCode;
                    MessageBox.Show($"Код ответа: {(int)statusCode} {statusCode}\n" +
                        $"Время выполнения: {elapsedTime.Seconds} секунд");
                }
                else
                {
                    MessageBox.Show($"Код состояния ответа: {(int)response.StatusCode} {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Ошибка HTTP-запроса: " + ex.Message + ". Убедитесь, что у вас есть доступ к интернету.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неверный адрес URL " + ex.Message);
            }

            //Напишите программу в WPF, которая скачивает указанную страницу.
            //Выведите тело ответа в текстовое поле.
            //Выведите код ответа.
            //Добавьте возможность сохранить содержимое страницы в файл(асинхронно).
            //Добавьте проверку на ошибки(например, обработайте кейс когда интернет-соединение отсутствует и прочие кейсы).
            //Выведите заголовки ответа.

            // Оформить на ГИТ

        }

        private async void SaveToFile_Click(object sender, EventArgs e)
        {
            try
            {                               
                await SaveTextToFileAsync();
                MessageBox.Show("Содержимое сохранено в файл.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message);
            }
        }

        private async Task SaveTextToFileAsync()
        {
            string text = $"Код страницы\n\n\n{textBoxResult.Text}\n\n\n" +
                $"Заголовки ответа\n\n\n{textBoxHeaders.Text}";
            // имя файла записывается по имени сайта
            string fileName = textBoxAddress.Text + ".txt";
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Записываем содержимое асинхронно
                await writer.WriteAsync(text);
            }
        }
    }
}
