//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialNetwork.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ratings_to_objects_with_rating
    {
        public int id { get; set; }
        public int user_id_from { get; set; }
        public int object_id { get; set; }
        public int value { get; set; }
    
        public virtual objects objects { get; set; }
        public virtual users users { get; set; }
    }
}
