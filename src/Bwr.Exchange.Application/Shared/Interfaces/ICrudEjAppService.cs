using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Shared.Interfaces
{
    public interface ICrudEjAppService<IReadDto, ICreateDto, IUpdateDto> 
        where IReadDto : class
        where ICreateDto : class
        where IUpdateDto : class
    {
        Task<IList<IReadDto>> GetAllAsync();
        ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm);
        IUpdateDto GetForEdit(int id);
        Task<IReadDto> CreateAsync(ICreateDto input);
        Task<IReadDto> UpdateAsync(IUpdateDto input);
        Task DeleteAsync(int id);
    }
}
