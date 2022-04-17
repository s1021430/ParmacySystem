namespace HisApiBase
{
    public class NHICard
    {
        public NHICard(IcCardData basicData)
        {
            BasicData = basicData;
        }

        public IcCardData BasicData { get; set; }
        public SeqNumber SeqNumberData { get; set; }
    }
}
