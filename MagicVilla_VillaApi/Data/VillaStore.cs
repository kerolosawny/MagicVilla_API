using MagicVilla_VillaApi.Models.Dto;

namespace MagicVilla_VillaApi.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> VillaList = new List<VillaDTO>
            {
                new VillaDTO { Id = 1, Name ="pool view" , sqft =100 , occupancy =4 },
                new VillaDTO { Id = 2, Name ="beach view" ,  sqft =200 , occupancy =6 }
            };
    }
}
