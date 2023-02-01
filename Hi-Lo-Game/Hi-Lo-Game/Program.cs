int min = 1;
int max = 100;

int minPlayers = 0;
int maxPlayers = 5;

Console.WriteLine("Hi-Lo Game - Multiplayer");
Console.WriteLine("========================");

Console.WriteLine($"Enter number of players between {minPlayers} and {maxPlayers}:");

string input = Console.ReadLine();

int? numberOfPlayers = TryNumberOfPlayers(input);

while (!numberOfPlayers.HasValue)
{
    Console.WriteLine($"!!!Write a valid number of players - between {minPlayers} and {maxPlayers}!!!");
    input = Console.ReadLine();
    numberOfPlayers = TryNumberOfPlayers(input);
}

Game game = new Game(min, max, numberOfPlayers.Value);
game.Start();

int? TryNumberOfPlayers(string input)
{
    int numberOfPlayers;
    if (int.TryParse(input, out numberOfPlayers) && numberOfPlayers <= maxPlayers && numberOfPlayers >= minPlayers)
        return numberOfPlayers;
    return null;
}

class Game
{
    private readonly int _min;
    private readonly int _max;
    private readonly List<PlayerService> _players;

    public Game(int min, int max, int numberOfPlayers)
    {
        _min = min;
        _max = max;
        _players = Enumerable
            .Range(1, numberOfPlayers)
            .Select(x => PlayerService.NewPlayer(x))
            .ToList();
    }

    public List<PlayerService> Players => _players;

    public void Start()
    {
        foreach (var player in _players)
        {
            var mysteryNumber = new Random().Next(_min, _max + 1);

            player.SetMysteryNumber(mysteryNumber);
            Console.WriteLine($"\n{player.PlayerName}'s turn to guess the mystery number");
            Play(player);
            Console.WriteLine($"Congratulations {player.PlayerName}! You have guessed the mystery number in {player.PlayerTurns} turns.");
        }

        Console.WriteLine("\nSummary of player statistics:");

        _players.ForEach(player => Console.WriteLine(player.PlayerInfo));

        Console.WriteLine("\nWinner(s):");
        _players
            .GroupBy(x => x.PlayerTurns)
            .OrderBy(x => x.Key)
            .First()
            .ToList()
            .ForEach(player => Console.WriteLine($"Winner: {player.PlayerInfo}"));

        Console.WriteLine("\nDo you want to play again? (Y -> yes)");
        string playAgain = Console.ReadLine();
        if (playAgain.ToLower() == "y")
        {
            Start();
        }
    }

    private void Play(PlayerService player)
    {
        int guess = -1;
        var turns = 0;

        while (guess != player.MysteryNumber)
        {
            Console.WriteLine($"Enter your guess between {_min} and {_max}:");

            string input = Console.ReadLine();
            if (!int.TryParse(input, out guess) || guess < _min || guess > _max)
            {
                Console.WriteLine($"!!!Your guess MUST be between {_min} and {_max}!!!");
                continue;
            }

            if (guess > player.MysteryNumber)
            {
                Console.WriteLine("LO");
            }
            else if (guess < player.MysteryNumber)
            {
                Console.WriteLine("HI");
            }

            turns++;
        }

        player.SetTurns(turns);
    }
}

class PlayerService
{
    private readonly PlayerInfo _playerInfo;

    private PlayerService(string name)
    {
        _playerInfo = new PlayerInfo(name);
    }

    public static PlayerService NewPlayer(int playerNumber)
    {
        return new PlayerService($"Player_{playerNumber}");
    }

    public int MysteryNumber => _playerInfo.MysteryNumber;

    public void SetTurns(int turns)
    {
        _playerInfo.Turns = turns;
    }

    public void SetMysteryNumber(int mysteryNumber)
    {
        _playerInfo.MysteryNumber = mysteryNumber;
    }

    public string PlayerInfo => $"{_playerInfo.Name}: {_playerInfo.Turns} turns";
    public string PlayerName => _playerInfo.Name;
    public int PlayerTurns => _playerInfo.Turns;
}

class PlayerInfo
{
    public string Name { get; private set; }
    public int Turns { get; set; }
    public int MysteryNumber { get; set; }

    public PlayerInfo(string name)
    {
        Name = name;
    }
}


