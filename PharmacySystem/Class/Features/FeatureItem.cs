using System.Windows.Media;

namespace PharmacySystem.Class.Features
{
    public class FeatureItem
    {
        public FeaturesType Type { get; }
        public FeaturesEnum Feature { get; }
        public ImageSource Icon { get; }
        public int Width { get; }
        public FeatureItem(FeaturesEnum feature, ImageSource icon, FeaturesType type = FeaturesType.None)
        {
            Type = type;
            Feature = feature;
            Icon = icon;
            switch (feature)
            {
                case FeaturesEnum.訂單管理:
                    Width = 22;
                    break;
                case FeaturesEnum.盤點作業:
                    Width = 23;
                    break;
                default:
                    Width = 25;
                    break;
            }
        }
    }
}