using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameInterface
{
    public interface IPirateGame
    {
        List<Pirate> AllEnemyPirates();

        List<Pirate> AllMyPirates();

        void Attack(Pirate pirate, Pirate target);

        void Debug(string message);

        void Debug(string format, params object[] messages);

        void Defend(Pirate pirate);

        Location Destination(Pirate pirate, List<Direction> directions);

        Location Destination(Location loc, List<Direction> directions);

        int Distance(Location loc1, Location loc2);

        int Distance(Pirate pirate, Location loc);

        int Distance(Pirate pirate1, Pirate pirate2);

        int Distance(Pirate pirate, Treasure treasure);

        List<Pirate> EnemyDrunkPirates();

        List<Pirate> EnemyLostPirates();

        List<Pirate> EnemyPirates();

        List<Pirate> EnemyPiratesWithoutTreasures();

        List<Pirate> EnemyPiratesWithTreasures();

        List<Pirate> EnemySoberPirates();

        int GetActionsPerTurn();

        int GetAttackRadius();

        int GetCols();

        int GetDefenseExpirationTurns();

        Pirate GetEnemyPirate(int id);

        int GetEnemyScore();

        int GetMaxPoints();

        int GetMaxTurns();

        Pirate GetMyPirate(int id);

        int GetMyScore();

        string GetOpponentName();

        Pirate GetPirateOn(Location loc);

        int GetReloadTurns();

        int GetRows();

        List<Location> GetSailOptions(Pirate pirate, Location destination, int moves);

        List<Location> GetSailOptions(Pirate pirate1, Pirate pirate2, int moves);

        List<Location> GetSailOptions(Pirate pirate, Treasure treasure, int moves);

        int GetSoberTurns();

        int GetSpawnTurns();

        int GetTurn();

        bool InRange(Pirate pirate, Location loc);

        bool InRange(Location loc1, Location loc2);

        bool InRange(Pirate pirate1, Pirate pirate2);

        bool IsOccupied(Location loc);

        List<Pirate> MyDrunkPirates();

        List<Pirate> MyLostPirates();

        List<Pirate> MyPirates();

        List<Pirate> MyPiratesWithoutTreasures();

        List<Pirate> MyPiratesWithTreasures();

        List<Pirate> MySoberPirates();

        void SetSail(Pirate pirate, Location destination);

        void StopPoint(string message);

        int TimeRemaining();

        List<Treasure> Treasures();
    }

    public interface IPirateBot
    {
        void DoTurn(IPirateGame state);
    }

    public static class Consts
    {
        public const int ME = 0;

        public const int NO_OWNER = -1;

        public const int ENEMY = 1;
    }

    public static class InnerConsts
    {
        public const int PIRATES = 0;

        public const int LOST = -1;

        public const int WATER = -2;

        public const int ZONE = -4;

        public const char LAND_IN_PARSE = 'l';

        public const char WATER_IN_PARSE = 'w';

        public const string PLAYER_PIRATE = "abcdefghij";

        public const string MAP_OBJECT = "?%*.!";

        public const string MAP_RENDER = "abcdefghij?%*.!";

        public readonly static Dictionary<Direction, Location> AIM;

        static InnerConsts()
        {
            InnerConsts.AIM = new Dictionary<Direction, Location>()
            {
                { Direction.NORTH, new Location(-1, 0) },
                { Direction.EAST, new Location(0, 1) },
                { Direction.SOUTH, new Location(1, 0) },
                { Direction.WEST, new Location(0, -1) },
                { Direction.NOTHING, new Location(0, 0) }
            };
        }
    }

    public enum Direction
    {
        NOTHING = 45,
        EAST = 101,
        NORTH = 110,
        SOUTH = 115,
        WEST = 119
    }

    public class Location : IEquatable<Location>
    {
        public int Col
        {
            get;
            set;
        }

        public int Row
        {
            get;
            set;
        }

        public Location(int Row, int Col)
        {
            this.Row = Row;
            this.Col = Col;
        }

        public Location(Location OtherLocation)
        {
            this.Row = OtherLocation.Row;
            this.Col = OtherLocation.Col;
        }

        public bool Equals(Location other)
        {
            if (other == null || this.Row != other.Row)
            {
                return false;
            }
            return this.Col == other.Col;
        }

        public override int GetHashCode()
        {
            return this.Col * 100 + this.Row;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.Row, this.Col);
        }
    }

    public class Move
    {
        public string Value
        {
            get;
            set;
        }

        public Move()
        {
        }
    }

    public class MoveAttack : Move
    {
        public MoveAttack(int targetID)
        {
            base.Value = string.Concat("a", targetID.ToString());
        }
    }

    public class MoveDefense : Move
    {
        public MoveDefense()
        {
            base.Value = "d";
        }
    }

    public class MoveDirection : Move
    {
        public MoveDirection(Direction direction)
        {
            base.Value = ((char)direction).ToString();
        }
    }

    public class Order
    {
        public List<Move> Moves;

        public object[] Arguments;

        public Order(params object[] arguments)
        {
            this.Moves = new List<Move>();
            this.Arguments = arguments;
        }

        public override string ToString()
        {
            string empty = string.Empty;
            foreach (Move move in this.Moves)
            {
                empty = string.Concat(empty, move.Value);
            }
            if (this.Arguments.Length != 0)
            {
                empty = string.Concat(empty, " ", string.Join(" ", this.Arguments));
            }
            return empty;
        }
    }

    public class Pirate : IEquatable<Pirate>
    {
        public int DefenseExpirationTurns
        {
            get;
            set;
        }

        public bool HasTreasure
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public Location InitialLocation
        {
            get;
            set;
        }

        public bool IsLost
        {
            get;
            set;
        }

        public Location Location
        {
            get;
            set;
        }

        public int Owner
        {
            get;
            set;
        }

        public int ReloadTurns
        {
            get;
            set;
        }

        public int TurnsToRevive
        {
            get;
            set;
        }

        public int TurnsToSober
        {
            get;
            set;
        }

        public Pirate(int Id, int Owner, Location Location, Location InitialLocation)
        {
            this.Id = Id;
            this.Owner = Owner;
            this.Location = Location;
            this.InitialLocation = InitialLocation;
            this.IsLost = false;
            this.TurnsToRevive = 0;
            this.ReloadTurns = 0;
            this.DefenseExpirationTurns = 0;
            this.TurnsToSober = 0;
            this.HasTreasure = false;
        }

        public bool Equals(Pirate other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return this.Id * 10 + this.Owner;
        }

        public override string ToString()
        {
            return string.Format("<Pirate ID:{0} OWNER:{1} LOC:({2}, {3})>", new object[] { this.Id, this.Owner, this.Location.Row, this.Location.Col });
        }
    }

    public class TeammateAttackException : Exception
    {
        public TeammateAttackException(string message) : base(message)
        {
        }
    }

    public class Treasure : IEquatable<Treasure>
    {
        public int Id
        {
            get;
            set;
        }

        public Location Location
        {
            get;
            set;
        }

        public Treasure(int Id, Location Loc)
        {
            this.Id = Id;
            this.Location = Loc;
        }

        public bool Equals(Treasure other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override string ToString()
        {
            return string.Format("<Treasure ID:{0} LOC:({2}, {3})>", this.Id, this.Location.Row, this.Location.Col);
        }
    }

    public class PirateGame : IPirateGame
    {
        private List<int> Scores;

        private List<int> CloakCooldowns;

        private List<int> LastTurnPoints;

        private int ActionsPerTurn
        {
            get;
            set;
        }

        private List<Pirate> AllPirates
        {
            get;
            set;
        }

        private List<Treasure> AllTreasures
        {
            get;
            set;
        }

        private int AttackRadius2
        {
            get;
            set;
        }

        private List<string> BotNames
        {
            get;
            set;
        }

        private int Cols
        {
            get;
            set;
        }

        private bool Cyclic
        {
            get;
            set;
        }

        private int DefenseExpirationTurns
        {
            get;
            set;
        }

        private int ENEMY
        {
            get;
            set;
        }

        private int LoadTime
        {
            get;
            set;
        }

        private IDictionary<Location, Pirate> Loc2Pirate
        {
            get;
            set;
        }

        private int[,] Map
        {
            get;
            set;
        }

        private int MaxPoints
        {
            get;
            set;
        }

        private int MaxTurns
        {
            get;
            set;
        }

        private int ME
        {
            get;
            set;
        }

        private int NEUTRAL
        {
            get;
            set;
        }

        private int NumPlayers
        {
            get;
            set;
        }

        private IDictionary<Location, Order> Orders
        {
            get;
            set;
        }

        private Random Random
        {
            get;
            set;
        }

        private bool recover_errors
        {
            get;
            set;
        }

        private int ReloadTurns
        {
            get;
            set;
        }

        private int Rows
        {
            get;
            set;
        }

        private int SoberTurns
        {
            get;
            set;
        }

        private List<Pirate> SortedEnemyPirates
        {
            get;
            set;
        }

        private List<Pirate> SortedMyPirates
        {
            get;
            set;
        }

        private int SpawnRadius2
        {
            get;
            set;
        }

        private int SpawnTurns
        {
            get;
            set;
        }

        private int Turn
        {
            get;
            set;
        }

        private DateTime TurnStartTime
        {
            get;
            set;
        }

        private int TurnTime
        {
            get;
            set;
        }

        private int ViewRadius2
        {
            get;
            set;
        }

        private bool[,] Vision
        {
            get;
            set;
        }

        private List<Location> VisionOffsets2
        {
            get;
            set;
        }

        public PirateGame(string gameData)
        {
            this.Cols = 0;
            this.Rows = 0;
            this.Map = new int[0, 0];
            this.AllTreasures = new List<Treasure>();
            this.AllPirates = new List<Pirate>();
            this.Scores = new List<int>();
            this.CloakCooldowns = new List<int>();
            this.LastTurnPoints = new List<int>();
            this.TurnTime = 0;
            this.LoadTime = 0;
            this.TurnStartTime = DateTime.Now;
            this.NumPlayers = 0;
            this.Vision = null;
            this.VisionOffsets2 = null;
            this.ViewRadius2 = 0;
            this.AttackRadius2 = 0;
            this.SpawnRadius2 = 0;
            this.ActionsPerTurn = 0;
            this.ReloadTurns = 0;
            this.DefenseExpirationTurns = 0;
            this.SpawnTurns = 0;
            this.SoberTurns = 0;
            this.MaxPoints = 0;
            this.MaxTurns = 0;
            this.Turn = 0;
            this.recover_errors = true;
            this.Cyclic = true;
            this.Orders = new Dictionary<Location, Order>();
            this.Loc2Pirate = new Dictionary<Location, Pirate>();
            this.BotNames = new List<string>();
            this.ME = 0;
            this.ENEMY = 1;
            this.NEUTRAL = -1;
            using (StringReader stringReader = new StringReader(gameData))
            {
                while (true)
                {
                    string str = stringReader.ReadLine();
                    string lower = str;
                    if (str == null)
                    {
                        break;
                    }
                    lower = lower.Trim().ToLower();
                    if (lower.Length != 0)
                    {
                        string[] strArrays = lower.Split(new char[0]);
                        string str1 = strArrays[0];
                        if (strArrays.Length > 1)
                        {
                            int num = int.Parse(strArrays[1]);
                            switch (str1)
                            {
                                case "spawn_turns":
                                    this.SpawnTurns = num;
                                    break;
                                case "turntime":
                                    this.TurnTime = num;
                                    break;
                                case "bot_names":
                                    this.BotNames = new List<string>(strArrays);
                                    this.BotNames.RemoveRange(0, 2);
                                    break;
                                case "numplayers":
                                    this.NumPlayers = num;
                                    break;
                                case "rows":
                                    this.Rows = num;
                                    break;
                                case "max_turns":
                                    this.MaxTurns = num;
                                    break;
                                case "reload_turns":
                                    this.ReloadTurns = num;
                                    break;
                                case "spawnradius2":
                                    this.SpawnRadius2 = num;
                                    break;
                                case "cyclic":
                                    this.Cyclic = num != 0;
                                    break;
                                case "defense_expiration_turns":
                                    this.DefenseExpirationTurns = num;
                                    break;
                                case "loadtime":
                                    this.LoadTime = num;
                                    break;
                                case "attackradius2":
                                    this.AttackRadius2 = num;
                                    break;
                                case "start_turn":
                                    this.Turn = num;
                                    break;
                                case "actions_per_turn":
                                    this.ActionsPerTurn = num;
                                    break;
                                case "recover_errors":
                                    this.recover_errors = num == 1;
                                    break;
                                case "player_seed":
                                    this.Random = new Random(num);
                                    break;
                                case "viewradius2":
                                    this.ViewRadius2 = num;
                                    break;
                                case "maxpoints":
                                    this.MaxPoints = num;
                                    break;
                                case "sober_turns":
                                    this.SoberTurns = num;
                                    break;
                                case "cols":
                                    this.Cols = num;
                                    break;
                            }
                        }
                    }
                }
            }
            this.Map = new int[this.Rows, this.Cols];
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    this.Map[i, j] = -2;
                }
            }
            this.Scores = Enumerable.Repeat(0, this.NumPlayers).ToList();
            this.LastTurnPoints = Enumerable.Repeat(0, this.NumPlayers).ToList();
        }

        public List<Pirate> AllEnemyPirates()
        {
            return new List<Pirate>(this.SortedEnemyPirates);
        }

        public List<Pirate> AllMyPirates()
        {
            return new List<Pirate>(this.SortedMyPirates);
        }

        public void Attack(Pirate pirate, Pirate target)
        {
            if (pirate.Owner == target.Owner)
            {
                throw new TeammateAttackException("Pirate cannot attack a teammate!");
            }
            this.Orders[pirate.Location].Moves.Add(new MoveAttack(target.Id));
        }

        private static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public void CancelCollisions()
        {
            int num = 0;
            for (int i = 0; i < this.MyPirates().Count(); i++)
            {
                List<Tuple<Location, List<Location>>> tuples = this.ValidateCollisions();
                if (tuples.Count() <= 0)
                {
                    break;
                }
                foreach (Tuple<Location, List<Location>> tuple in tuples)
                {
                    Location item1 = tuple.Item1;
                    List<Location> item2 = tuple.Item2;
                    for (int j = 1; j < item2.Count(); j++)
                    {
                        num++;
                        this.CancelOrder(item2[j]);
                    }
                }
            }
            if (num > 0)
            {
                this.Debug("WARNING: was forced to cancel collisions for {0} pirates", new object[] { num });
            }
        }

        public void CancelOrder(Pirate pirate)
        {
            this.CancelOrder(pirate.Location);
        }

        public void CancelOrder(Location location)
        {
            this.Orders.Remove(location);
        }

        public void Debug(string message)
        {
            Console.Out.WriteLine(string.Format("m {0}", PirateGame.Base64Encode(message)));
            Console.Out.Flush();
        }

        public void Debug(string format, params object[] messages)
        {
            this.Debug(string.Format(format, messages));
        }

        public void Defend(Pirate pirate)
        {
            this.Orders[pirate.Location].Moves.Add(new MoveDefense());
        }

        public Location Destination(Pirate pirate, List<Direction> directions)
        {
            return this.Destination(pirate.Location, directions);
        }

        public Location Destination(Location loc, List<Direction> directions)
        {
            Location location = new Location(loc);
            foreach (Direction direction in directions)
            {
                Location item = InnerConsts.AIM[direction];
                if (!this.Cyclic)
                {
                    Location row = location;
                    row.Row = row.Row + item.Row;
                    Location col = location;
                    col.Col = col.Col + item.Col;
                }
                else
                {
                    location.Row = (location.Row + item.Row) % this.Rows;
                    location.Col = (location.Col + item.Col) % this.Cols;
                }
            }
            return location;
        }

        public int Distance(Location loc1, Location loc2)
        {
            int num = Math.Abs(loc1.Row - loc2.Row);
            int num1 = Math.Abs(loc1.Col - loc2.Col);
            if (!this.Cyclic)
            {
                return num + num1;
            }
            return Math.Min(num1, this.Cols - num1) + Math.Min(num, this.Rows - num);
        }

        public int Distance(Pirate pirate, Location loc)
        {
            return this.Distance(pirate.Location, loc);
        }

        public int Distance(Pirate pirate1, Pirate pirate2)
        {
            return this.Distance(pirate1.Location, pirate2.Location);
        }

        public int Distance(Pirate pirate, Treasure treasure)
        {
            return this.Distance(pirate.Location, treasure.Location);
        }

        public List<Pirate> EnemyDrunkPirates()
        {
            List<Pirate> pirates = this.EnemyPirates();
            return pirates.Where(p => p.TurnsToSober > 0).ToList();
        }

        public List<Pirate> EnemyLostPirates()
        {
            List<Pirate> sortedEnemyPirates = this.SortedEnemyPirates;
            return sortedEnemyPirates.Where(pirate => pirate.IsLost).ToList();
        }

        public List<Pirate> EnemyPirates()
        {
            List<Pirate> sortedEnemyPirates = this.SortedEnemyPirates;
            return sortedEnemyPirates.Where(pirate => !pirate.IsLost).ToList();
        }

        public List<Pirate> EnemyPiratesWithoutTreasures()
        {
            List<Pirate> pirates = this.EnemyPirates();
            return pirates.Where(pirate => !pirate.HasTreasure).ToList();
        }

        public List<Pirate> EnemyPiratesWithTreasures()
        {
            List<Pirate> pirates = this.EnemyPirates();
            return pirates.Where(pirate => pirate.HasTreasure).ToList();
        }

        public List<Pirate> EnemySoberPirates()
        {
            List<Pirate> pirates = this.EnemyPirates();
            return pirates.Where(pirate => pirate.TurnsToSober <= 0).ToList();
        }

        private List<Direction> ExtractDirectionsFromMoves(List<Move> moves)
        {
            List<Direction> directions = new List<Direction>();
            foreach (Move move in moves)
            {
                if (!(move is MoveDirection))
                {
                    continue;
                }
                int num = move.Value.First();
                Direction direction = (Direction)Enum.Parse(typeof(Direction), num.ToString());
                directions.Add(direction);
            }
            return directions;
        }

        public void FinishTurn()
        {
            foreach (KeyValuePair<Location, Order> order in this.Orders)
            {
                if (order.Value.Moves.Count() <= 0)
                {
                    continue;
                }
                Console.Out.WriteLine(string.Format("o {0} {1} {2}", order.Key.Row, order.Key.Col, order.Value));
            }
            this.Orders.Clear();
            Console.Out.WriteLine("go");
            Console.Out.Flush();
        }

        public int GetActionsPerTurn()
        {
            return this.ActionsPerTurn;
        }

        public int GetAttackRadius()
        {
            return this.AttackRadius2;
        }

        public int GetCols()
        {
            return this.Cols;
        }

        public int GetDefenseExpirationTurns()
        {
            return this.DefenseExpirationTurns;
        }

        public List<Direction> GetDirections(Location loc1, Location loc2)
        {
            int rows = this.Rows;
            int cols = this.Cols;
            int row = loc1.Row;
            int col = loc1.Col;
            int num5 = loc2.Row;
            int num6 = loc2.Col;
            int num7 = this.Distance(loc1, loc2);
            List<Direction> list = new List<Direction>();
            if (loc1.Equals(loc2))
            {
                list.Add(Direction.NOTHING);
                return list;
            }
            for (int i = 0; i < num7; i++)
            {
                if (row < num5)
                {
                    if (((num5 - row) >= rows) && this.Cyclic)
                    {
                        list.Add(Direction.NORTH);
                        row--;
                        continue;
                    }
                    if (((num5 - row) <= rows) || !this.Cyclic)
                    {
                        list.Add(Direction.SOUTH);
                        row++;
                        continue;
                    }
                }
                if (num5 < row)
                {
                    if (((row - num5) >= rows) && this.Cyclic)
                    {
                        list.Add(Direction.SOUTH);
                        row++;
                        continue;
                    }
                    if (((row - num5) <= rows) || !this.Cyclic)
                    {
                        list.Add(Direction.NORTH);
                        row--;
                        continue;
                    }
                }
                if (col < num6)
                {
                    if (((num6 - col) >= cols) && this.Cyclic)
                    {
                        list.Add(Direction.WEST);
                        col--;
                        continue;
                    }
                    if (((num6 - col) <= cols) || !this.Cyclic)
                    {
                        list.Add(Direction.EAST);
                        col++;
                        continue;
                    }
                }
                if (num6 < col)
                {
                    if (((col - num6) >= cols) && this.Cyclic)
                    {
                        list.Add(Direction.EAST);
                        col++;
                    }
                    else if (((col - num6) <= cols) || !this.Cyclic)
                    {
                        list.Add(Direction.WEST);
                        col--;
                    }
                }
            }
            return list;
        }

        public List<Direction> GetDirections(Pirate pirate, Treasure treasure)
        {
            return this.GetDirections(pirate.Location, treasure.Location);
        }

        public List<Direction> GetDirections(Pirate pirate1, Pirate pirate2)
        {
            return this.GetDirections(pirate1.Location, pirate2.Location);
        }

        public List<Direction> GetDirections(Pirate pirate, Location location)
        {
            return this.GetDirections(pirate.Location, location);
        }

        public Pirate GetEnemyPirate(int id)
        {
            if (id < 0 || id >= this.SortedEnemyPirates.Count)
            {
                return null;
            }
            return this.SortedEnemyPirates[id];
        }

        public int GetEnemyScore()
        {
            return this.Scores[1];
        }

        public List<int> GetLastTurnPoints()
        {
            return this.LastTurnPoints;
        }

        public int GetMaxPoints()
        {
            return this.MaxPoints;
        }

        public int GetMaxTurns()
        {
            return this.MaxTurns;
        }

        public Pirate GetMyPirate(int id)
        {
            if (id < 0 || id >= this.SortedMyPirates.Count)
            {
                return null;
            }
            return this.SortedMyPirates[id];
        }

        public int GetMyScore()
        {
            return this.Scores[0];
        }

        public string GetOpponentName()
        {
            if (this.BotNames == null || this.BotNames.Count <= 1)
            {
                return null;
            }
            return this.BotNames.Last();
        }

        public Pirate GetPirateOn(Location location)
        {
            if (!this.Loc2Pirate.ContainsKey(location))
            {
                return null;
            }
            return this.Loc2Pirate[location];
        }

        public int GetReloadTurns()
        {
            return this.ReloadTurns;
        }

        public int GetRows()
        {
            return this.Rows;
        }

        public List<Location> GetSailOptions(Pirate pirate1, Pirate pirate2, int moves)
        {
            return this.GetSailOptions(pirate1, pirate2.Location, moves);
        }

        public List<Location> GetSailOptions(Pirate pirate, Treasure treasure, int moves)
        {
            return this.GetSailOptions(pirate, treasure.Location, moves);
        }

        public List<Location> GetSailOptions(Pirate pirate, Location destination, int moves)
        {
            int num;
            int num1;
            if (moves < 0)
            {
                throw new ArgumentException("moves must be non negative!");
            }
            List<Location> locations = new List<Location>();
            if (pirate.Location == destination)
            {
                locations.Add(pirate.Location);
                return locations;
            }
            List<Direction> directions = this.GetDirections(pirate.Location, destination);
            List<Direction> directions1 = new List<Direction>();
            foreach (Direction direction in directions)
            {
                if (directions1.Contains(direction))
                {
                    continue;
                }
                directions1.Add(direction);
            }
            int num2 = directions.IndexOf(directions1.Last());
            num = (num2 - moves <= 0 ? 0 : num2 - moves);
            num1 = (num2 + moves >= directions.Count() ? directions.Count() : num2 + moves);
            List<Direction> range = directions.GetRange(num, num1 - num);
            if (range.Count() >= moves)
            {
                for (int i = 0; i < range.Count() - moves + 1; i++)
                {
                    locations.Add(this.Destination(pirate, range.GetRange(i, moves)));
                }
            }
            else
            {
                locations.Add(this.Destination(pirate, range));
            }
            return locations;
        }

        public List<int> GetScores()
        {
            return this.Scores;
        }

        public int GetSoberTurns()
        {
            return this.SoberTurns;
        }

        public int GetSpawnTurns()
        {
            return this.SpawnTurns;
        }

        public int GetTurn()
        {
            return this.Turn;
        }

        public Dictionary<Pirate, List<Direction>> InDanger(Pirate anAnt, List<Pirate> possibleAttackers = null)
        {
            if (possibleAttackers == null)
            {
                possibleAttackers = this.EnemyPirates();
            }
            Dictionary<Pirate, List<Direction>> pirates = new Dictionary<Pirate, List<Direction>>();
            foreach (Pirate possibleAttacker in possibleAttackers)
            {
                List<Direction> directions = new List<Direction>();
                foreach (Direction direction in directions)
                {
                    if (!this.InRange(anAnt, this.Destination(possibleAttacker, directions)))
                    {
                        continue;
                    }
                    directions.Add(direction);
                }
                if (directions.Count <= 0)
                {
                    continue;
                }
                pirates[possibleAttacker] = directions;
            }
            return pirates;
        }

        public bool InRange(Pirate pirate1, Pirate pirate2)
        {
            return this.InRange(pirate1.Location, pirate2.Location);
        }

        public bool InRange(Pirate pirate, Location location)
        {
            return this.InRange(pirate.Location, location);
        }

        public bool InRange(Location loc1, Location loc2)
        {
            int row = loc1.Row - loc2.Row;
            int col = loc1.Col - loc2.Col;
            double num = Math.Pow((double)row, 2) + Math.Pow((double)col, 2);
            if (0 < num && num <= (double)this.AttackRadius2)
            {
                return true;
            }
            return false;
        }

        public bool IsEmpty(Location otherLocation)
        {
            return (
                from kv in this.Orders
                select this.Destination(kv.Key, this.ExtractDirectionsFromMoves(kv.Value.Moves))).All((Location location) => location != otherLocation);
        }

        public bool IsOccupied(Location loc)
        {
            List<Pirate> allPirates = this.AllPirates;
            return allPirates.Where(pirate => !pirate.IsLost).Any((Pirate pirate) => pirate.Location.Equals(loc));
        }

        public bool IsPassable(Location location)
        {
            if (location.Row < 0 || location.Col < 0 || location.Row >= this.Rows || location.Col >= this.Cols)
            {
                return false;
            }
            return this.Map[location.Row, location.Col] != -4;
        }

        public bool isVisible(Location loc)
        {
            if (this.Vision == null)
            {
                if (this.VisionOffsets2 == null)
                {
                    this.VisionOffsets2 = new List<Location>();
                    int num = (int)Math.Sqrt((double)this.ViewRadius2);
                    for (int i = -num; i <= num; i++)
                    {
                        for (int j = -num; j <= num; j++)
                        {
                            if ((int)(Math.Pow((double)i, 2) + Math.Pow((double)j, 2)) <= this.ViewRadius2)
                            {
                                this.VisionOffsets2.Add(new Location(i % this.Rows - this.Rows, j % this.Cols - this.Cols));
                            }
                        }
                    }
                }
                this.Vision = new bool[this.Rows, this.Cols];
                for (int k = 0; k < this.Rows; k++)
                {
                    for (int l = 0; l < this.Cols; l++)
                    {
                        this.Vision[k, l] = false;
                    }
                }
                foreach (Pirate pirate in this.MyPirates())
                {
                    Location location = pirate.Location;
                    foreach (Location visionOffsets2 in this.VisionOffsets2)
                    {
                        this.Vision[location.Row + visionOffsets2.Row, location.Col + visionOffsets2.Col] = true;
                    }
                }
            }
            return this.Vision[loc.Row, loc.Col];
        }

        public List<Pirate> MyDrunkPirates()
        {
            List<Pirate> pirates = this.MyPirates();
            return pirates.Where(pirate => pirate.TurnsToSober > 0).ToList();
        }

        public List<Pirate> MyLostPirates()
        {
            List<Pirate> pirates = this.AllMyPirates();
            return pirates.Where(pirate => pirate.IsLost).ToList();
        }

        public List<Pirate> MyPirates()
        {
            List<Pirate> pirates = this.AllMyPirates();
            return pirates.Where(pirate => !pirate.IsLost).ToList();
        }

        public List<Pirate> MyPiratesWithoutTreasures()
        {
            List<Pirate> pirates = this.MyPirates();
            return pirates.Where(pirate => !pirate.HasTreasure).ToList();
        }

        public List<Pirate> MyPiratesWithTreasures()
        {
            List<Pirate> pirates = this.MyPirates();
            return pirates.Where(pirate => pirate.HasTreasure).ToList();
        }

        public List<Pirate> MySoberPirates()
        {
            List<Pirate> pirates = this.MyPirates();
            return pirates.Where(pirate => pirate.TurnsToSober <= 0).ToList();
        }

        private Location parseLocationFrom(string row, string col)
        {
            return new Location(int.Parse(row), int.Parse(col));
        }

        private void parseMapUpdateData(string mapData)
        {
            using (StringReader stringReader = new StringReader(mapData))
            {
                while (true)
                {
                    string str = stringReader.ReadLine();
                    string lower = str;
                    if (str == null)
                    {
                        break;
                    }
                    lower = lower.Trim().ToLower();
                    if (lower.Length != 0)
                    {
                        string[] strArrays = lower.Split(new char[0]);
                        if (strArrays[0] == "w")
                        {
                            Location location = this.parseLocationFrom(strArrays[1], strArrays[2]);
                            this.Map[location.Row, location.Col] = -4;
                        }
                        else if (strArrays[0] == "g")
                        {
                            List<int> list = Enumerable.Repeat(0, (int)strArrays.Length - 2).ToList();
                            for (int i = 0; i < list.Count; i++)
                            {
                                list[i] = int.Parse(strArrays[i + 2]);
                            }
                            if (strArrays[1] == "s")
                            {
                                this.Scores = list;
                            }
                            else if (strArrays[1] == "p")
                            {
                                this.LastTurnPoints = list;
                            }
                        }
                        else if (strArrays[0] == "t")
                        {
                            this.parseTreasureData(strArrays);
                        }
                        else if ((int)strArrays.Length >= 4 && (strArrays[0] == "a" || strArrays[0] == "k" || strArrays[0] == "d"))
                        {
                            this.parsePirateData(strArrays);
                        }
                    }
                }
            }
        }

        private void parsePirateData(string[] tokens)
        {
            int num = int.Parse(tokens[1]);
            Location order = this.parseLocationFrom(tokens[2], tokens[3]);
            int num1 = int.Parse(tokens[4]);
            Location location = this.parseLocationFrom(tokens[5], tokens[6]);
            Pirate pirate = new Pirate(num, num1, order, location);
            if (tokens[0] != "k")
            {
                if (!this.Loc2Pirate.ContainsKey(order))
                {
                    this.Loc2Pirate.Add(order, pirate);
                }
                else
                {
                    this.Loc2Pirate[order] = pirate;
                }
                pirate.TurnsToSober = int.Parse(tokens[7]);
                pirate.HasTreasure = Convert.ToBoolean(int.Parse(tokens[8]));
                pirate.ReloadTurns = int.Parse(tokens[9]);
                pirate.DefenseExpirationTurns = int.Parse(tokens[10]);
            }
            else
            {
                pirate.TurnsToRevive = int.Parse(tokens[7]);
                pirate.IsLost = true;
            }
            this.AllPirates.Add(pirate);
            this.Orders[order] = new Order(new object[0]);
        }

        private void parseTreasureData(string[] tokens)
        {
            int num = int.Parse(tokens[1]);
            Location location = this.parseLocationFrom(tokens[2], tokens[3]);
            Treasure treasure = new Treasure(num, location);
            this.AllTreasures.Add(treasure);
        }

        public void SetSail(Pirate pirate, Location destination)
        {
            foreach (Direction direction in this.GetDirections(pirate.Location, destination))
            {
                this.Orders[pirate.Location].Moves.Add(new MoveDirection(direction));
            }
        }

        public bool ShouldRecoverErrors()
        {
            return this.recover_errors;
        }

        public void StopPoint(string message)
        {
            Console.Out.WriteLine(string.Format("s {0}", PirateGame.Base64Encode(message)));
            Console.Out.Flush();
        }

        public int TimeRemaining()
        {
            TimeSpan now = DateTime.Now - this.TurnStartTime;
            return (Convert.ToInt16(this.Turn == 1) * 9 + 1) * this.TurnTime - now.Milliseconds;
        }

        public List<Treasure> Treasures()
        {
            return new List<Treasure>(this.AllTreasures);
        }

        public void Update(string mapData)
        {
            this.TurnStartTime = DateTime.Now;
            this.Vision = null;
            this.AllPirates = new List<Pirate>();
            this.AllTreasures = new List<Treasure>();
            this.Loc2Pirate = new Dictionary<Location, Pirate>();
            this.SortedMyPirates = new List<Pirate>();
            this.SortedEnemyPirates = new List<Pirate>();
            this.Turn = this.Turn + 1;
            this.parseMapUpdateData(mapData);
            List<Pirate> allPirates = this.AllPirates;
            IEnumerable<Pirate> pirates = allPirates.Where(pirate => pirate.Owner == 0);
            this.SortedMyPirates = pirates.OrderBy(pirate => pirate.Id).ToList();
            List<Pirate> allPirates1 = this.AllPirates;
            IEnumerable<Pirate> pirates1 = allPirates1.Where(pirate => pirate.Owner != 0);
            this.SortedEnemyPirates = pirates1.OrderBy(pirate => pirate.Id).ToList();
            List<Treasure> allTreasures = this.AllTreasures;
            this.AllTreasures = allTreasures.OrderBy(treasure => treasure.Id).ToList();
        }

        private List<Tuple<Location, List<Location>>> ValidateCollisions()
        {
            Dictionary<Location, List<Location>> locations = new Dictionary<Location, List<Location>>();
            List<Pirate> pirates = this.MyPirates();
            List<Tuple<Location, bool>> list = (
                from loc in pirates.Select(pir => pir.Location).ToList()
                select Tuple.Create(loc, this.Orders.ContainsKey(loc))).ToList();
            foreach (Tuple<Location, bool> tuple in list.OrderBy(tup => tup.Item2).ToList())
            {
                Location item1 = tuple.Item1;
                Location location = (tuple.Item2 ? this.Destination(item1, this.ExtractDirectionsFromMoves(this.Orders[item1].Moves)) : item1);
                if (!locations.ContainsKey(location))
                {
                    locations[location] = new List<Location>();
                }
                locations[location].Add(item1);
            }
            List<Tuple<Location, List<Location>>> tuples = new List<Tuple<Location, List<Location>>>();
            foreach (KeyValuePair<Location, List<Location>> keyValuePair in locations)
            {
                Location key = keyValuePair.Key;
                List<Location> value = keyValuePair.Value;
                if (value.Count() <= 1)
                {
                    continue;
                }
                tuples.Add(Tuple.Create(key, value));
            }
            return tuples;
        }
    }
}
