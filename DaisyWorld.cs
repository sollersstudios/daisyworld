using System;
using System.IO;
using Godot;

public partial class DaisyWorld : Node2D
{
    [Export]
    public int width = 200;

    [Export]
    public int height = 200;

    private TileMapLayer tilemap;

    // Constants
    private const double optimalTemperature = 25;
    private const double randomGrowthThreshold = 0.1; // 10%

    // Assumme world temp goes from 0 - 50
    // Black tulips prefer colder temp, while whites prefer hotter
    private double curWorldTemp = 15;

    // -1 is white
    // +1 is black
    // 0 is barren ground
    int[,] worldData;

    private RandomNumberGenerator rng;

    // Tilemap positions
    Vector2I blackTulip = new Vector2I(0, 0);
    Vector2I whiteTulip = new Vector2I(1, 1);
    Vector2I barrenGround = new Vector2I(0, 1);

    // Time management
    double tickTime = 0.1;
    double remainingTick = 0;

    // UI References
    Label tempLabel;
    Label whitePercentageLabel;
    Label blackPercentageLabel;

    // Data output
    string filePath = "world stats.csv";
    StreamWriter writer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (!File.Exists(filePath))
        {
            using (FileStream fs = File.Create(filePath))
            {
                Console.WriteLine($"New file created: {filePath}");
            }
        }
        writer = new StreamWriter(filePath);
        remainingTick = tickTime;
        worldData = new int[width, height];

        rng = new RandomNumberGenerator();
        rng.Seed = Time.GetTicksMsec(); // Set the seed for better randomness

        GD.Print("Hello World !");
        tilemap = GetNode<TileMapLayer>("TerrainMap"); // Assuming reference by name
        tempLabel = GetNode<Label>("CanvasLayer/LabelGrid/TempLabel");
        whitePercentageLabel = GetNode<Label>("CanvasLayer/LabelGrid/WhitePercent");
        blackPercentageLabel = GetNode<Label>("CanvasLayer/LabelGrid/BlackPercent");

        playTick();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        remainingTick -= delta;
        if (remainingTick < 0)
        {
            remainingTick += tickTime;
            playTick();
        }
    }

    private void playTick()
    {
        tempLabel.Text = String.Format($"temp {curWorldTemp:F3}");
        updateWorldData();
        updateWorldTiles();
    }

    private void updateWorldTiles()
    {
        double tempChange = 0;
        int whiteCount = 0;
        int blackCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2I pos = new Vector2I(x, y);

                switch (worldData[x, y])
                {
                    case -1:
                        tilemap.SetCell(pos, 0, whiteTulip, 0);
                        whiteCount++;
                        tempChange -= 1;
                        break;
                    case 1:
                        tilemap.SetCell(pos, 0, blackTulip, 0);
                        blackCount++;
                        tempChange += 1;
                        break;
                    default:
                        tilemap.SetCell(pos, 0, barrenGround, 0);
                        break;
                }
            }
        }

        double whitePercentage = (double)(whiteCount * 100) / (width * height);
        double blackPercentage = (double)(blackCount * 100) / (width * height);
        whitePercentageLabel.Text = String.Format($"White {whitePercentage:F3}%");
        blackPercentageLabel.Text = String.Format($"Black {blackPercentage:F3}%");

        tempChange = tempChange / (width * height);
        curWorldTemp += tempChange;
        writer.WriteLine($"{whitePercentage},{blackPercentage},{curWorldTemp}");
    }

    private void updateWorldData()
    {
        double deathRate = CalculateDaisyDeathRate();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // If tile dead, randomly decide to grow a tulip 10% of the time
                if (worldData[x, y] == 0 && rng.Randf() < randomGrowthThreshold)
                {
                    worldData[x, y] = blackOrWhite();
                }
                else if (worldData[x, y] != 0 && rng.Randf() < deathRate)
                {
                    worldData[x, y] = 0;
                }
            }
        }
    }

    // Generate a black or white tulip based on world temp
    private int blackOrWhite()
    {
        return rng.Randf() > curWorldTemp / 50 ? 1 : -1;
    }

    private double CalculateDaisyDeathRate()
    {
        // Adjust these parameters to fine-tune the death rate curve
        double temperatureTolerance = 15.0; // How much temperature can deviate before death rate increases significantly
        double maxDeathRate = 0.5; // Maximum possible death rate

        // Calculate the temperature deviation from the optimum
        double deviation = Math.Abs(curWorldTemp - optimalTemperature);

        // Calculate the death rate based on the deviation
        double deathRate = maxDeathRate * (deviation / temperatureTolerance);

        // Ensure the death rate doesn't exceed the maximum
        deathRate = Math.Min(deathRate, maxDeathRate);

        return deathRate;
    }

    public void increaseTemp()
    {
        curWorldTemp += 1;
    }

    public void decreaseTemp()
    {
        curWorldTemp -= 1;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GD.Print("Exiting script");

            GetTree().Quit(); // default behavior
        }
    }
}
