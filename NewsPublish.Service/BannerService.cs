using System;
using System.Collections.Generic;
using System.Text;
using NewsPublish.Model.Response;
using NewsPublish.Model.Request;
using NewsPublish.Model.Entity;
using System.Linq;

namespace NewsPublish.Service
{
    public class BannerService
    {
        private Db _db;
        public BannerService(Db db)
        {
            this._db = db;
        }
        /// <summary>
        /// Add banner
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        public ResponseModel AddBanner(AddBanner banner)
        {
            var ba = new Banner { AddTime = DateTime.Now, Image = banner.Image, Url = banner.Url, Remark = banner.Remark };
            _db.Banner.Add(ba);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Banner is created successfully" };
            return new ResponseModel { code = 0, result = "Fail to create new Banner" };

        }

        /// <summary>
        /// Get Banner List
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetBannerList()
        {
            var banners = _db.Banner.Where(c=>c.IndDelete!="X").ToList().OrderByDescending(c=>c.AddTime);//_db.Banner.ToList().OrderByDescending(c => c.AddTime); 
            var response = new ResponseModel();
            response.code = 200;
            response.result = "Successfully get Banner List";
            response.data = new List<BannerModel>();
            foreach (var banner in banners)
            {
                response.data.Add(new BannerModel
                {
                    Id = banner.Id,
                    Image = banner.Image,
                    Url = banner.Url,
                    Remark = banner.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// Delete Banner
        /// </summary>
        /// <param name="bannerId"></param>
        /// <returns></returns>
        public ResponseModel DeleteBanner(int bannerId)
        {
            var banner = _db.Banner.Find(bannerId);
            if (banner == null)
                return new ResponseModel { code = 0, result = "Banner does not exist." };
            //_db.Banner.Remove(banner);
            banner.IndDelete = "X";
            _db.Banner.Update(banner);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Banner is deleted successfully" };
            return new ResponseModel { code = 0, result = "Fail to delete new Banner" };
        }
    }
}
