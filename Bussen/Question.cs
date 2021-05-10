namespace Bussen
{
    public abstract class Question
    {
        public static ConsoleQuestion Console() => new ConsoleQuestion();
        public static WindowsQuestion Window() => new WindowsQuestion();

        public abstract void Tell(string text, bool newlineBefore, bool newlineAfter);

        protected abstract System.Threading.Tasks.Task<string?> Ask(string question);

        public void Tell(string text) => Tell(text, false, true);

        public System.Threading.Tasks.Task<string> AskString(string question) => AskString(
            question,
            new System.Text.RegularExpressions.Regex(".")
        );

        public async System.Threading.Tasks.Task<int> AskInt(string question) => System.Int32.Parse(
            await AskString(question, new System.Text.RegularExpressions.Regex("^-?(0|[1-9]\\d*)$"))
        );

        public async System.Threading.Tasks.Task<int> AskUInt(string question) => System.Int32.Parse(
            await AskString(question, new System.Text.RegularExpressions.Regex("^(0|[1-9]\\d*)$"))
        );

        public async System.Threading.Tasks.Task<int> AskIntRange(string question, int min, int max)
        {
            if (min > max)
            {
                throw new System.ArgumentException("Invalid range " + min + " - " + max + ".");
            }

            while(true)
            {
                int answer = await AskInt(question);
                if (answer >= min && answer <= max)
                {
                    return answer;
                }
            }
        }

        public async System.Threading.Tasks.Task<int> AskOptions(string title,
            System.Collections.Generic.IEnumerable<string> list)
        {
            int index = 0;
            System.Text.StringBuilder text = new System.Text.StringBuilder(title);
            foreach(string row in list) {
                text.AppendLine();
                text.Append($"{++index}. {row}.");
            }

            text.AppendLine();
            text.Append($"Ange radnummer ovan (1-{index}): ");

            return await AskIntRange(text.ToString(), 1, index);
        }
        
        public async System.Threading.Tasks.Task<string> AskString(string question, System.Text.RegularExpressions.Regex filter)
        {
            string? answer;
            do
            {
                answer = await Ask(question);
            } while (answer == null || !filter.IsMatch(answer));

            return answer;
        }
    }
}