using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace youtube.core.Entities
{
    public  class BaseEntitiy
    {
        [Key]
        public int Id { get; set; }

    }
}
