using System;
using Avalonia.Controls;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace lols.Views
{
    public partial class MainView : UserControl
    {
        const int Max = 500;
        static int count = 0;
        static readonly System.Timers.Timer timer = new System.Timers.Timer(500);
        static readonly Stopwatch stopwatch = new Stopwatch();

        public MainView()
        {
            InitializeComponent();
            timer.Elapsed += OnTimer;
        }

        void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            double avg = count / stopwatch.Elapsed.TotalSeconds;
            string text = "LOL/s: " + avg.ToString("0.00", CultureInfo.InvariantCulture);
            Dispatcher.UIThread.Post(() => UpdateText(text), DispatcherPriority.Render);
        }

        void UpdateText(string text)=>Lols.Text = text;




        private async void Picture_OnLoaded(object? sender, RoutedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            stopwatch.Start();
            timer.Start();

          StartT2();
        }

        void StartT1()
        {
            _ = Task.Factory.StartNew(RunTest, TaskCreationOptions.LongRunning);
        }

        void StartT2()
        {
            new MyTextBlock(Picture);
        }

        void RunTest()
        {
            var random = Random.Shared;
            double width = Picture.Bounds.Width;
            double height = Picture.Bounds.Height;

            //TODO: something better?
            while (width == 0 || height == 0)
            {
                width = Picture.Bounds.Width;
                height = Picture.Bounds.Height;
            }

            while (count < 5000)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Byte[] c = new Byte[3];
                    Random.Shared.NextBytes(c);
                    var control = new TextBlock
                    {
                        Text = "lol?",
                        Foreground = new Avalonia.Media.SolidColorBrush(new Color(255, c[0], c[1], c[2])),
                        RenderTransform = new RotateTransform(Random.Shared.NextDouble() * 360),
                        Height = 24,
                        Width = 80,
                    };
                 
                    Canvas.SetLeft(control, Random.Shared.NextDouble() * Picture.Bounds.Width);
                    Canvas.SetTop(control, Random.Shared.NextDouble() * Picture.Bounds.Height);
                    
                    if (Picture.Children.Count >= Max)
                        Picture.Children.RemoveAt(0);

                    Picture.Children.Add(control);
                    count++;
                });
                //Thread.Sleep(1);
            }

            stopwatch.Stop();
            timer.Stop();
        }

    public class MyTextBlock : TextBlock
        {
            private Canvas _canvas;

            public MyTextBlock(Canvas canvas) : base()
            {
                _canvas = canvas;
                Byte[] c = new Byte[3];
                Random.Shared.NextBytes(c);

                Text = "lol?";
                Foreground = new Avalonia.Media.SolidColorBrush(new Color(255, c[0], c[1], c[2]));
                RenderTransform = new RotateTransform(Random.Shared.NextDouble() * 360);
                Height = 24;
                Width = 80;

                canvas.BeginBatchUpdate();
                Canvas.SetLeft(this, Random.Shared.NextDouble() * canvas.Bounds.Width);
                Canvas.SetTop(this, Random.Shared.NextDouble() * canvas.Bounds.Height);

      
                if (canvas.Children.Count >= Max)
                    canvas.Children.RemoveAt(0);

                canvas.Children.Add(this);
                canvas.EndBatchUpdate();
                count++;
            }

            private bool loaded = false;
            protected override void OnLoaded()
            {
                base.OnLoaded();
                if (!loaded)
                    loaded = true;
                if (count < 5000)
                    new MyTextBlock(_canvas);
                else
                {
                    stopwatch.Stop();
                    timer.Stop();
                }
            }
        }
    }

    
}