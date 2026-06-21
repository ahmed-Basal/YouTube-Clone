using System;
using System.Collections.Generic;
using System.Text;
using youtube.core.Entities;
using youtube.DataAccess.Data;

namespace youtube.DataAccess.Repo
{
    public class VideoRepo : BaseRepo<videos>, youtube.core.IRepo.IVideoRepo
    {
        public VideoRepo(Context context) : base(context, context.videos)
        {
        }
    }
}
