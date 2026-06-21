using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using youtube.core.IRepo;
using youtube.DataAccess.Data;

namespace youtube.DataAccess.Repo
{
    public class UnitOfWork : youtube.core.IRepo.IUnirOFWork
    {
        private readonly Context _Context;

        public UnitOfWork(Context context)
        {
            _Context = context;
        }

        public IChannal Channal => new ChannalRepo(_Context);

        public IVideoRepo Video => new VideoRepo(_Context);

        public ICategoryRepo Category => new CategoryRepo(_Context);

        public ICommentRepo Comment => new CommentRepo(_Context);

        public async Task<bool> CompleteAsync()
        {
            bool result = false;

            if (_Context.ChangeTracker.HasChanges())
            {
                result = await _Context.SaveChangesAsync() > 0;
            }

            return result;
        }

        public void Dispose()
        {
            _Context.Dispose();
        }
    }
}
