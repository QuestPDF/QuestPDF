namespace QuestPDF.Infrastructure;

internal interface IStateful
{
    public object CloneState();
    public void SetState(object state);
    public void ResetState(bool hardReset);
}
