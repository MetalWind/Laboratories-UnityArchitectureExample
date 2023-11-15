namespace Laboratory.Core
{
    public interface IDataResolver
    {
        public IDataAccessor CreateAccessor();
        public void SaveCurrent();
    }
}