namespace HisApiBase
{
    public abstract class HISAPIService
    {
        public abstract NHICard GetCardWithBasicData();

        public abstract void GetMedicalNumber(NHICard card);
    }

    public class HISAPI : HISAPIService
    {
        private readonly IHISAPI hisAPI;
        public HISAPI(bool isInDesign)
        {
            if (isInDesign)
                hisAPI = new HisAPITest();
            else
                hisAPI = new HisAPI();
        }

        public override NHICard GetCardWithBasicData()
        {
            return new(hisAPI.GetBasicData());
        }

        public override void GetMedicalNumber(NHICard card)
        {
            card.SeqNumberData = hisAPI.GetSeqNumber(1);
        }
    }
}
