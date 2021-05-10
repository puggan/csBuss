namespace Bussen
{
    public class WindowsQuestion : Question
    {
        public override void Tell(string text, bool newlineBefore, bool newlineAfter) => Microsoft.VisualBasic.Interaction.MsgBox(text);

        protected override async System.Threading.Tasks.Task<string?> Ask(string question) => await System.Threading.Tasks.Task.Run(
            () => Microsoft.VisualBasic.Interaction.InputBox(question)
        );
    }
}