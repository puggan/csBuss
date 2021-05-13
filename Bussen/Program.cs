using AppException = System.ApplicationException;
using AsyncBool = System.Threading.Tasks.Task<bool>;
using AsyncTask = System.Threading.Tasks.Task;
using Env = System.Environment;
using RandomSeed = System.Random;
using SexList = System.Collections.Generic.List<Bussen.Sex>;
using StringConcater = System.Text.StringBuilder;
using StringList = System.Collections.Generic.List<string>;
using ProgramList = System.Collections.Generic.List<Bussen.Program>;

namespace Bussen
{
    class Program
    {
        private static readonly StringList MainMenyOptions = new() {
            "Lägg till passagerare",
            "Lista passagerare",
            "Summera åldern",
            "Peta på slumpvald passagerare",
            "Skrota bussen"
        };
        private static readonly StringList SexNamedOptions = new() {
            "Okänd",
            "Man",
            "Kvinna",
            "Annat",
        };
        private static readonly SexList SexOptions = new() {
            Sex.Unknown,
            Sex.Male,
            Sex.Female,
            Sex.Other
        };
        
        private Buss? buss;
        private readonly Question ui;
        private RandomSeed? seed;
        private readonly AsyncTask main;

        private static void Main(string[] args)
        {
            int port = 1337;
            ProgramList activePrograms = new();
            Question consoleUI = Question.Console();
            consoleUI.Tell("Program also avaible using telnet at port " + port);

            Program consoleProgram = (new Program(consoleUI));
            activePrograms.Add(consoleProgram);

            AsyncTask listener = AsyncTask.Run(() =>
                {
                    foreach (Question connection in NetworkQuestion.ListenUi(port))
                    {
                        activePrograms.Add(new Program(connection));
                    }
                }
            );

            consoleProgram.main.GetAwaiter().GetResult();
            NetworkQuestion.stop();
            
            foreach (var program in activePrograms)
            {
                program.main.GetAwaiter().GetResult();
            }
        }

        private Program(Question ui)
        {
            this.ui = ui;
            main = MainLoop();
        }

        private async AsyncTask MainLoop()
        {
            ui.Tell("Welcome to the awesome Buss-simulator, Buss Tycoon v13.37");
            int size = await ui.AskUInt("Hur många passagerar får det plats på bussen?");
            buss = new Buss(size);

            while(true)
            {
                try
                {
                    await MainMenu();
                }
                catch (ExitException)
                {
                    break;
                }
            }

            //Env.Exit(0);
        }

        private string Status()
        {
            if (buss == null)
            {
                throw new AppException("Unreachable state");
            }

            return string.Format(
                "Bussen har en chaför och {0} av maximalt {1} passagerare",
                buss.PassengerCount(),
                buss.PassengerMaxCount()
            );
        }

        private async AsyncBool MainMenu()
        {
            switch (await ui.AskOptions($"Status {this.Status()}", MainMenyOptions))
            {
                case 5:
                    throw new ExitException("User want's to leave");
                case 1:
                    return await AddPassenger();
                case 2:
                    return await ListPassenger();
                case 3:
                    return await SumPassenger();
                case 4:
                    return await PokePassenger();
            }

            throw new AppException("Unreachable state?");
        }

        private async AsyncBool AddPassenger()
        {
            if (buss == null)
            {
                throw new AppException("Unreachable state");
            }

            if (buss.PassengerMaxCount() <= buss.PassengerCount())
            {
                ui.Tell("Tyvärr, bussen är full");
                return false;
            }

            int sexIndex = await ui.AskOptions("Ny passagerare, hur identiferar sig passageraren? ", SexNamedOptions);
            int age = await ui.AskIntRange("Hur gammal är passagerare? ", 0, 999);

            buss.AddPassenger(new Passenger(SexOptions[sexIndex], age));
            return true;
        }

        private AsyncBool ListPassenger()
        {
            if (buss == null)
            {
                throw new AppException("Unreachable state");
            }

            StringConcater text = new();
            foreach (Passenger passenger in buss.Passengers())
            {
                text.AppendLine(string.Format(
                    "* En {1} på {0} år",
                    passenger.Age(),
                    SexTranslations.sv(passenger.Sex())
                ));
            }
            ui.Tell(text.ToString());
            return AsyncTask.FromResult(true);
        }

        private AsyncBool SumPassenger()
        {
            if (buss == null)
            {
                throw new AppException("Unreachable state");
            }

            int totalAge = 0;
            int count = 0;
            foreach (Passenger passenger in buss.Passengers())
            {
                count++;
                totalAge += passenger.Age();
            }

            if (count > 0)
            {
                ui.Tell(string.Format(
                    "Total ålder är {0} år på de {1} passagerarna, ett snitt på {2} år",
                    totalAge,
                    count,
                    (double) totalAge / count
                ));
            }
            else
            {
                ui.Tell("Total ålder 0 år på de 0 passagerarna, ett snitt på 1337 år");
            }
            return AsyncTask.FromResult(true);
        }

        private AsyncBool PokePassenger()
        {
            if (buss == null)
            {
                throw new AppException("Unreachable state");
            }

            if (buss.PassengerCount() < 1)
            {
                ui.Tell("Petade på alla sätten i bussen, inte en enda reaktion från varken männsika eller spöke.");
                return AsyncTask.FromResult(false);
            }

            if (seed == null)
            {
                seed = new RandomSeed();
            }

            int index = seed.Next(0, buss.PassengerCount());
            ui.Tell(string.Format(
                "Petade på passagerare nr {0}, passagerarens reaktion var: {1}",
                index,
                buss.Passenger(index).pokeReaction()
            ));

            return AsyncTask.FromResult(true);
        }
    }
}