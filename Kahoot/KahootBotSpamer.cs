using QuizTools.GeneralUtils;
using QuizTools.Kahoot.Exceptions;
using QuizTools.Kahoot.Games;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootBotSpamer
    {

        private KahootChallenge challenge;
        private HttpClient client;

        private List<string> generatedNicknames;
        public int Spamed { get; private set; }
        public Action<KahootPlayer> OnBotSent;
        public bool HasBaseNickname { get; }
        
        public KahootBotSpamer(KahootChallenge challenge) : this("", challenge) { }
        public KahootBotSpamer(string baseNickname, KahootChallenge challenge)
        {
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
            HasBaseNickname = !string.IsNullOrEmpty(baseNickname);
            GenerateNicknames(baseNickname);
        }
        private void GenerateNicknames(string baseNickname)
        {
            if (HasBaseNickname)
            {
                generatedNicknames = NicknameGenerator.GenerateNicknameWithZeroWidthSpace(baseNickname, KahootConstants.MAX_NICKNAME_LENGHT, challenge.MaxPlayers, x => !Kahoot.NicknameHasProfanity(x)).ToList();
                Logger.WriteInfoLine($"Generated {generatedNicknames.Count} nicknames based on '{baseNickname}'");
                return;
            }

            generatedNicknames = NicknameGenerator.RandomNicknames(challenge.MaxPlayers, x => !Kahoot.NicknameHasProfanity(x)).ToList();
        }
        public async Task SpamAsync()
        {
            List<Task> allTasks = new List<Task>();
            SemaphoreSlim semaphore = new SemaphoreSlim(5);

            while (generatedNicknames.Count > 0)
            {
                await semaphore.WaitAsync();

                string nickname;
                lock (generatedNicknames)
                {
                    if (generatedNicknames.Count == 0)
                    {
                        semaphore.Release();
                        break;
                    }
                    nickname = generatedNicknames[0];
                    generatedNicknames.RemoveAt(0);
                }

                Task task = Task.Run(async () =>
                {
                    try
                    {
                        KahootPlayer player = await challenge.JoinAsync(nickname, client);
                        Spamed++;
                        OnBotSent?.Invoke(player);
                    }
                    catch (Exception ex) when (ex is NicknameExistsException or KahootIsFullException)
                    {
                        if (ex is KahootIsFullException)
                        {
                            lock (generatedNicknames)
                                generatedNicknames.Clear();
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                allTasks.Add(task);
            }

            await Task.WhenAll(allTasks);
        }


        public void Spam()
        {

            while (generatedNicknames.Count > 0)
            {

                try
                {
                    string nickname;
                    if (generatedNicknames.Count == 0)
                        return;
                    nickname = generatedNicknames[0];
                    KahootPlayer player = challenge.Join(nickname, client);
                    Spamed++;
                    OnBotSent?.Invoke(player);
                    generatedNicknames.RemoveAt(0);
                }
                catch (NicknameExistsException)
                {
                }
                catch (KahootIsFullException)
                {
                    generatedNicknames.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

    }
}
