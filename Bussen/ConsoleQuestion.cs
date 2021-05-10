namespace Bussen
{
    public class ConsoleQuestion : Question
    {
        public override void Tell(string text, bool newlineBefore, bool newlineAfter)
        {
            if (newlineBefore)
            {
                System.Console.WriteLine();
            }

            System.Console.Write(text);

            if (newlineAfter)
            {
                System.Console.WriteLine();
            }
        }

        protected override async System.Threading.Tasks.Task<string?> Ask(string question)
        {
            Tell(question + " ", true, false);
            return await System.Threading.Tasks.Task.Run(() => System.Console.ReadLine());
        }
    }
}