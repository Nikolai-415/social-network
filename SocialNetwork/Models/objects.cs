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
    
    public partial class objects
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public objects()
        {
            this.commentaries = new HashSet<commentaries>();
            this.commentaries_to_objects_with_commentaries = new HashSet<commentaries_to_objects_with_commentaries>();
            this.ratings_to_objects_with_rating = new HashSet<ratings_to_objects_with_rating>();
        }
    
        public int id { get; set; }
        public int object_type_id { get; set; }
        public int user_id_from { get; set; }
        public int creation_datetime_int { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<commentaries> commentaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<commentaries_to_objects_with_commentaries> commentaries_to_objects_with_commentaries { get; set; }
        public virtual object_types object_types { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ratings_to_objects_with_rating> ratings_to_objects_with_rating { get; set; }
    }
}
