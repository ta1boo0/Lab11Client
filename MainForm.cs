using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Lab11Client;

public class MainForm : Form
{
    private readonly TextBox output = new();
    private readonly Button requestButton = new();
    private readonly Label title = new();
    private static readonly HttpClient httpClient = new();

    public MainForm()
    {
        Text = "Лабораторная работа №11 — HTTP-клиент";
        Width = 560;
        Height = 330;
        StartPosition = FormStartPosition.CenterScreen;

        title.Text = "Получение данных от API";
        title.Left = 20;
        title.Top = 20;
        title.Width = 480;
        title.Height = 30;
        title.Font = new Font("Segoe UI", 12, FontStyle.Bold);

        output.Left = 20;
        output.Top = 65;
        output.Width = 490;
        output.Height = 130;
        output.Multiline = true;
        output.ReadOnly = true;
        output.ScrollBars = ScrollBars.Vertical;

        requestButton.Text = "Получить случайную запись";
        requestButton.Left = 20;
        requestButton.Top = 215;
        requestButton.Width = 240;
        requestButton.Height = 35;
        requestButton.Click += RequestButton_Click;

        Controls.Add(title);
        Controls.Add(output);
        Controls.Add(requestButton);
    }

    private async void RequestButton_Click(object? sender, EventArgs e)
    {
        requestButton.Enabled = false;
        try
        {
            Person? person = await httpClient.GetFromJsonAsync<Person>(
                "http://localhost:5041/");

            if (person is null)
            {
                output.Text = "Сервер вернул пустой ответ.";
                return;
            }

            output.Text =
                $"Имя: {person.Name}{Environment.NewLine}" +
                $"Возраст: {person.Age}{Environment.NewLine}" +
                $"Активен: {(person.IsActive ? "Да" : "Нет")}";

            // Проверяем имя: разрешены только русские буквы, пробел и дефис.
            bool hasInvalidCharacters =
                Regex.IsMatch(person.Name, @"[^А-Яа-яЁё\s-]");

            if (hasInvalidCharacters)
            {
                MessageBox.Show(
                    "Обнаружены недопустимые символы в имени!",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (HttpRequestException)
        {
            MessageBox.Show(
                "Не удалось подключиться к API. Сначала запустите проект Lab11Server.",
                "Ошибка соединения",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка обработки ответа: {ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            requestButton.Enabled = true;
        }
    }

    private record Person(string Name, int Age, bool IsActive);
}
