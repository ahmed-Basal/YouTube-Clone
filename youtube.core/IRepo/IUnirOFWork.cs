using System;
using System.Collections.Generic;
using System.Text;

namespace youtube.core.IRepo
{
    public  interface IUnirOFWork: IDisposable
    {
        IChannal Channal { get; }
        IVideoRepo Video { get; }
        ICategoryRepo Category { get; }
        ICommentRepo Comment { get; }
       // IComment Comment { get; }
       // IVideo Video { get; }
       // IPlaylist Playlist { get; }
        //IPlaylistVideo PlaylistVideo { get; }
       Task<bool> CompleteAsync();
    }
}
