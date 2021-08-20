namespace Tasks
{
    public class Id
    {
        private static long lastId;
        private long _value;

        public Id(string value)
        {
            _value = long.Parse(value);
        }

        public static long GetNextId()
        {
            return ++lastId;
        }

        public static implicit operator long(Id id) => id._value;
    }
}
