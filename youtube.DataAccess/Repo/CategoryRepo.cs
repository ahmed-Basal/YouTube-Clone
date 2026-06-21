using System;
using System.Collections.Generic;
using System.Text;
using youtube.core.Entities;
using youtube.DataAccess.Data;

namespace youtube.DataAccess.Repo
{
    public class CategoryRepo : BaseRepo<Category>, youtube.core.IRepo.ICategoryRepo
    {
        public CategoryRepo(Context context) : base(context, context.Categories)
        {
        }
    }
}
