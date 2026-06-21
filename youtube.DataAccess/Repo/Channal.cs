using System;
using System.Collections.Generic;
using System.Text;
using youtube.DataAccess.Data;

namespace youtube.DataAccess.Repo
{
    public class ChannalRepo: BaseRepo<youtube.core.Entities.Channal>, youtube.core.IRepo.IChannal
    {
        public ChannalRepo(Context context) : base(context, context.Channals)
        {
        }
    }
}
