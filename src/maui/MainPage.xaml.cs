using Microsoft.Maui.Layouts;
using System.Diagnostics;
using System.Globalization;

namespace lols;

public partial class MainPage : ContentPage
{
	 const int Max = 500;
	static int count = 0;
	static readonly System.Timers.Timer timer = new System.Timers.Timer(500);
	static readonly Stopwatch stopwatch = new Stopwatch();

	public MainPage()
	{
		InitializeComponent();

        timer.Elapsed += OnTimer;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(TimeSpan.FromSeconds(3));

        stopwatch.Start();
        timer.Start();

		StartOriginal();
    }

    void StartOriginal()
    {
        _ = Task.Factory.StartNew(RunTest, TaskCreationOptions.LongRunning);
    }

    void StartT1()
	{
        _ = Task.Factory.StartNew(RunTestNew, TaskCreationOptions.LongRunning);
    }

    void StartT2()
    {
        new MyLabel(absolute);
    }

    void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
	{
		double avg = count / stopwatch.Elapsed.TotalSeconds;
		string text = "LOL/s: " + avg.ToString("0.00", CultureInfo.InvariantCulture);
		Dispatcher.Dispatch(() => UpdateText(text));
	}

    void UpdateText(string text) => lols.Text = text;


    void RunTest()
    {
        var random = Random.Shared;

        while (count < 5000)
        {
            var label = new Label
            {
                Text = "lol?",
                TextColor = new Color(random.NextSingle(), random.NextSingle(), random.NextSingle()),
                Rotation = random.NextDouble() * 360
            };
            AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(label, new Rect(random.NextDouble(), random.NextDouble(), 80, 24));
            Dispatcher.Dispatch(() =>
            {
                if (absolute.Children.Count >= Max)
                    absolute.Children.RemoveAt(0);
                absolute.Children.Add(label);
                count++;
            });
            //NOTE: plain Android we could put 1
            Thread.Sleep(2);
        }

        stopwatch.Stop();
        timer.Stop();
    }



    void RunTestNew()
	{
		var random = Random.Shared;

		while (count < 5000)
		{
			var label = new Label
			{
				Text = "lol?",
				TextColor = new Color(random.NextSingle(), random.NextSingle(), random.NextSingle()),
				Rotation = random.NextDouble() * 360
			};
			AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(label, new Rect(random.NextDouble(), random.NextDouble(), 80, 24));
			AddToUI(label);
			//NOTE: plain Android we could put 1
			Thread.Sleep(1);
		}

		stopwatch.Stop();
		timer.Stop();
	}


    

    public async void AddToUI(Label label)
	{
		TaskCompletionSource<bool> _waitToFinish = new TaskCompletionSource<bool>();
#if ANDROID
        var nativeLayout = absolute.Handler.PlatformView as Android.Views.ViewGroup;
        nativeLayout.Post(() =>
#else
			 Dispatcher.Dispatch(() =>
#endif
        {
            if (absolute.Children.Count >= Max)
                absolute.Children.RemoveAt(0);
            absolute.Children.Add(label);
            count++;
			_waitToFinish.TrySetResult(true);
        });
		await _waitToFinish.Task;
    }


	public class MyLabel:Label
	{
		AbsoluteLayout _layout;
        public MyLabel(AbsoluteLayout layout):base()
		{
			_layout = layout;
			Text = "lol?";
			TextColor = new Color(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle());
			Rotation = Random.Shared.NextDouble() * 360;

            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(this, new Rect(Random.Shared.NextDouble(), Random.Shared.NextDouble(), 80, 24));


            if (layout.Children.Count >= Max)
                layout.Children.RemoveAt(0);
            layout.Children.Add(this);
            count++;
#if ANDROID
            var nativeLayout = _layout.Handler.PlatformView as Android.Views.ViewGroup;
            nativeLayout.Post(() => new MyLabel(_layout));
#endif
        }
    }
}

