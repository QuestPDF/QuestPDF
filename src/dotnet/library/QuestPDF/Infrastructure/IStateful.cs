namespace QuestPDF.Infrastructure
{
    internal interface IStateful
    {
        void ResetState(bool hardReset = true);
        object GetState();
        void SetState(object state);
    }
}