using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace Ultimate_prediction
{
    public partial class Form1 : Form
    {
        private const string APP_NAME = "ULTIMATE_PREDICTOR";  // Имя нашего приложения
        private readonly string PREDICTIONS_CONFIG_PATH = $"{Environment.CurrentDirectory}\\predictionsConfig.json";  // Путь к конфигу с предсказаниями
        private string[] _predictions;  //  Массив строк, в который момещаются данные(предсказания)
        private Random _random = new Random();  //  Класс рандом для выбора предсказаний

        public Form1()  //  Стандартный метод
        {
            InitializeComponent();  //  Стандартный метод
        }

        private async void bPredict_Click(object sender, EventArgs e)  // Подписываемся на событие (Автоматически создался метод, который будет вызываться при нажатие на кнопку)
        {
            bPredict.Enabled = false;  //  Выключение кнопки predict для того что бы не было одновременно несколько предсказаний
            await Task.Run(() =>   //  Лямбда, чтобы заполнение прогресс бара работало в отдельном потоке
            {
                for (int i = 1; i <= 100; i++)
                {
                    this.Invoke(new Action(() =>  // Лямбда, чтобы заполнение прогресс бара работало в отдельном потоке
                    {
                        UpdateProgressBar(i);  // Метод для заполнения прогресс бара
                        this.Text = $"{i}%";  //  Отоброжение прогресс бара в процентах в поле формы
                    }));
                    
                    Thread.Sleep(20);  //  Задержка, чтобы прогресс бар не заполнялся слишком быстро
                }
            });

            var index = _random.Next(_predictions.Length);  //  Генерация рандомного числа для выбора предсказания

            var prediction = _predictions[index];  //  Генерация рандомного числа для выбора предсказания

            MessageBox.Show($"{prediction}!");  //  Вывод предсказания

            progressBar1.Value = 0;  //  Сброс прогресс бара на 0 после заполнения
            this.Text = APP_NAME;  //  Замена теста на форме на название нашего приложения
            bPredict.Enabled = true;  //  Включение кнопки после 30той строки
        }

        private void UpdateProgressBar(int i)  //  Метод для заполнения прогресс бара
        {
            // To get around the progressive animation, we need to move the progress bar backwards.
            if (i == progressBar1.Maximum)  
            {
                // Special case as value can't be set greater than Maximum.
                progressBar1.Maximum = i + 1;   // Temporarily increase Maximum
                progressBar1.Value = i + 1;     // Move past
                progressBar1.Maximum = i;       // Reset Maximum
            }
            else
            {
                progressBar1.Value = i + 1;     // Move past
            }
            progressBar1.Value = i;             // Move to correct value

        }

        private void Form1_Load(object sender, EventArgs e)  //Вызывается при создание формы при запуске приложения
        {
            this.Text = APP_NAME;  //  Замена теста на форме на название нашего приложения

            // Обработка ошибки при отсутствие файла с конфигом предсказаний
            try
            {
                var data = File.ReadAllText(PREDICTIONS_CONFIG_PATH);  //Считытвание текста из файла JSON
                _predictions = JsonConvert.DeserializeObject<string[]>(data);  //  Десериализация данных из json
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);  //  Вывод сообщения с ошибкой, если файла с конфигом не оказалось
            }
            finally
            {
                if (_predictions == null)
                {
                    Close();
                }
                else if(_predictions.Length == 0)  //  Если файл был, но пустой
                {
                    MessageBox.Show("Предсказания закончились, кина не будет! (:");  //Вывод сообщения с ошибкой, если файл был, но пустой
                    Close();
                }
            }
        }
    }
}
