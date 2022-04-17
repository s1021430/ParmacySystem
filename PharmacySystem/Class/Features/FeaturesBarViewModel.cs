using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PharmacySystem.Class.Features
{
    public class FeaturesBarViewModel : ViewModelBase
    {
        public List<FeatureItem> FavoriteList { get; }
        public List<FeaturesCollection> FeaturesCollections { get; }
        public FeaturesBarViewModel()
        {
            FavoriteList = FeaturesFactory.GetFavoriteList();
            FeaturesCollections = FeaturesFactory.GetFeaturesCollection();
        }
    }
}