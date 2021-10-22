using AutoMapper;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class MapperProfileDTO : Profile
    {
        public MapperProfileDTO()
        {
            CreateMap<Palavra, PalavraDTO>();

            // para situações que o nome de uma propriedade e um objeto esta diferente do outro...
            // utileze o "...().ForMember()" informando que campo x da objeto 1 vai receber o campo y do objeto 2
        }
    }
}
