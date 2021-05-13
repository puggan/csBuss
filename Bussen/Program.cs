namespace Bussen
{
    class Program
    {
        private static readonly System.Collections.Generic.IList<string> MainMenyOptions = new System.Collections.Generic.List<string>() {
            "Lägg till passagerare",
            "Lista passagerare",
            "Summera åldern",
            "Peta på slumpvald passagerare",
            "Skrota bussen"
        };
        private static readonly System.Collections.Generic.IList<string> SexNamedOptions = new System.Collections.Generic.List<string>() {
            "Okänd",
            "Man",
            "Kvinna",
            "Annat",
        };
        private static readonly System.Collections.Generic.IList<Sex> SexOptions = new System.Collections.Generic.List<Sex>() {
            Sex.Unknown,
            Sex.Male,
            Sex.Female,
            Sex.Other
        };
        
        private Buss? buss;
        private readonly Question ui;
        private System.Random? seed;
        private readonly System.Threading.Tasks.Task main;

        private static void Main(string[] args)
        {
            (new Program(Question.Console())).main.GetAwaiter().GetResult();
        }

        private Program(Question ui)
        {
            this.ui = ui;
            main = MainLoop();
        }

        private async System.Threading.Tasks.Task MainLoop()
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

            System.Environment.Exit(0);
        }

        private string Status()
        {
            if (buss == null)
            {
                throw new System.ApplicationException("Unreachable state");
            }

            return string.Format(
                "Bussen har en chaför och {0} av maximalt {1} passagerare",
                buss.PassengerCount(),
                buss.PassengerMaxCount()
            );
        }

        private async System.Threading.Tasks.Task<bool> MainMenu()
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

            throw new System.ApplicationException("Unreachable state?");
        }

        private async System.Threading.Tasks.Task<bool> AddPassenger()
        {
            if (buss == null)
            {
                throw new System.ApplicationException("Unreachable state");
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

        private System.Threading.Tasks.Task<bool> ListPassenger()
        {
            if (buss == null)
            {
                throw new System.ApplicationException("Unreachable state");
            }

            System.Text.StringBuilder text = new System.Text.StringBuilder();
            foreach (Passenger passenger in buss.Passengers())
            {
                text.AppendLine(string.Format(
                    "* En {1} på {0} år",
                    passenger.Age(),
                    SexTranslations.sv(passenger.Sex())
                ));
            }
            ui.Tell(text.ToString());
            return System.Threading.Tasks.Task.FromResult(true);
        }

        private System.Threading.Tasks.Task<bool> SumPassenger()
        {
            if (buss == null)
            {
                throw new System.ApplicationException("Unreachable state");
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
            return System.Threading.Tasks.Task.FromResult(true);
        }

        private System.Threading.Tasks.Task<bool> PokePassenger()
        {
            if (buss == null)
            {
                throw new System.ApplicationException("Unreachable state");
            }

            if (buss.PassengerCount() < 1)
            {
                ui.Tell("Petade på alla sätten i bussen, inte en enda reaktion från varken männsika eller spöke.");
                return System.Threading.Tasks.Task.FromResult(false);
            }

            if (seed == null)
            {
                seed = new System.Random();
            }

            int index = seed.Next(0, buss.PassengerCount());
            ui.Tell(string.Format(
                "Petade på passagerare nr {0}, passagerarens reaktion var: {1}",
                index,
                buss.Passenger(index).pokeReaction()
            ));

            return System.Threading.Tasks.Task.FromResult(true);
        }
    }
}