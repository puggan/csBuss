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

        protected override async System.Threading.Tasks.Task<string?> Ask(string question)
        {
            Tell(question + " ", true, false);
            return await System.Threading.Tasks.Task.Run(() => ConsoleIF.ReadLine());
        }
    }
}