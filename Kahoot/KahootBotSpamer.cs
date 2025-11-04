using QuizTools.GeneralUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizTools.Kahoot.Games;
using QuizTools.Kahoot.Exceptions;

namespace QuizTools.Kahoot
{
    class KahootBotSpamer
    {
        private static readonly string[] firstNames = [
            "Amazing", "Yellow", "Majestic", "Fast", "Smart", "Astonishing", "Golden", "Impressive", "Big", "Magic",
            "Epic", "Legendary", "Mighty", "Swift", "Clever", "Brilliant", "Radiant", "Glorious", "Supreme", "Ultimate",
            "Quantum", "Cosmic", "Galactic", "Solar", "Lunar", "Stellar", "Celestial", "Orbital", "Nebular",
            "Crimson", "Azure", "Emerald", "Violet", "Scarlet", "Sapphire", "Ruby", "Amber", "Jade", "Onyx",
            "Electric", "Thunder", "Lightning", "Storm", "Blizzard", "Hurricane", "Tornado", "Volcanic", "Inferno", "Arctic",
            "Phantom", "Shadow", "Ghost", "Stealth", "Mystic", "Enigmatic", "Cryptic", "Arcane", "Ethereal", "Mystical",
            "Iron", "Steel", "Titanium", "Adamant", "Diamond", "Crystal", "Obsidian", "Granite", "Marble", "Cobalt",
            "Digital", "Virtual", "Cyber", "Neon", "Pixel", "Binary", "Algorithm", "Matrix", "Syntax", "Protocol",
            "Ancient", "Timeless", "Eternal", "Immortal", "Primeval", "Prehistoric", "Mythic", "Legend", "Fabled", "Historic",
            "Flying", "Soaring", "Gliding", "Hovering", "Aerial", "Skybound", "Cloud", "Zenith", "Summit", "Peak",
            "Oceanic", "Marine", "Abyssal", "Tidal", "Aqua", "Deep", "Coral", "Reef", "Wave", "Tsunami",
            "Forest", "Jungle", "Natural", "Organic", "Eco",
            "Mechanical", "Robotic", "Automated", "Precise", "System", "Engine", "Gear", "Piston", "Hydraulic", "Pneumatic",
            "Royal", "Imperial", "Noble", "Regal", "Majesty", "Dynasty", "Empire", "Kingdom", "Realm", "Dominion",
            "Silent", "Quiet", "Peaceful", "Calm", "Serene", "Tranquil", "Still", "Hushed", "Muted", "Whisper",
            "Rapid", "Quick", "Instant", "Immediate", "Sudden", "Accelerated", "Express", "Turbo", "Velocity", "Momentum",
            "Giant", "Colossal", "Massive", "Enormous", "Gigantic", "Titan", "Behemoth", "Mammoth", "Vast", "Immense",
            "Tiny", "Micro", "Mini", "Petite", "Compact", "Atomic", "Subatomic", "Nano", "Pico", "Femto",
            "Happy", "Joyful", "Cheerful", "Jolly", "Merry", "Blissful", "Ecstatic", "Delighted", "Pleased", "Content"
        ];


        private static readonly string[] secondNames = [
            "Elephant", "Snail", "Zebra", "Knight", "Shaman", "Artist",
            "Dragon", "Griffin", "Unicorn", "Pegasus", "Chimera", "Kraken", "Leviathan", "Hydra",
            "Wizard", "Ranger", "Bard", "Monk", "Druid",
            "Tiger", "Lion", "Wolf", "Bear", "Eagle", "Hawk", "Falcon", "Raven", "Owl", "Crow",
            "Samurai", "Viking", "Spartan", "Gladiator", "Centurion", "Valkyrie",
            "Android", "Cyborg", "Drone", "Bot", "Droid", "Automaton", "Golem", "Sentinel", "Watcher",
            "Pioneer", "Explorer", "Adventurer", "Voyager", "Navigator", "Pathfinder", "Trailblazer", "Discoverer", "Scout", "Surveyor",
            "Genius", "Prodigy", "Savant", "Virtuoso", "Master", "Expert", "Specialist", "Authority", "Guru", "Maestro",
            "Champion", "Victor", "Winner", "Hero", "Legend", "Icon", "Star", "Superstar", "Celebrity", "Idol",
            "Phantom", "Ghost", "Specter", "Wraith", "Spirit", "Apparition", "Shadow", "Shade",
            "Titan", "Colossus", "Behemoth", "Mammoth", "Giant", "Monolith", "Tower",
            "Comet", "Asteroid", "Meteor", "Nova", "Supernova", "Quasar", "Pulsar", "Nebula", "Galaxy",
            "Hurricane", "Avalanche", "Blizzard", "Tornado", "Volcano", "Lightning", "Thunder", "Storm",
            "Alchemist", "Scientist", "Researcher", "Inventor", "Innovator", "Theorist", "Analyst", "Strategist",
            "Jester", "Clown", "Comedian", "Entertainer", "Performer", "Actor", "Magician", "Illusionist", "Trickster",
            "Captain", "Commander", "General", "Admiral", "Marshal", "Chief", "Director", "Manager", "Supervisor", "Coordinator",
            "Scholar", "Sage", "Mentor", "Teacher", "Professor", "Doctor", "Academic", "Intellectual", "Philosopher",
            "Guardian", "Protector", "Defender", "Warden", "Keeper", "Custodian", "Guard", "Shield", "Bulwark",
            "Nomad", "Wanderer", "Traveler", "Roamer", "Drifter", "Wayfarer", "Pilgrim", "Globetrotter", "Explorer",
            "Engineer", "Architect", "Designer", "Builder", "Creator", "Maker", "Artisan", "Craftsman", "Technician", "Mechanic",
            "Prophet", "Oracle", "Seer", "Diviner", "Fortune", "Mystic", "Visionary", "Clairvoyant", "Psychic", "Medium",
            "Ninja", "Shogun", "Ronin", "Kabuki", "Sumo", "friendly",
            "Cyclops", "Minotaur", "Satyr", "Centaur", "Siren", "Harpy", "Medusa", "Gorgon", "Cerberus", "Sphinx",
            "Jaguar", "Panther", "Cheetah", "Lynx", "Cougar", "Puma", "Leopard", "Snow", "Night",
            "Phoenix", "Firebird", "Salamander", "Ifrit", "Djinn", "Efreet", "Drake", "Wyrm", "Wyvern"
        ];

