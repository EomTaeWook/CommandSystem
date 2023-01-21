namespace CLISystem.Interface
{
    public interface ICmdProcessor
    {
        void Invoke(string[] args);

        string Print();
    }
}
