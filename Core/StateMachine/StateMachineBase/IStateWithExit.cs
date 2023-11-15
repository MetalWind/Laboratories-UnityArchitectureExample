namespace Laboratory.Core
{
    public interface IStateWithExit : IState
    {
        public void OnExit();
    }
}