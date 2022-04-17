using System.Collections.Generic;

namespace PharmacySystem.Class.Features
{
    public class FeaturesCollection
    {
        public FeaturesType Type { get; }
        public List<FeatureItem> Features { get; }
        public FeaturesCollection(FeaturesType type, IEnumerable<FeatureItem> features)
        {
            Type = type;
            Features = new List<FeatureItem>(features);
            if (type != FeaturesType.處方作業) return;
            var prescriptionEdit = Features.Find(f => f.Feature == FeaturesEnum.處方編輯);
            Features.Remove(prescriptionEdit);
        }
    }
}