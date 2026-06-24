using AutoMapper;
using jwt_rest_api.Models;
using jwt_rest_api.Models.Dto;
using System.Text.Json;

namespace jwt_rest_api.MappingProfiles;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        // Instruksi: Cara memindahkan dari GameProgressDto (Kardus) ke GameProgress (Brankas)
        CreateMap<GameProgressDto, GameProgress>()
            
            // 1. ABAIKAN kolom UserId. (Kita tidak mau ID database tertimpa barang dari luar!)
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            
            // 2. KASUS KHUSUS: Beda nama dan beda bentuk tipe data
            // Kardus: "StateData" (JsonElement) --> Brankas: "StateDataJson" (String)
            .ForMember(dest => dest.StateDataJson, 
                       opt => opt.MapFrom(src => src.StateData.ToString()))
            
            // Kardus: "Inventory" (List/Array) --> Brankas: "InventoryJson" (String)
            .ForMember(dest => dest.InventoryJson, 
                       opt => opt.MapFrom(src => JsonSerializer.Serialize(src.Inventory, (JsonSerializerOptions)null)));
                       
        // Catatan: Kolom Level, Score, dan Coins tidak perlu ditulis karena namanya sama persis! Robot akan mengurusnya otomatis.
    }
}
