using ArgumentException = System.ArgumentException;
using AsyncInt = System.Threading.Tasks.Task<int>;
using AsyncNullString = System.Threading.Tasks.Task<string?>;
using AsyncString = System.Threading.Tasks.Task<string>;
using RegExp = System.Text.RegularExpressions.Regex;
using StringConcater = System.Text.StringBuilder;
using StringList = System.Collections.Generic.IEnumerable<string>;

namespace Bussen
{
    public abstract class Question
    {
        public static ConsoleQuestion Console() => new();
        public static WindowsQuestion Window() => new();

        public abstract void Tell(string text, bool newlineBefore, bool newlineAfter);

        protected abstract AsyncNullString Ask(string question);

        public void Tell(string text) => Tell(text, false, true);

        public AsyncString AskString(string question) => AskString(
            question,
            new RegExp(".")
        );

        public async AsyncInt AskInt(string question) => System.Int32.Parse(
            await AskString(question, new RegExp("^-?(0|[1-9]\\d*)$"))
        );

        public async AsyncInt AskUInt(string question) => System.Int32.Parse(
            await AskString(question, new RegExp("^(0|[1-9]\\d*)$"))
        );

        public async AsyncInt AskIntRange(string question, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException("Invalid range " + min + " - " + max + ".");
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

        public async AsyncInt AskOptions(string title, StringList list)
        {
            int index = 0;
            StringConcater text = new(title);
            foreach(string row in list) {
                text.AppendLine();
                text.Append($"{++index}. {row}.");
            }

            text.AppendLine();
            text.Append($"Ange radnummer ovan (1-{index}): ");

            return await AskIntRange(text.ToString(), 1, index);
        }
        
        public async AsyncString AskString(string question, RegExp filter)
        {
            string cleanAnswer;
            do
            {
                string? answer;
                do
                {
                    answer = await Ask(question);
                } while (answer == null);

                cleanAnswer = answer.Trim();
            } while (!filter.IsMatch(cleanAnswer));

            return cleanAnswer;
        }
    }
}