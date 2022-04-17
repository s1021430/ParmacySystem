namespace GeneralClass
{
    class CommonSqlSyntax
    {
        private static CommonSqlSyntax instance;
        public static CommonSqlSyntax GetInstance()
        {
            if(instance == null)
                instance = new CommonSqlSyntax();

            return instance;
        }
    }
}
