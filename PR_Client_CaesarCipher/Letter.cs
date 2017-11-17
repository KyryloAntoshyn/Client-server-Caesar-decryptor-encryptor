namespace PR_Client_CaesarCipher
{
    // Контейнер для хранения информации о букве
    public class Letter
    {
        public string Name { get; set; }
        public double Frequency { get; set; }
        public Letter()
        {
            Name = "";
            Frequency = 0;
        }
        public Letter(string Name, double Frequency)
        {
            this.Name = Name;
            this.Frequency = Frequency;
        }
    }
}
