using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesApi
{
    public interface IFacesApiClient
    {
        Task<OperationResult> CreatePerson(string name, string groupId);
        Task<OperationResult> AddPersonFace(string groupId, string personId, byte[] imageBytes);
        Task<OperationResult> DetectFace(byte[] imageBytes);
        Task<OperationResult> Identify(string groupId, string personId);
        Task<OperationResult> TrainGroup(string groupId);
    }
}
