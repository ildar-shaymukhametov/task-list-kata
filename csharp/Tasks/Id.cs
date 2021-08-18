namespace Tasks
{
    public class Id
    {
        private static long lastId;
        public Id(string value)
        {
            Value = long.Parse(value);
        }

        public long Value { get; }

        public static long GetNextId()
        {
            return ++lastId;
        }
    }
}
