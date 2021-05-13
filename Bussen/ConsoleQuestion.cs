using AsyncNullString = System.Threading.Tasks.Task<string?>;
using AsyncTask = System.Threading.Tasks.Task;
using ConsoleIF = System.Console;

namespace Bussen
{
    public class ConsoleQuestion : Question
    {
        public override void Tell(string text, bool newlineBefore, bool newlineAfter)
        {
            if (newlineBefore)
            {
                ConsoleIF.WriteLine();
            }

            ConsoleIF.Write(text);

            if (newlineAfter)
            {
                ConsoleIF.WriteLine();
            }
        }

        protected override async AsyncNullString Ask(string question)
        {
            Tell(question + " ", true, false);
            return await AsyncTask.Run(() => ConsoleIF.ReadLine());
        }
    }
}