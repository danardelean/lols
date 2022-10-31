using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Globalization;
using System.Threading.Tasks;
using Uno.UI;
using System.Diagnostics;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace lols;


/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    const int Max = 500;
    static int count = 0;
    static readonly System.Timers.Timer timer = new System.Timers.Timer(500);
    static readonly Stopwatch stopwatch = new Stopwatch();

    public MainPage()
    {
        this.InitializeComponent();
        timer.Elapsed += OnTimer;
    }

    void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
    {
        double avg = count / stopwatch.Elapsed.TotalSeconds;
        string text = "LOL/s: " + avg.ToString("0.00", CultureInfo.InvariantCulture);
        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High,async()=> UpdateText(text));
    }

    void UpdateText(string text) => Lols.Text = text;


    protected override async void OnNativeLoaded()
    {
        base.OnNativeLoaded();

        await Task.Delay(TimeSpan.FromSeconds(3));

        stopwatch.Start();
        timer.Start();

        StartT1();
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
        double width = Picture.ActualWidth;
        double height = Picture.ActualHeight;

        //TODO: something better?
        while (width == 0 || height == 0)
        {
            width = Picture.ActualWidth;
            height = Picture.ActualHeight;
        }

        while (count < 5000)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () => 
            {
                Byte[] c = new Byte[3];
                Random.Shared.NextBytes(c);
                var control = new TextBlock
                {
                    Text = "lol?",
                    Foreground = new SolidColorBrush(new Windows.UI.Color { A = 255, R = c[0], G = c[1], B = c[2] }),
                    RenderTransform = new RotateTransform { Angle = Random.Shared.NextDouble() * 360 },
                    Height = 24,
                    Width = 80,
                };

                Canvas.SetLeft(control, Random.Shared.NextDouble() * Picture.ActualWidth);
                Canvas.SetTop(control, Random.Shared.NextDouble() * Picture.ActualHeight);

                if (Picture.Children.Count >= Max)
                    Picture.Children.RemoveAt(0);

                Picture.Children.Add(control);
                count++;
            });
            Thread.Sleep(1);
        }
        stopwatch.Stop();
        timer.Stop();
    }


    public partial class MyTextBlock : TextBlock
    {
        private Canvas _canvas;

        public MyTextBlock(Canvas canvas) : base()
        {
            _canvas = canvas;
            Byte[] c = new Byte[3];
            Random.Shared.NextBytes(c);

            Text = "lol?";
            Foreground = new SolidColorBrush(new Windows.UI.Color { A = 255, R = c[0], G = c[1], B = c[2] });
            RenderTransform = new RotateTransform { Angle = Random.Shared.NextDouble() * 360 };
            Height = 24;
            Width = 80;

            Canvas.SetLeft(this, Random.Shared.NextDouble() * canvas.ActualWidth);
            Canvas.SetTop(this, Random.Shared.NextDouble() * canvas.ActualHeight);


            if (canvas.Children.Count >= Max)
                canvas.Children.RemoveAt(0);

            canvas.Children.Add(this);
            count++;
        }

        protected override void OnNativeLoaded()
        {
            base.OnNativeLoaded();
            if (!loaded)
                loaded = true;
            if (count < 5000)
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () => new MyTextBlock(_canvas));
            else
            {
                stopwatch.Stop();
                timer.Stop();
            }
        }

        private bool loaded = false;

    }
}

