namespace ExpiredPassportChecker.Helpers
{
    public class InMemoryContainer<T>
        where T : new()
    {
        public InMemoryContainer()
        {
            Value = new T();
        }

        public T Value { get; set; }
    }
}
