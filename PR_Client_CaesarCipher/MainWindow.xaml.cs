using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Text;

namespace PR_Client_CaesarCipher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {    
        private Client client;
        public MainWindow()
        {
            InitializeComponent();
            client = new Client();
        }

        // Зашифровка
        private void buttonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Количество символов-цифр в введеном shift
                char[] arr = textBoxShift.Text
                    .Where(x => Char.IsDigit(x))
                    .Select(x => x).ToArray();

                if (textBoxShift.Text != string.Empty && textBoxShift.Text.Length == arr.Length &&
                    (int.Parse(textBoxShift.Text) >= 0 && int.Parse(textBoxShift.Text) <= 26))
                {
                    //Запоминаю ROT
                    client.shiftToServer = int.Parse(textBoxShift.Text);

                    // Разделители данных
                    string data = "true" + ' ' + client.shiftToServer + '_';

                    StringBuilder tmpData = new StringBuilder();
                    tmpData.Append(new TextRange(richTextBoxInput.Document.ContentStart, richTextBoxInput.Document.ContentEnd).Text);

                    // Убираю '_'
                    List<int> indexes = new List<int>();
                    for (int i = 0; i < tmpData.Length; i++)
                        if (tmpData[i] == '_')
                            tmpData[i] = ' ';

                    data += tmpData.ToString();
                    client.encryptedData = client.SendAndReceiveDataWithServer(data);

                    // Добавляю текст в richtextbox
                    FlowDocument myFlowDoc = new FlowDocument();
                    myFlowDoc.Blocks.Add(new Paragraph(new Run(client.encryptedData)));
                    richTextBoxResult.Document = myFlowDoc;
                }
            }
            catch
            {
                // Сообщение об ошибке
                MessageBox.Show("Ooops, something went wrong :) Open server or enter english letters.");
            }
        }

        // Расшифровка
        private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Количество символов-цифр в введеном shift
                char[] arr = textBoxShift.Text
                    .Where(x => Char.IsDigit(x))
                    .Select(x => x).ToArray();

                if (textBoxShift.Text != string.Empty && textBoxShift.Text.Length == arr.Length &&
                    (int.Parse(textBoxShift.Text) >= 0 && int.Parse(textBoxShift.Text) <= 26))
                {
                    //Запоминаю ROT
                    client.shiftToServer = int.Parse(textBoxShift.Text);

                    // Разделители данных
                    string data = "false" + ' ' + client.shiftToServer + '_';

                    StringBuilder tmpData = new StringBuilder();
                    tmpData.Append(new TextRange(richTextBoxInput.Document.ContentStart, richTextBoxInput.Document.ContentEnd).Text);

                    // Убираю '_'
                    List<int> indexes = new List<int>();
                    for (int i = 0; i < tmpData.Length; i++)
                        if (tmpData[i] == '_')
                            tmpData[i] = ' ';

                    data += tmpData.ToString();
                    client.decryptedData = client.SendAndReceiveDataWithServer(data);

                    // Добавляю текст в richtextbox
                    FlowDocument myFlowDoc = new FlowDocument();
                    myFlowDoc.Blocks.Add(new Paragraph(new Run(client.decryptedData)));
                    richTextBoxResult.Document = myFlowDoc;
                }
            }
            catch
            {
                MessageBox.Show("Ooops, something went wrong :) Open server or enter english letters.");
            }
        }

        // Отправка серверу на анализ вхождений символов текста
        private void richTextBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                char[] checkContent = new TextRange(richTextBoxInput.Document.ContentStart, richTextBoxInput.Document.ContentEnd).Text.ToCharArray();
                var entities = checkContent.GroupBy(c => c)
                    .OrderByDescending(g => g.Count())
                    .Select(t => t.Key)
                    .Where(l => char.IsLetterOrDigit(l)).ToList();

                if (entities.Count != 0)
                {
                    // Разделители данных
                    string data = "null" + ' ' + -1111 + '_';

                    StringBuilder tmpData = new StringBuilder();
                    tmpData.Append(new TextRange(richTextBoxInput.Document.ContentStart, richTextBoxInput.Document.ContentEnd).Text);

                    // Убираю '_'
                    List<int> indexes = new List<int>();
                    for (int i = 0; i < tmpData.Length; i++)
                        if (tmpData[i] == '_' || tmpData[i] == '~')
                            tmpData[i] = ' ';

                    data += tmpData.ToString();

                    data = client.SendAndReceiveDataWithServer(data);
                    // Создаю строителя диаграммы
                    DiagramBuilder diagramBuilder = new DiagramBuilder(new List<Letter>(), data);

                    // Построение диаграммы
                    if (diagramBuilder.LetterCollection.First().Frequency >= 0)
                        DrawRectangles(diagramBuilder.LetterCollection);

                    // Угадываение ключа
                    FlowDocument myFlowDoc = new FlowDocument();
                    string key = data.Split('~')[1];
                    if (key.Equals("NOT"))
                    {
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("This text is not encrypted!")));
                    }
                    else
                    {
                        myFlowDoc.Blocks.Add(new Paragraph(new Run("If this text is tiny, key (shift) will be wrong. For best results enter big or giant text. On the other hand, it will be good. Key = " + key + ". Is it true? :)")));
                    }
                    richTextBoxKeySuppose.Document = myFlowDoc;

                }
                else
                    ClearDiagramCanvasAndSupposeBox();
            }
            catch
            {
                MessageBox.Show("Ooops, something went wrong :) Open server or enter english letters.");
            }
        }

        private void DrawRectangles(List<Letter> diagramData)
        {
            ClearDiagramCanvasAndSupposeBox();
            int thickness = 30;
            foreach (Letter letter in diagramData)
            {
                Rectangle myRect = new Rectangle();
                myRect.Stroke = Brushes.OrangeRed;
                myRect.Fill = Brushes.SkyBlue;
                myRect.HorizontalAlignment = HorizontalAlignment.Left;
                myRect.VerticalAlignment = VerticalAlignment.Center;

                myRect.Height = (diagramCanvas.ActualHeight - imageTitleDiagram.ActualHeight - 35) * letter.Frequency; // Определение высоты столбца

                myRect.Width = labelA.ActualWidth;

                Canvas.SetLeft(myRect, thickness); // Отступ слева
                Canvas.SetBottom(myRect, 30); // Отступ от нижнего края
                thickness += 50;

                diagramCanvas.Children.Add(myRect);

                // Добавить частоту над Rect
                TextBlock myTextBlock = new TextBlock();
                myTextBlock.Text = Math.Round(letter.Frequency, 3).ToString();
                myTextBlock.FontFamily = new FontFamily("Verdana");
                myTextBlock.FontSize = 15;
                Canvas.SetLeft(myTextBlock, Canvas.GetLeft(myRect));
                Canvas.SetBottom(myTextBlock, myRect.Height + 40);
                diagramCanvas.Children.Add(myTextBlock);
            }
        }

        // Удаление прошлой диаграммы
        private void ClearDiagramCanvasAndSupposeBox()
        {
            // Очистка поля для угадывания ключа
            richTextBoxKeySuppose.Document.Blocks.Clear();
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < diagramCanvas.Children.Count; i++)
                {
                    UIElement ui = diagramCanvas.Children[i];
                    if (ui is Rectangle)
                    {
                        diagramCanvas.Children.Remove(ui as Rectangle);
                        flag = true;
                        break;
                    }
                    else if (ui is TextBlock)
                    {
                        diagramCanvas.Children.Remove(ui as TextBlock);
                        flag = true;
                        break;
                    }
                }
            }
        }
    }
}