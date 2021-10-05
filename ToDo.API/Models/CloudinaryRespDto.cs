using System;

namespace ToDo.API.Models
{
    public class CloudinaryRespDto
    {
         public string public_id { get; set; }
         public int version { get; set; }
         public string signature { get; set; }
         public string resource_type { get; set; }
         public DateTime created_at { get; set; }
         public string type { get; set; }
         public string etag { get; set; }
         public string url { get; set; }
         public string secure_url { get; set; }
         public string original_filename { get; set; }
    }
}