        private static string RandomNickname => $"{firstNames[rng.Next(0, firstNames.Length)]} {secondNames[rng.Next(0, secondNames.Length)]}";
        private static Random rng = new Random();

        private List<string> generatedNicknames;
        private string NextNickname
        {
            get
            {
                if (generatedNicknames.Count == 0)
                    return "";
                string name = generatedNicknames[generatedNicknames.Count - 1];
                generatedNicknames.RemoveAt(generatedNicknames.Count - 1);
                return name;
            }
        }

        private KahootChallenge challenge;
        private HttpClient client;

        public int Spamed { get; private set; }
        public Action<KahootPlayer> OnBotSent;
        public bool HasBaseNickname { get; }

        public KahootBotSpamer(string baseNickname, KahootChallenge challenge)
        {
            GenerateNicknames(baseNickname);
            HasBaseNickname = !string.IsNullOrEmpty(baseNickname);
            this.challenge = challenge;
            HttpClientHandler handler = new HttpClientHandler()
            {
                MaxConnectionsPerServer = 50,
                UseProxy = false,
                UseCookies = false
            };
            client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            Spamed = 0;
        }
        private void GenerateNicknames(string baseNickname)
        {
            generatedNicknames = new();
            if (string.IsNullOrEmpty(baseNickname))
            {
                while (generatedNicknames.Count < KahootConstants.MAX_PLAYERS)
                {
                    string name = RandomNickname;
                    if (!generatedNicknames.Contains(name))
                        generatedNicknames.Add(name);
                }
                return;
            }

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(baseNickname);
            generatedNicknames.Add(baseNickname);

            while (queue.Count > 0 && generatedNicknames.Count < KahootConstants.MAX_PLAYERS)
            {
                string current = queue.Dequeue();

                for (int i = 0; i <= current.Length; i++)
                {
                    string newNick = current.Insert(i, GeneralConstants.INVISIBLE_SYMBOL);
                    if (newNick.Length <= KahootConstants.MAX_NICKNAME_LENGHT && !generatedNicknames.Contains(newNick))
                    {
                        generatedNicknames.Add(newNick);
                        queue.Enqueue(newNick);

                        if (generatedNicknames.Count >= KahootConstants.MAX_PLAYERS)
                            break;
                    }
                }
            }

        }

        public async Task SpamAsync()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(10);
            List<Task> tasks = new List<Task>();

            bool stopWhile = false;
            while (!stopWhile)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        string nickname = NextNickname;
                        if (nickname != "")
                        {
                            KahootPlayer player = await challenge.JoinAsync(nickname, client);
                            Spamed++;
                            OnBotSent?.Invoke(player);
                        }
                        else
                            stopWhile = true;
                    }
                    catch (NicknameExistsException)
                    {
                        stopWhile = true;
                    }
                    catch (KahootIsFullException)
                    {
                        stopWhile = true;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));

                if (tasks.Count >= 100)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }
            await Task.WhenAll(tasks);
        }

        public void Spam()
        {
            while (true)
            {
                try
                {
                    string nickname = NextNickname;
                    if (nickname == "")
                        return;
                    KahootPlayer player = challenge.Join(nickname, client);
                    Spamed++;
                    OnBotSent?.Invoke(player);
                }
                catch (NicknameExistsException) { }
                catch (KahootIsFullException)
                {
                    break;
                }
            }
        }

    }
}
