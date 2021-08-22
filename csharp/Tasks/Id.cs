namespace Tasks
{
    public class Id
    {
        private static long lastId;
        private string _value;

        public Id(string value)
        {
            if (value.Contains(' '))
            {
                throw new System.Exception("no spaces");
            }
            _value = value;
        }

        public static string GetNextId()
        {
            return (++lastId).ToString();
        }

        public static implicit operator string(Id id) => id._value;
    }
}
