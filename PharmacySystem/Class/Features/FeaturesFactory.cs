using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using PharmacySystem.Medicine.ControlMedicine;
using PharmacySystem.OrdersOperation;
using PharmacySystem.PrescriptionsOperation.Declare;
using PharmacySystem.PrescriptionsOperation.Dispense;
using PharmacySystem.PrescriptionsOperation.Search;
using PharmacySystem.PurchaseRequisition;
using PharmacySystem.UserControl;
using PharmacySystem.View;

namespace PharmacySystem.Class.Features
{
    public static class FeaturesFactory
    {
        private static readonly Dictionary<FeaturesEnum, FeatureItem> Features = new Dictionary<FeaturesEnum, FeatureItem>
        {
            {
                FeaturesEnum.首頁,
                new FeatureItem(FeaturesEnum.首頁,
                    new BitmapImage(new Uri("../../Images/Pharmacy.png", UriKind.Relative)))
            },
            {
                FeaturesEnum.調劑,
                new FeatureItem(FeaturesEnum.調劑,
                    new BitmapImage(new Uri("../../Images/Dispense.png", UriKind.Relative)))
            },
            {
                FeaturesEnum.處方查詢,
                new FeatureItem(FeaturesEnum.處方查詢,
                    new BitmapImage(new Uri("../../Images/PrescriptionSearch.png", UriKind.Relative)),
                    FeaturesType.處方作業)
            },
            {
                FeaturesEnum.處方申報,
                new FeatureItem(FeaturesEnum.處方申報,
                    new BitmapImage(new Uri("../../Images/PrescriptionDeclare.png", UriKind.Relative)),
                    FeaturesType.處方作業)
            },
            {
                FeaturesEnum.管藥申報,
                new FeatureItem(FeaturesEnum.管藥申報,
                    new BitmapImage(new Uri("../../Images/ControlledDrug.png", UriKind.Relative)),
                    FeaturesType.處方作業)
            },
            {
                FeaturesEnum.採購單管理,
                new FeatureItem(FeaturesEnum.採購單管理,
                    new BitmapImage(new Uri("../../Images/StoreOrderManagement.png", UriKind.Relative)),
                    FeaturesType.庫存管理)
            },
            {
                FeaturesEnum.訂單管理,
                new FeatureItem(FeaturesEnum.訂單管理,
                    new BitmapImage(new Uri("../../Images/StoreOrderManagement.png", UriKind.Relative)),
                    FeaturesType.庫存管理)
            },
            {
                FeaturesEnum.盤點作業,
                new FeatureItem(FeaturesEnum.盤點作業,
                    new BitmapImage(new Uri("../../Images/Inventory.png", UriKind.Relative)),
                    FeaturesType.庫存管理)
            },
            {
                FeaturesEnum.產品查詢,
                new FeatureItem(FeaturesEnum.產品查詢,
                    new BitmapImage(new Uri("../../Images/ProductSearch.png", UriKind.Relative)),
                    FeaturesType.庫存管理)
            },
            {
                FeaturesEnum.處方編輯,
                new FeatureItem(FeaturesEnum.處方編輯,
                    new BitmapImage(new Uri("../../Images/PrescriptionEdit.png", UriKind.Relative)),
                    FeaturesType.處方作業)
            }
        };

        public static HeaderWithCloseViewModel GetFeatureHeader(FeaturesEnum feature)
        {
            switch (feature)
            {
                case FeaturesEnum.首頁:
                case FeaturesEnum.調劑:
                    return new HeaderWithCloseViewModel(Features[feature].Feature,
                        Features[feature].Icon, false);
                default:
                    return new HeaderWithCloseViewModel(Features[feature].Feature,
                        Features[feature].Icon);
            }
        }

        public static object GetFeatureContent(FeaturesEnum feature)
        {
            switch (feature)
            {
                case FeaturesEnum.首頁:
                    return new HomePageView();
                case FeaturesEnum.調劑:
                    return new DispenseView();
                case FeaturesEnum.處方查詢:
                    return new PrescriptionSearchView();
                case FeaturesEnum.處方申報:
                    return new PrescriptionDeclareView();
                case FeaturesEnum.管藥申報:
                    return new ControlledMedicationDeclareView();
                case FeaturesEnum.採購單管理:
                    return new PurchaseRequisitionManagementView();
                case FeaturesEnum.訂單管理:
                    return new StoreOrderManagementView();
                case FeaturesEnum.盤點作業:
                    return new InventoryView();
                case FeaturesEnum.產品查詢:
                    return new ProductSearchView();
                default:
                    return null;
            }
        }

        public static List<FeatureItem> GetFavoriteList()
        {
            var favorites = new List<FeaturesEnum> { FeaturesEnum.處方查詢 };
            var favoritesFeatures = favorites.Select(f => new FeatureItem(f, Features[f].Icon)).ToList();
            return favoritesFeatures;
        }

        public static List<FeaturesCollection> GetFeaturesCollection()
        {
            var featuresCollections = new List<FeaturesCollection>();
            foreach (var type in (FeaturesType[])Enum.GetValues(typeof(FeaturesType)))
            {
                if (type.Equals(FeaturesType.None))
                    continue;
                var features = Features.Select(f => f.Value).Where(f => f.Type.Equals(type));
                var collection = new FeaturesCollection(type, features);
                featuresCollections.Add(collection);
            }
            return featuresCollections;
        }
    }
}
