using System;
using System.Collections.Generic;
using System.Text;
using youtube.core.Entities;
using youtube.DataAccess.Data;

namespace youtube.DataAccess.Repo
{
    public class CommentRepo : BaseRepo<Comment>, youtube.core.IRepo.ICommentRepo
    {
        public CommentRepo(Context context) : base(context, context.Comments)
        {
        }
    }
}
