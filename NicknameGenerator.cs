using QuizTools.GeneralUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools
{
    class NicknameGenerator
    {
        public static string[] GenerateNicknameWithZeroWidthSpace(string nickname, int maxLength, int count, bool includeOriginal = true)
        {
            if (nickname.Length > maxLength)
                throw new ArgumentException("Nickname is longer than max length");

            if (nickname.Length == maxLength)
                return includeOriginal ? [nickname] : Array.Empty<string>();

            int insertable = maxLength - nickname.Length;
            List<int[]> positions = MathFunctions.DivideIntoAddends(insertable, nickname.Length, true);
            HashSet<string> nicknames = new();

            if (includeOriginal)
                nicknames.Add(nickname);

            foreach (int[] position in positions)
            {
                if (nicknames.Count >= count)
                    break;

                StringBuilder newNickname = new StringBuilder();

                for (int j = 0; j < nickname.Length; j++)
                {
                    for (int k = 0; k < position[j]; k++)
                        newNickname.Append(GeneralConstants.ZERO_WIDTH_SPACE);

                    newNickname.Append(nickname[j]);
                }

                nicknames.Add(newNickname.ToString());
            }

            return nicknames.ToArray();
        }


        public static string[] RandomNicknamesWithoutRepeats(int count)
        {
            count = Math.Min(count, MaxNicknamesCount);
            if (count <= 0)
                return Array.Empty<string>();

            HashSet<string> nicknamesSet = new();
            List<string> allNicknames = AllNicknames().ToList();

            while (nicknamesSet.Count < count)
            {
                string nickname = Program.RNG.ChoseRandom(allNicknames);
                nicknamesSet.Add(nickname);
                allNicknames.Remove(nickname);
                Logger.WriteInfoLine(nicknamesSet.Count);
            }
            return nicknamesSet.ToArray();
        }
        public static string[] RandomNicknames(int count)
        {
            count = Math.Min(count, MaxNicknamesCount);
            if (count <= 0)
                return Array.Empty<string>();

            string[] nicknames = new string[count];
            for (int i = 0; i < count; i++)
                nicknames[i] = RandomNickname();
            return nicknames;
        }
        public static string RandomNickname()
        {
            if (Program.RNG.RandomBool())
                return $"{Program.RNG.ChoseRandom(firstNames)} {Program.RNG.ChoseRandom(secondNames)}";
            else
                return $"{Program.RNG.ChoseRandom(secondNames)} {Program.RNG.ChoseRandom(firstNames)}";
        }
        public static string[] AllNicknames()
        {
            string[] nicknames = new string[MaxNicknamesCount];
            int index = 0;
            foreach (string first in firstNames)
            {
                foreach (string second in secondNames)
                {
                    nicknames[index++] = $"{first} {second}";
                    nicknames[index++] = $"{second} {first}";
                }
            }
            return nicknames;
        }
        public static int MaxNicknamesCount => firstNames.Length * secondNames.Length * 2;

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
    }
}
